using System;
using System.Collections;
using UnityEngine;

public class LoadoutController : StateController
{
	public const float PanelAnimateInTime = 0.5f;

	public static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(LoadoutController), LogLevel.Log);

	public BountyController bountyController;

	public Transform skylanderRoot;

	public GameObject leftPanel;

	public GameObject centerPanel;

	public GameObject activeCharacterLight;

	public Camera skylanderCamera;

	public SoundEventData championOutSFX;

	public SoundEventData playBtnSFX;

	public SoundEventData characterMenuSelectSfx;

	public SaleDialog saleDialogPrefab;

	[HideInInspector]
	public GameObject riggedSkylander;

	private Vector3 skylanderCameraStartPos;

	private bool _playButtonPressed;

	public SaleTag saleTag;

	public GameObject ButtonTintAnimNode;

	private CharacterData _visibleSkylander;

	private bool ShowingActiveSkylander
	{
		get
		{
			return riggedSkylander != null && _visibleSkylander == StartGameSettings.Instance.activeSkylander;
		}
	}

	private void Awake()
	{
		skylanderCameraStartPos = skylanderCamera.transform.localPosition;
	}

	private void Start()
	{
		LoadSkylanderAndStartAnimation();
		skylanderCamera.enabled = false;
	}

	protected override void ShowState()
	{
		GetComponent<StateRoot>().stateTitle = LocalizationManager.Instance.GetFormatString("HEADER_HIGH_SCORE", "0");
		HeaderUI.Instance.titleString = GetComponent<StateRoot>().stateTitle;
		SwrveEconomy.UpdateCoinPacksFromSwrve(true);
		SwrveEconomy.UpdateGemPacksFromSwrve(false);
		saleTag.UpdateVisibility();
		PurchaseHandler.Instance.ReCheckForPendingCompletedPurchases();
		base.ShowState();
	}

	protected override void HideState()
	{
		base.HideState();
	}

	public IEnumerator TryShowSaleDialog()
	{
		return SaleDialog.TryShowSaleDialogCoroutine(saleDialogPrefab);
	}

	private void LoadSkylanderAndStartAnimation()
	{
		if (ShowingActiveSkylander)
		{
			return;
		}
		if (riggedSkylander != null)
		{
			UnityEngine.Object.Destroy(riggedSkylander);
			riggedSkylander = null;
			_visibleSkylander = null;
		}
		_visibleSkylander = StartGameSettings.Instance.activeSkylander;
		riggedSkylander = (GameObject)UnityEngine.Object.Instantiate(_visibleSkylander.GetRiggedModelPrefab());
		riggedSkylander.transform.parent = skylanderRoot.transform;
		if (_visibleSkylander.loadoutPosition == Vector3.zero)
		{
			riggedSkylander.transform.localScale = _visibleSkylander.detailsScale;
			riggedSkylander.transform.localEulerAngles = _visibleSkylander.detailsRotation;
			riggedSkylander.transform.localPosition = _visibleSkylander.detailsPosition;
		}
		else
		{
			riggedSkylander.transform.localScale = _visibleSkylander.loadoutScale;
			riggedSkylander.transform.localEulerAngles = _visibleSkylander.loadoutRotation;
			riggedSkylander.transform.localPosition = _visibleSkylander.loadoutPosition;
		}
		HideAccessory[] components = riggedSkylander.GetComponents<HideAccessory>();
		if (components != null)
		{
			HideAccessory[] array = components;
			foreach (HideAccessory hideAccessory in array)
			{
				if (hideAccessory != null)
				{
					hideAccessory.enabled = false;
				}
			}
		}
		HideParticle component = riggedSkylander.GetComponent<HideParticle>();
		if (component != null)
		{
			component.enabled = false;
		}
		AnimationUtils.PlayClip(riggedSkylander.GetComponent<Animation>(), "idle");
	}

	private void SetupSkylanderLighting()
	{
		GameObjectUtils.ManageLighting(activeCharacterLight, "Loadout");
		GameObjectUtils.ResetAmbientLight();
	}

	protected override IEnumerator AnimateStateIn()
	{
		leftPanel.transform.localPosition = new Vector3(-2546f, 0f, 0f);
		centerPanel.transform.localPosition = new Vector3(1241f, 0f, 0f);
		skylanderCamera.transform.localPosition = new Vector3(-51f, 24.87f, -48.7512f);
		bountyController.UpdateBounties();
		bountyController.Reset();
		HideState();
		while (StateManager.Instance.Loading)
		{
			yield return new WaitForSeconds(0.1f);
		}
		ShowState();
		LoadSkylanderAndStartAnimation();
		SetupSkylanderLighting();
		skylanderCamera.enabled = true;
		iTween.MoveTo(skylanderCamera.gameObject, iTween.Hash("position", skylanderCameraStartPos, "time", 0.5f, "islocal", true));
		iTween.MoveTo(leftPanel, new Vector3(0f, 0f, 0f), 0.5f);
		iTween.MoveTo(centerPanel, new Vector3(0f, 0f, 0f), 0.5f);
		StartCoroutine(TryShowSaleDialog());
	}

	protected override IEnumerator AnimateStateOut()
	{
		StopCoroutine("AnimateStateIn");
		if (_playButtonPressed)
		{
			FooterUI.Instance.nagElementOfTheDay.Hide();
		}
		else
		{
			iTween.MoveTo(skylanderCamera.gameObject, iTween.Hash("position", new Vector3(-51f, 24.87f, -48.7512f), "time", 0.5f, "islocal", true));
		}
		iTween.MoveTo(leftPanel, new Vector3(-2546f, 0f, 0f), 0.5f);
		iTween.MoveTo(centerPanel, new Vector3(1241f, 0f, 0f), 0.5f);
		yield return new WaitForSeconds(0.25f);
		if (_playButtonPressed)
		{
			SoundEventManager.Instance.Play2D(championOutSFX);
			AnimationUtils.PlayClip(riggedSkylander.GetComponent<Animation>(), "jump");
			yield return new WaitForSeconds(0.5f);
		}
		if (riggedSkylander != null)
		{
			skylanderCamera.enabled = false;
		}
		yield return new WaitForSeconds(0.5f);
		HideState();
		_playButtonPressed = false;
	}

	private void OnPlayBtnClick()
	{
		if (!UIManager.instance.blockInput)
		{
			ElementOfTheDayChanger.AllowElementOfTheDayChanges = false;
			SoundEventManager.Instance.Play2D(playBtnSFX);
			UIManager.instance.blockInput = true;
			MusicManager.Instance.StopMusic();
			_playButtonPressed = true;
			InvokeHelper.InvokeSafe(TransitionController.Instance.StartTransitionFromFrontEnd, 0.25f, TransitionController.Instance);
		}
	}

	private void OnSkylanderBtnClick()
	{
		UIManager.instance.blockInput = true;
		SoundEventManager.Instance.Play2D(characterMenuSelectSfx);
		iTween.PunchScale(riggedSkylander, new Vector3(0.2f, 0.2f, 0.2f), 0.5f);
		Action action = delegate
		{
			SkylanderSelectController.LastStateName = "Loadout";
			StateManager.Instance.LoadAndActivateState("SkylanderSelect");
		};
		InvokeHelper.InvokeSafe(action, 0.3f, this);
	}

	private void OnStoreBtnClick()
	{
		UIManager.instance.blockInput = true;
		SwrveEventsUI.CollectionButtonTouched();
		Action action = delegate
		{
			StateManager.Instance.LoadAndActivateState("StoreHub");
		};
		InvokeHelper.InvokeSafe(action, 0.5f, this);
	}

	private void OnBountyBtnClick()
	{
		StateManager.Instance.LoadAndActivateState("Bounties");
	}
}
