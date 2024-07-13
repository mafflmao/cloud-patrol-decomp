using System;
using UnityEngine;

public class FooterUI : MonoBehaviour
{
	public const string LEVEL_STATE_NAME = "RankInfo";

	public const string GEMS_STATE_NAME = "GemStore";

	public const string RAD_STATE_NAME = "GemConverter";

	public static FooterUI _inst;

	[SerializeField]
	private bool _visible;

	public Transform container;

	public UIButton btnLevel;

	public PackedSprite levelShield;

	public UIButton btnGems;

	public UIButton btnCoins;

	public LoadoutElementIcon elementIcon;

	public UIButton btnElementOfTheDay;

	public Nag nagGems;

	public Nag nagCoins;

	public Nag nagLevel;

	public Nag nagElementOfTheDay;

	public FooterStarContainer starContainer;

	public SoundEventData coinAwardSFX;

	public ParticleSystem coinAwardParticleFX;

	public Vector3 startPosition;

	private Elements.Type _currentElementOfTheDay;

	public static FooterUI Instance
	{
		get
		{
			if (_inst == null)
			{
				GameObject gameObject = GameObject.Find("Footer");
				if (gameObject == null)
				{
					gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("UI Prefabs/Common/Footer"));
					gameObject.name = "Footer";
				}
				_inst = gameObject.GetComponent<FooterUI>();
			}
			return _inst;
		}
	}

	[HideInInspector]
	public bool visible
	{
		get
		{
			return _visible;
		}
		set
		{
		}
	}

	private void Awake()
	{
		if (_inst != null)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			return;
		}
		_inst = this;
		UIManager.AddAction<FooterUI>(btnLevel, base.gameObject, "OnLevelBtnClick", POINTER_INFO.INPUT_EVENT.RELEASE);
		UIManager.AddAction<FooterUI>(btnGems, base.gameObject, "OnGemsBtnClick", POINTER_INFO.INPUT_EVENT.RELEASE);
		UIManager.AddAction<FooterUI>(btnCoins, base.gameObject, "OnRadBtnClick", POINTER_INFO.INPUT_EVENT.RELEASE);
		container.gameObject.transform.localPosition = new Vector3(0f, -300f, 0f);
	}

	private void Start()
	{
		UpdateSize();
		UpdateVisibility();
		UpdateValuesFromData();
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void UpdateSize()
	{
		float width = GUISystem.Instance.guiCamera.width;
		btnLevel.width = width - (btnGems.width + btnCoins.width + btnElementOfTheDay.width);
		btnLevel.transform.localPosition = new Vector3((0f - GUISystem.Instance.guiCamera.width) / 2f, btnLevel.transform.localPosition.y, btnLevel.transform.localPosition.z);
		btnGems.transform.localPosition = new Vector3(btnLevel.transform.localPosition.x + btnLevel.width, btnGems.transform.localPosition.y, btnGems.transform.localPosition.z);
		btnCoins.transform.localPosition = new Vector3(btnLevel.transform.localPosition.x + btnLevel.width + btnGems.width, btnCoins.transform.localPosition.y, btnCoins.transform.localPosition.z);
		btnElementOfTheDay.transform.localPosition = new Vector3(btnLevel.transform.localPosition.x + btnLevel.width + btnGems.width + btnCoins.width, btnElementOfTheDay.transform.localPosition.y, btnElementOfTheDay.transform.localPosition.z);
	}

	private void HandlePlayerGemsChanged(object sender, EventArgs e)
	{
		UpdateValuesFromData();
	}

	private void HandlePlayerCoinsChanged(object sender, EventArgs e)
	{
		UpdateValuesFromData();
	}

	private void HandlePlayerRankOrStarsChanged(object sender, EventArgs e)
	{
		UpdateValuesFromData();
	}

	private void HandleBonusElementChanged(object sender, EventArgs e)
	{
		if (ElementOfTheDayChanger.AllowElementOfTheDayChanges)
		{
			elementIcon.UpdateGraphic();
			nagElementOfTheDay.nagText = ElementOfTheDayChanger.ElementOfTheDayNagText;
			if ((StateManager.Instance.CurrentStateName == "Loadout" || StateManager.Instance.CurrentStateName == "Results") && _currentElementOfTheDay != 0)
			{
				_currentElementOfTheDay = Elements.Type.Air;
				nagElementOfTheDay.Show();
			}
		}
	}

	private void UpdateVisibility()
	{
		if (_visible)
		{
			AnimateIn();
		}
		else
		{
			AnimateOut();
		}
	}

	private void UpdateValuesFromData()
	{
		btnLevel.spriteText.Text = RankDataManager.Instance.CurrentRank.Rank.RankNumber.ToString();
		starContainer.UpdateData();
		starContainer.UpdateGraphics();
	}

	public void ShowCoinRewardFeedback()
	{
		SoundEventManager.Instance.Play(coinAwardSFX, GUISystem.Instance.guiCamera.gameObject);
		coinAwardParticleFX.Play();
	}

	public void OnStateChanged()
	{
		btnLevel.controlIsEnabled = true;
		btnGems.controlIsEnabled = true;
		btnCoins.controlIsEnabled = true;
		if (StateManager.Instance.CurrentStateName == "GemStore")
		{
			btnGems.controlIsEnabled = false;
		}
		else if (StateManager.Instance.CurrentStateName == "GemConverter")
		{
			btnCoins.controlIsEnabled = false;
		}
	}

	public void OnLevelBtnClick()
	{
		if (!Nag.nagIsShowing)
		{
			nagLevel.Hide();
			RankAndStars currentRank = RankDataManager.Instance.CurrentRank;
			int num = currentRank.Rank.StarsForNextRank - currentRank.Stars;
			string key = ((num != 1) ? "FOOTER_LEVEL_NAG_PLURAL" : "FOOTER_LEVEL_NAG_SINGLE");
			nagLevel.nagText = LocalizationManager.Instance.GetFormatString(key, num, SwrveEconomy.RankGemsAwarded);
			nagLevel.Show();
		}
	}

	public void OnGemsBtnClick()
	{
		nagGems.Hide();
		SwrveEventsUI.GemBarButtonTouched();
		HeaderUI.Instance.titleString = LocalizationManager.Instance.GetString("GEMSTORE_TITLE");
		StateManager.Instance.LoadAndActivateState("GemStore");
	}

	public void OnRadBtnClick()
	{
		nagCoins.Hide();
		SwrveEventsUI.CoinBarButtonTouched();
		StateManager.Instance.LoadAndActivateState("GemConverter");
	}

	public void OnElementOfTheDayBtnClick()
	{
		SwrveEventsUI.ElementOfTheDayTouched();
		nagElementOfTheDay.Show();
	}

	public void ShowGemsNag()
	{
		if (!Nag.nagIsShowing)
		{
			nagGems.Show();
		}
	}

	public void ShowCoinsNag()
	{
		if (!Nag.nagIsShowing)
		{
			nagCoins.Show();
		}
	}

	public void AnimateOut()
	{
		nagGems.Hide();
		nagCoins.Hide();
		nagElementOfTheDay.Hide();
		nagLevel.Hide();
		iTween.MoveTo(container.gameObject, iTween.Hash("position", new Vector3(0f, -300f, 0f), "time", 0.5f, "islocal", true));
	}

	public void AnimateIn()
	{
		elementIcon.UpdateGraphic();
		nagElementOfTheDay.nagText = ElementOfTheDayChanger.ElementOfTheDayNagText;
		container.gameObject.transform.localPosition = new Vector3(0f, -300f, 0f);
		iTween.MoveTo(container.gameObject, Vector3.zero, 0.5f);
		if (ElementOfTheDayChanger.UpdatedElementOfTheDay)
		{
			ElementOfTheDayChanger.UpdatedElementOfTheDay = false;
			nagElementOfTheDay.Show(2f);
		}
	}
}
