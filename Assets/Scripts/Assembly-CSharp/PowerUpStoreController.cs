using System;
using System.Collections;
using UnityEngine;

public class PowerUpStoreController : StateController
{
	public enum PowerUpStoreState
	{
		NORMAL = 0,
		MESSAGE_BOX = 1,
		CALLOUT = 2
	}

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(PowerUpStoreController), LogLevel.Debug);

	public static string OneTimeItemToSelect;

	public ScrollListController scrollListController;

	public MessageTray tray;

	public MagicItemCallout callout;

	public SoundEventData sfxCalloutClosed;

	public SoundEventData sfxMagicItemUnlock;

	public SoundEventData sfxMagicItemUpgrade;

	public ConfirmationDialog confirmationDialogPrefab;

	public SoundEventData confirmationDialogOkSound;

	public SoundEventData confirmationDialogCancelSound;

	public SoundEventData confirmationDialogClosingSound;

	public PowerUpStoreState currState;

	public Animation fxLevelUp;

	public Animation fxUnlock;

	public ToyLinkScreen toyLinkScreenPrefab;

	public PortalLinkScreen portalLinkScreenPrefab;

	public GemRefundDialog gemRefundDialogPrefab;

	public GameObject saleTagPrefab;

	public ExpandableButtonComposite collectionVaultExpander;

	private BaseItemStoreButton _currButton;

	private IDisposable _backButtonOverrideContext;

	private string CurrentPowerupReadableName
	{
		get
		{
			return _currButton.powerupData.LocalizedName;
		}
	}

	private void OnEnable()
	{
		BaseItemStoreButton.ButtonPressed += HandleBaseItemStoreButtonPressed;
	}

	private void OnDisable()
	{
		BaseItemStoreButton.ButtonPressed -= HandleBaseItemStoreButtonPressed;
	}

	protected override void ShowState()
	{
		base.ShowState();
		ResetState();
	}

	protected override void HideState()
	{
		base.HideState();
		if (_backButtonOverrideContext != null)
		{
			_backButtonOverrideContext.Dispose();
			_backButtonOverrideContext = null;
		}
	}

	private void ResetState()
	{
		_log.LogDebug("ResetState");
		tray.FadeInImmediate();
		scrollListController.Reset();
		callout.Hide();
		if (_backButtonOverrideContext != null)
		{
			_backButtonOverrideContext.Dispose();
			_backButtonOverrideContext = null;
		}
		if (_currButton != null)
		{
			_currButton.gameObject.transform.localScale = Vector3.one;
			_currButton.ShowUpgradeNotifyOverlaysIfAvailable = true;
			_currButton.UpdateGraphics();
		}
		_currButton = null;
		if (string.IsNullOrEmpty(OneTimeItemToSelect))
		{
			return;
		}
		_log.LogDebug("Using one-time-item-to-select {0}", OneTimeItemToSelect);
		BaseItemStoreButton baseItemStoreButton = null;
		BaseItemStoreButton[] componentsInChildren = scrollListController.GetComponentsInChildren<BaseItemStoreButton>();
		foreach (BaseItemStoreButton baseItemStoreButton2 in componentsInChildren)
		{
			PowerupData powerupData = baseItemStoreButton2.powerupData;
			_log.LogDebug("Checking powerupData vs {0}", powerupData.storageKey);
			if (powerupData.storageKey == OneTimeItemToSelect)
			{
				_log.LogDebug("Found matching button!");
				baseItemStoreButton = baseItemStoreButton2;
				break;
			}
		}
		if (baseItemStoreButton != null)
		{
			_log.LogDebug("Triggering button click code.");
			OnPowerUpBtnClick(baseItemStoreButton);
		}
		_log.LogDebug("Clearing one-time-item-to-select field.");
		OneTimeItemToSelect = null;
	}

	private void HandleBaseItemStoreButtonPressed(object sender, EventArgs args)
	{
		_log.LogDebug("HandleBaseItemStoreButtonPressed");
		if (base.IsShowing)
		{
			OnPowerUpBtnClick((BaseItemStoreButton)sender);
		}
	}

	public void OnPowerUpBtnClick(BaseItemStoreButton btn)
	{
		_log.LogDebug("OnPowerUpBtnClick");
		if (_currButton != null)
		{
			_log.LogWarning("Already have a button stored. Aborting button press.");
			return;
		}
		_currButton = btn;
		_currButton.ShowUpgradeNotifyOverlaysIfAvailable = false;
		_currButton.UpdateGraphics();
		scrollListController.RemoveButton(_currButton.uiButton);
		scrollListController.FadeOut();
		tray.FadeOut();
		currState = PowerUpStoreState.CALLOUT;
		callout.powerupData = _currButton.powerupData;
		callout.Show();
		_backButtonOverrideContext = HeaderUI.Instance.PushBackButtonOverrideAction(HideCalloutOnBackButtonClick);
		StartCoroutine(BlockInputTimed());
		SwrveEventsUI.MagicItemTouched(CurrentPowerupReadableName);
	}

	private IEnumerator BlockInputTimed()
	{
		UIManager.instance.blockInput = true;
		yield return new WaitForSeconds(0.3f);
		UIManager.instance.blockInput = false;
	}

	private void HideCalloutOnBackButtonClick()
	{
		onCalloutBtnClick(false);
	}

	public void onCalloutBtnClick(bool confirm)
	{
		_log.LogDebug("onCalloutBtnClick({0})", confirm);
		SoundEventManager.Instance.Play2D(sfxCalloutClosed);
		_log.LogDebug("Setting current state.");
		currState = PowerUpStoreState.NORMAL;
		_log.LogDebug("Collapsing background.");
		collectionVaultExpander.CollapseImmediately();
		_log.LogDebug("Hiding callout.");
		callout.Hide();
		if (_backButtonOverrideContext != null)
		{
			_log.LogDebug("Disposing back button override context.");
			_backButtonOverrideContext.Dispose();
			_backButtonOverrideContext = null;
		}
		_log.LogDebug("Replacing button and fading back in.");
		scrollListController.ReplaceButtonAndFadeIn();
		_log.LogDebug("Fading tray in.");
		tray.FadeIn();
		_log.LogDebug("Starting block input coroutine.");
		StartCoroutine(BlockInputTimed());
		if (_currButton != null)
		{
			_log.LogDebug("Updating graphics on current button");
			_currButton.ShowUpgradeNotifyOverlaysIfAvailable = true;
			_currButton.UpdateGraphics();
			_currButton = null;
		}
	}

	private void OnCalloutUnlockBtnClick()
	{
		if (_currButton != null && _currButton.powerupData.IsToyClaimable)
		{
			TryShowRefundPrompt(OnToyClaimSuccess);
		}
	}

	private void UnlockAfterPurchase()
	{
		SwrveEventsPurchase.MagicItem(_currButton.powerupData);
		AchievementManager.Instance.autoSync = true;
		AchievementManager.Instance.IncrementStep(Achievements.BuyMagic);
		AchievementManager.Instance.IncrementStepBy(Achievements.CoinsSpend, _currButton.powerupData.cost);
		AchievementManager.Instance.autoSync = false;
		StartUnlockSequence(false);
	}

	private void PlayUnlockSuccessEffects()
	{
		SoundEventManager.Instance.Play2D(confirmationDialogOkSound);
		fxUnlock.gameObject.SetActive(true);
		fxUnlock.Play();
		fxUnlock.transform.position = _currButton.graphics[0].transform.position;
		fxUnlock.transform.Translate(new Vector3(0f, 0f, -10f));
	}

	private void PurchaseCancelled()
	{
		_log.Log("Powerup unlock cancelled.");
		SoundEventManager.Instance.Play2D(confirmationDialogCancelSound);
		if (_currButton != null)
		{
			SwrveEventsPurchase.MagicItemFailed(_currButton.powerupData);
		}
	}

	private void OnConsumableGemPurchaseBtnClick()
	{
		if (_currButton != null)
		{
			PowerupData powerupData = _currButton.powerupData;
			PurchaseUpgradeOrConsumable(powerupData, powerupData.PurchaseCost, PowerupData.CostType.Gems);
		}
	}

	private void OnConsumableCoinPurchaseBtnClick()
	{
		if (_currButton != null)
		{
			PowerupData powerupData = _currButton.powerupData;
			PurchaseUpgradeOrConsumable(powerupData, powerupData.AltPurchaseCost, PowerupData.CostType.Coins);
		}
	}

	private void OnCalloutUpgradeBtnClick()
	{
		if (_currButton != null)
		{
			PowerupData powerupData = _currButton.powerupData;
			int purchaseCost = powerupData.PurchaseCost;
			PowerupData.CostType purchaseCostType = powerupData.PurchaseCostType;
			PurchaseUpgradeOrConsumable(powerupData, purchaseCost, purchaseCostType);
		}
	}

	private void PurchaseUpgradeOrConsumable(PowerupData powerupData, int purchaseCost, PowerupData.CostType purchaseCostType)
	{
		if (!(_currButton != null))
		{
		}
	}

	private void OnCalloutLinkToyButtonClick()
	{
		_log.LogDebug("OnCalloutLinkToyButtonClick");
		collectionVaultExpander.CollapseImmediately();
		TryShowRefundPrompt(ShowToyLinkScreen);
	}

	private void OnCalloutLinkPortalButtonClick()
	{
		_log.LogDebug("OnCalloutLinkPortalButtonClick");
		collectionVaultExpander.CollapseImmediately();
		TryShowRefundPrompt(ShowPortalLinkScreen);
	}

	private void TryShowRefundPrompt(Action userConfirmedAction)
	{
		_log.LogDebug("PromptForRefundConfirm(...)");
		UIManager.instance.blockInput = true;
	}

	private void ShowToyLinkScreen()
	{
		Debug.Log("Showing toy link screen");
		ToyLinkScreen toyLinkScreen = (ToyLinkScreen)UnityEngine.Object.Instantiate(toyLinkScreenPrefab);
		toyLinkScreen.LinkableData = _currButton.powerupData;
		toyLinkScreen.SuccessAction = OnToyLinkSuccess;
		HeaderUI.Instance.ShowSocialButtons(false);
		UIManager.instance.blockInput = false;
	}

	private void ShowPortalLinkScreen()
	{
		Debug.Log("Showing portal link screen");
		PortalLinkScreen portalLinkScreen = (PortalLinkScreen)UnityEngine.Object.Instantiate(portalLinkScreenPrefab);
		portalLinkScreen.LinkableData = _currButton.powerupData;
		portalLinkScreen.SuccessAction = OnPortalLinkSuccess;
		HeaderUI.Instance.ShowSocialButtons(false);
		UIManager.instance.blockInput = false;
	}

	private void OnToyClaimSuccess()
	{
		_currButton.powerupData.UnlockFromToy(0u);
		OnToyLinkSuccess();
	}

	private void OnToyLinkSuccess()
	{
		_log.Log("OnToyLinkSuccess()");
		StartUnlockSequence(false);
	}

	private void OnPortalLinkSuccess()
	{
		StartUnlockSequence(true);
	}

	private void LinkSkylanderConfirmed()
	{
		SoundEventManager.Instance.Play2D(confirmationDialogOkSound);
		ToyLinkManager.Instance.BeginToyLink(_currButton.powerupData, PortalManager.Instance.DetectedToyWebcode, LinkFromPortalSuccess);
	}

	private void LinkFromPortalSuccess()
	{
		callout.UpdateGraphics();
		_currButton.UpdateGraphics();
	}

	private void StartUnlockSequence(bool unlockedFromPortal)
	{
		_log.LogDebug("StartUnlockSequence({0})", unlockedFromPortal);
		UIManager.instance.blockInput = true;
		UIManager.BlockInputChanging += HandleUIManagerBlockInputChanging;
		SoundEventManager.Instance.Play2D(confirmationDialogOkSound);
		if (unlockedFromPortal)
		{
			MagicMoment.MagicMomentComplete += HandleMagicMomentFromPortalComplete;
		}
		else
		{
			MagicMoment.MagicMomentComplete += HandleMagicMomentComplete;
		}
		_log.LogDebug("Loading magic moment scene '{0}'", _currButton.powerupData.magicMomentScene);
		Application.LoadLevelAdditiveAsync(_currButton.powerupData.magicMomentScene);
	}

	private void HandleMagicMomentComplete(object sender, EventArgs args)
	{
		MagicMoment.MagicMomentComplete -= HandleMagicMomentComplete;
		UIManager.BlockInputChanging -= HandleUIManagerBlockInputChanging;
		UIManager.instance.blockInput = false;
		callout.UpdateGraphics();
		_currButton.UpdateGraphics();
		PlayUnlockSuccessEffects();
	}

	private void HandleMagicMomentFromPortalComplete(object sender, EventArgs args)
	{
		MagicMoment.MagicMomentComplete -= HandleMagicMomentFromPortalComplete;
		UIManager.BlockInputChanging -= HandleUIManagerBlockInputChanging;
		UIManager.instance.blockInput = false;
		PlayUnlockSuccessEffects();
		if (Bedrock.isDeviceAnonymouslyLoggedOn())
		{
			ToyLinkManager.Instance.BeginToyLink(_currButton.powerupData, PortalManager.Instance.DetectedToyWebcode, LinkFromPortalSuccess);
		}
		else
		{
			ToyLinkManager.Instance.CanToyLink(_currButton.powerupData, PortalManager.Instance.DetectedToyWebcode, LinkSkylanderConfirmed);
		}
	}

	private void HandleUIManagerBlockInputChanging(object sender, UIManager.BlockInputChangingEventArgs e)
	{
		e.Cancel();
	}

	private void OnCalloutCancelBtnClick()
	{
		onCalloutBtnClick(false);
	}
}
