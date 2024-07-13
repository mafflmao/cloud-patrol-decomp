using System;
using System.Collections;
using UnityEngine;

public class PortalLinkScreen : MonoBehaviour
{
	public enum HelpTextState
	{
		Disconnected = 0,
		Connected = 1,
		TooManyToys = 2
	}

	public PortalManager portalManager;

	public ToyLinkManager toyLinkManager;

	public SoundEventData enterPressedSound;

	public GameObject portalSprite;

	public int gemsToReimburse;

	private string _oldHeaderText;

	private IDisposable _backButtonOverrideContext;

	public SpriteText subText;

	public SpriteText headingText;

	public string[] errorStrings;

	public ParticleSystem sparkles;

	public SoundEventData connectionSound;

	public bool debugFoundPortal;

	private bool shuttingDown;

	private ILinkable _linkableData;

	private bool _portalWasConnected;

	private bool _wrongToyWasPlaced;

	public Action SuccessAction { get; set; }

	public Action DismissAction { get; set; }

	public ILinkable LinkableData
	{
		get
		{
			return _linkableData;
		}
		set
		{
			_linkableData = value;
			UpdateText();
		}
	}

	public bool PortalWasConnected
	{
		get
		{
			return _portalWasConnected;
		}
		set
		{
			if (!_portalWasConnected && value)
			{
				_portalWasConnected = true;
				SwrveEventsProgression.PortalLinkConnectedPortal(LinkableData.ToyLinkDisplayName, gemsToReimburse);
			}
		}
	}

	public bool WrongToyWasPlaced
	{
		get
		{
			return _wrongToyWasPlaced;
		}
		set
		{
			if (!_wrongToyWasPlaced && value)
			{
				_wrongToyWasPlaced = true;
				SwrveEventsProgression.PortalLinkWrongToy(LinkableData.ToyLinkDisplayName, gemsToReimburse);
			}
		}
	}

	private void Awake()
	{
		if (PortalManager.Instance == null)
		{
			UnityEngine.Object.Instantiate(portalManager);
		}
		PortalManager.Instance.ShouldDetectToys = true;
		if (ToyLinkManager.Instance == null)
		{
			UnityEngine.Object.Instantiate(toyLinkManager);
		}
	}

	private void Start()
	{
		SwrveEventsProgression.PortalLinkEnterScreen(LinkableData.ToyLinkDisplayName, gemsToReimburse);
		StartCoroutine(UpdateHelpText());
		_backButtonOverrideContext = HeaderUI.Instance.PushBackButtonOverrideAction(BackButtonPressed);
		FooterUI.Instance.AnimateOut();
		StartCoroutine(FlipSubheadings());
	}

	private void OnEnable()
	{
		StateManager.StateDeactivated += HandleStateDeactivated;
	}

	private void OnDisable()
	{
		StateManager.StateDeactivated -= HandleStateDeactivated;
	}

	private void HandleStateDeactivated(object sender, StateEventArgs e)
	{
		Dismiss();
	}

	private void UpdateText()
	{
		if (HeaderUI.Instance.titleString != LinkableData.ToyLinkDisplayName)
		{
			_oldHeaderText = HeaderUI.Instance.titleString;
			HeaderUI.Instance.titleString = LinkableData.ToyLinkDisplayName;
		}
	}

	private IEnumerator UpdateHelpText()
	{
		if (!(PortalManager.Instance != null))
		{
			yield break;
		}
		while (!shuttingDown)
		{
			if (!PortalManager.Instance.PortalConnected && !debugFoundPortal)
			{
				headingText.Text = LocalizationManager.Instance.GetString("PORTALUI_CANNOTFIND");
				subText.Hide(false);
				portalSprite.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.2f, 0.3f, 0.2f, 1f));
				portalSprite.GetComponent<Animation>().Play("PortalWait");
			}
			else
			{
				PortalWasConnected = true;
				portalSprite.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 1f));
				portalSprite.GetComponent<Animation>().Play("PortalBounce");
				subText.Hide(true);
				if (PortalManager.Instance.TooManyToysDetected)
				{
					WrongToyWasPlaced = true;
					headingText.Text = LocalizationManager.Instance.GetString("PORTALUI_TOOMANYTOYS");
				}
				else if (PortalManager.Instance.ToyDetected && !LinkableData.MatchesToyAndSubtype(PortalManager.Instance.DetectedToy, PortalManager.Instance.DetectedToySubType))
				{
					WrongToyWasPlaced = true;
					headingText.Text = LocalizationManager.Instance.GetFormatString("PORTALUI_WRONGTOY", LinkableData.ToyLinkDisplayName);
				}
				else
				{
					headingText.Text = LocalizationManager.Instance.GetFormatString("PORTALUI_PLACETOY", LinkableData.ToyLinkDisplayName);
				}
			}
			if (PortalManager.Instance.ToyDetected && LinkableData.MatchesToyAndSubtype(PortalManager.Instance.DetectedToy, PortalManager.Instance.DetectedToySubType))
			{
				StartCoroutine(DelayDismiss());
				SuccessAction();
			}
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator FlipSubheadings()
	{
		if (errorStrings.Length > 0)
		{
			int i = 0;
			while (true)
			{
				subText.transform.localScale = new Vector3(1f, 0f, 1f);
				subText.Text = LocalizationManager.Instance.GetString(errorStrings[i]);
				iTween.ScaleTo(subText.gameObject, iTween.Hash("scale", Vector3.one, "time", 0.2f, "easetype", iTween.EaseType.linear));
				yield return new WaitForSeconds(2.5f);
				iTween.ScaleTo(subText.gameObject, iTween.Hash("scale", new Vector3(1f, 0f, 1f), "time", 0.2f, "easetype", iTween.EaseType.linear));
				yield return new WaitForSeconds(0.4f);
				i = ((i < errorStrings.Length - 1) ? (i + 1) : 0);
			}
		}
	}

	private IEnumerator DelayDismiss()
	{
		SwrveEventsProgression.PortalLinkToySuccess(LinkableData.ToyLinkDisplayName, gemsToReimburse);
		PortalManager.Instance.ShouldDetectToys = false;
		shuttingDown = true;
		yield return new WaitForSeconds(2f);
		Dismiss();
	}

	private void BackButtonPressed()
	{
		Dismiss();
	}

	private void Dismiss()
	{
		PortalManager.Instance.ShouldDetectToys = false;
		if (_oldHeaderText != null)
		{
			HeaderUI.Instance.titleString = _oldHeaderText;
		}
		if (_backButtonOverrideContext != null)
		{
			_backButtonOverrideContext.Dispose();
		}
		FooterUI.Instance.AnimateIn();
		if (DismissAction != null)
		{
			DismissAction();
		}
		UnityEngine.Object.Destroy(base.gameObject);
		HeaderUI.Instance.ShowSocialButtons(true);
		UIManager.instance.blockInput = false;
	}

	private void OnHelpBtnClick()
	{
		SwrveEventsProgression.PortalLinkHelpButtonHit(LinkableData.ToyLinkDisplayName, gemsToReimburse, PortalWasConnected, WrongToyWasPlaced);
		ActivateWatcher.Instance.ShowActivateUI(Bedrock.brUserInterfaceScreen.BR_CUSTOMER_SERVICE_UI);
	}
}
