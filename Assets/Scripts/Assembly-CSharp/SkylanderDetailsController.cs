using System;
using System.Collections;
using UnityEngine;

public class SkylanderDetailsController : StateController
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(SkylanderDetailsController), LogLevel.Log);

	public static CharacterData SelectedCharacterData;

	public CharacterData defaultCharData;

	public CharacterUserData charUserData;

	public ElementUserData elemUserData;

	public UIButtonComposite btnSelect;

	public UIButtonComposite btnBuy;

	public UIButtonComposite btnPortalLinkToy;

	public UIButtonComposite btnCollectionVault;

	public ExpandableButtonComposite collectionVaultExpandableButton;

	public PackedSprite iconGem;

	public PackedSprite iconCollectionVault;

	public PackedSprite iconPortal;

	public Vector3 tmpScale = new Vector3(700f, 700f, 700f);

	public GameObject skylanderRoot;

	public GameObject lockedLighting;

	public GameObject unlockedLighting;

	public GameObject centerPanel;

	public GameObject rightPanel;

	public GameObject riggedSkylander;

	public Camera skylanderCamera;

	public ConfirmationDialog purchaseConfirmationDialogPrefab;

	public SoundEventData purchaseConfirmationDialogOkSound;

	public SoundEventData purchaseConfirmationDialogCancelSound;

	public SoundEventData purchaseConfirmationDialogCancelSound2;

	public GemRefundDialog gemRefundDialogPrefab;

	public Color lockedAmbient;

	public Color unlockedAmbient;

	public GameObject characterSpotlight;

	public ToyLinkScreen toyLinkScreenPrefab;

	public PortalLinkScreen portalLinkScreenPrefab;

	public float lockedRotationRate = 0.5f;

	public bool doRotation = true;

	public UIButtonComposite upgradeButton;

	public PackedSprite upgradeCoinIcon;

	public PackedSprite upgradeCheckmarkIcon;

	public SpriteText upgradeTitleText;

	public SpriteText upgradeDescriptionText;

	public SpriteText upgradePriceText;

	public SpriteText upgradeRequirementText;

	public SimpleSprite upgradeIcon;

	public PackedSprite upgradeRequirementLockIcon;

	public SoundEventData upgradePurchaseConfirmationDialogOkSound;

	public SoundEventData upgradePurchaseConfirmationDialogCancelSound;

	public SoundEventData upgradePurchaseRequirementsNotMetSound;

	public PrefabPlaceholder upgradeSaleTagPlaceholder;

	public PrefabPlaceholder unlockSaleTagPlaceholder;

	public GameObject collectionVaultAppDetails;

	private Color upgradeUnlockedPurchasedTitleColor = ColorUtils.ColorFromInt(255, 217, 4);

	private Color upgradeUnlockedNotPurchasedTitleColor = ColorUtils.ColorFromInt(164, 172, 180);

	private Color upgradeLockedTitleColor = ColorUtils.ColorFromInt(164, 172, 180);

	private Color upgradeLockedPriceColor = ColorUtils.ColorFromInt(90, 119, 129);

	private Color upgradeUnlockedPriceColor = ColorUtils.ColorFromInt(255, 217, 4);

	private bool _animIsPlaying;

	private GameObject _unlockFx;

	private int _unlockPrice = 40;

	private Vector3 _originalRequirementTextPosition;

	private SaleTag _upgradeSaleTag;

	private SaleTag _unlockSaleTag;

	private void Start()
	{
		_upgradeSaleTag = upgradeSaleTagPlaceholder.InstantiatePrefab().GetComponent<SaleTag>();
		_unlockSaleTag = unlockSaleTagPlaceholder.InstantiatePrefab().GetComponent<SaleTag>();
		_originalRequirementTextPosition = upgradeRequirementText.transform.localPosition;
	}

	private void OnEnable()
	{
		UIButtonComposite.ButtonClicking += HandleButtonClicking;
		Bedrock.UnlockContentChanged += HandleUnlockContentChanged;
	}

	private void OnDisable()
	{
		UIButtonComposite.ButtonClicking -= HandleButtonClicking;
		Bedrock.UnlockContentChanged -= HandleUnlockContentChanged;
	}

	protected override void ShowState()
	{
		base.ShowState();
		if (SelectedCharacterData == null)
		{
			SelectedCharacterData = defaultCharData;
		}
		UpdateVisuals();
		_animIsPlaying = false;
	}

	protected override void OnStateDeactivate(string oldState)
	{
		StopAllCoroutines();
		base.OnStateDeactivate(oldState);
	}

	protected override void HideState()
	{
		characterSpotlight.GetComponent<Renderer>().enabled = false;
		base.HideState();
	}

	protected override IEnumerator AnimateStateIn()
	{
		HideState();
		while (StateManager.Instance.Loading)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(1f);
		centerPanel.transform.localPosition = new Vector3(-2500f, 0f, 0f);
		rightPanel.transform.localPosition = new Vector3(2500f, 0f, 0f);
		skylanderCamera.transform.localPosition = new Vector3(-61f, 24.87f, -48.7512f);
		ShowState();
		LoadSkylander();
		DisplaySkylander();
		iTween.MoveTo(skylanderCamera.gameObject, iTween.Hash("position", new Vector3(21.5f, 23f, -48.7512f), "time", 0.5f, "islocal", true));
		yield return new WaitForSeconds(0.2f);
		UIManager.instance.blockInput = false;
		iTween.MoveTo(centerPanel, new Vector3(0f, 0f, 0f), 0.5f);
		iTween.MoveTo(rightPanel, new Vector3(0f, 0f, 0f), 0.5f);
	}

	protected override IEnumerator AnimateStateOut()
	{
		LoadingPanel loadingPanel = LoadingPanel.InstanceAutoCreate;
		loadingPanel.DismissIfNotLoading = false;
		iTween.MoveTo(centerPanel, new Vector3(-2500f, 0f, 0f), 0.5f);
		iTween.MoveTo(rightPanel, new Vector3(2500f, 0f, 0f), 0.5f);
		UIBackground.Instance.FadeTo(UIBackground.SkyTime.DAY);
		iTween.MoveTo(skylanderCamera.gameObject, iTween.Hash("position", new Vector3(-61f, 23f, -48.7512f), "time", 0.5f, "islocal", true));
		if (riggedSkylander != null)
		{
			UnityEngine.Object.Destroy(riggedSkylander);
			riggedSkylander = null;
		}
		yield return new WaitForSeconds(0.5f);
		CharacterData[] allCharacters = ElementDataManager.Instance.characterDataList.GetAllReleasedSkylanders();
		CharacterData[] array = allCharacters;
		foreach (CharacterData character in array)
		{
			if (character != StartGameSettings.Instance.activeSkylander)
			{
				character.ReleaseReferencesToResources();
			}
		}
		Resources.UnloadUnusedAssets();
		HideState();
	}

	private void Update()
	{
		if (riggedSkylander != null && doRotation)
		{
			skylanderRoot.transform.Rotate(new Vector3(0f, 0f - lockedRotationRate * Time.deltaTime, 0f));
		}
	}

	private bool IsSkylanderFulyLinked(CharacterUserData cud)
	{
		bool isToyLinked = cud.IsToyLinked;
		isToyLinked &= cud.HasToyLinkSkylanders2011 == cud.characterData.FeatureSkylanders2011;
		isToyLinked &= cud.HasToyLinkSkylanders2012 == cud.characterData.FeatureSkylanders2012;
		return isToyLinked & (cud.HasToyLinkSkylanders2012LightCore == cud.characterData.FeatureSkylanders2012LightCore);
	}

	private void UpdateVisuals()
	{
		if (ServerVariables.HideCollectionVaultFeatures)
		{
			_log.LogDebug("Hiding collection vault app details button.");
			collectionVaultAppDetails.SetActive(false);
		}
		else
		{
			_log.LogDebug("Collection vault features enabled. Not hiding.");
		}
		collectionVaultExpandableButton.CollapseImmediately();
		_unlockPrice = SelectedCharacterData.GemCost;
		btnBuy.UIButton3D.spriteText.Text = _unlockPrice.ToString();
		charUserData = ElementDataManager.Instance.GetCharacterUserData(SelectedCharacterData);
		elemUserData = ElementDataManager.Instance.GetElementUserData(SelectedCharacterData.elementData.elementType);
		base.gameObject.GetComponent<StateRoot>().SetTitle(charUserData.characterData.charName);
		ManageLighting();
		bool isToyClaimable = charUserData.IsToyClaimable;
		bool flag = true;
		btnBuy.Hide(flag);
		btnBuy.UIButton3D.controlIsEnabled = !flag;
		btnBuy.GetComponent<Collider>().enabled = !flag;
		iconGem.Hide(flag);
		btnSelect.Hide(!flag);
		btnSelect.UIButton3D.controlIsEnabled = flag;
		btnSelect.UIButton3D.GetComponent<Collider>().enabled = flag;
		if (isToyClaimable)
		{
			btnSelect.UIButton3D.spriteText.Text = LocalizationManager.Instance.GetString("GENERIC_CLAIM");
		}
		else
		{
			btnSelect.UIButton3D.spriteText.Text = LocalizationManager.Instance.GetString("SD_SELECT_BUTTON");
		}
		doRotation = true;
		UIBackground.Instance.FadeTo((!flag) ? UIBackground.SkyTime.NIGHT : UIBackground.SkyTime.DAY);
		bool flag2 = !isToyClaimable && !charUserData.IsToyLinked;
		btnCollectionVault.UIButton3D.controlIsEnabled = flag2;
		iconCollectionVault.SetColor((!flag2) ? Color.gray : Color.white);
		bool hidePortalButton = ServerVariables.HidePortalButton;
		btnPortalLinkToy.UIButton3D.controlIsEnabled = !hidePortalButton;
		iconPortal.Hide(hidePortalButton);
		UpdateUpgradeVisuals(flag);
	}

	private void UpdateUpgradeVisuals(bool skylanderIsUnlocked)
	{
		CharacterUpgradeData passiveUpgrade = charUserData.characterData.passiveUpgrade;
		bool allRequirementsMet = passiveUpgrade.AllRequirementsMet;
		bool flag = true;
		skylanderIsUnlocked = true;
		upgradeButton.IsButtonColliderEnabled = skylanderIsUnlocked && !flag;
		upgradeButton.Hide(flag);
		Color color = ((!skylanderIsUnlocked) ? upgradeLockedTitleColor : ((!flag) ? upgradeUnlockedNotPurchasedTitleColor : upgradeUnlockedPurchasedTitleColor));
		upgradeTitleText.SetColor(color);
		upgradeTitleText.Text = passiveUpgrade.LocalizedName.ToUpper();
		Color color2 = ((!skylanderIsUnlocked) ? upgradeLockedTitleColor : ((!flag) ? upgradeUnlockedNotPurchasedTitleColor : Color.white));
		upgradeDescriptionText.SetColor(color2);
		upgradeDescriptionText.Text = passiveUpgrade.LocalizedDescription.ToUpper();
		upgradeRequirementText.Hide(allRequirementsMet);
		upgradeRequirementLockIcon.Hide(allRequirementsMet);
		if (!allRequirementsMet)
		{
			string text = passiveUpgrade.requirement.NotMetText.ToUpper();
			float width = upgradeRequirementText.GetWidth(text);
			upgradeRequirementText.SetAnchor(SpriteText.Anchor_Pos.Middle_Center);
			upgradeRequirementText.transform.localPosition = _originalRequirementTextPosition + new Vector3(width / 2f, 0f, 0f);
			upgradeRequirementText.Text = text;
		}
		upgradePriceText.Hide(flag);
		upgradePriceText.SetColor((!skylanderIsUnlocked) ? upgradeLockedPriceColor : upgradeUnlockedPriceColor);
		upgradePriceText.Text = charUserData.characterData.UpgradeCost.ToString("n0");
		upgradeCheckmarkIcon.Hide(!flag);
		upgradeCoinIcon.Hide(flag);
		upgradeCoinIcon.PlayAnim((!skylanderIsUnlocked) ? "Disabled" : "Enabled");
		upgradeIcon.GetComponent<MeshRenderer>().material.mainTexture = passiveUpgrade.Icon;
		upgradeIcon.SetColor((!skylanderIsUnlocked) ? Color.gray : Color.white);
		UpdateSaleTags();
	}

	private void HandleButtonClicking(object sender, CancellableEventArgs e)
	{
		if (sender == upgradeButton && !charUserData.characterData.passiveUpgrade.AllRequirementsMet)
		{
			e.Cancel();
			SoundEventManager.Instance.Play2D(upgradePurchaseRequirementsNotMetSound);
			iTween.PunchScale(upgradeRequirementText.gameObject, Vector3.one, 0.5f);
		}
	}

	private void HandleUnlockContentChanged(object sender, EventArgs e)
	{
		if (base.IsShowing)
		{
			UpdateVisuals();
		}
	}

	private void OnLinkToyBtnClick()
	{
		UIManager.instance.blockInput = true;
		int? amountUserSpentToUnlock = charUserData.AmountUserSpentToUnlock;
		if (!amountUserSpentToUnlock.HasValue || (amountUserSpentToUnlock.HasValue && amountUserSpentToUnlock.Value <= 0))
		{
			StartCoroutine(LoadToyLinkScreen());
		}
		else if (amountUserSpentToUnlock != -1)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(gemRefundDialogPrefab.gameObject);
			GemRefundDialog component = gameObject.GetComponent<GemRefundDialog>();
			string formatString = LocalizationManager.Instance.GetFormatString("GEM_REFUND_SKYLANDER", charUserData.AmountUserSpentToUnlock);
			component.Display(formatString, GemRefundConfirm, GemRefundCancelled);
		}
	}

	private void OnLinkPortalBtnClick()
	{
		UIManager.instance.blockInput = true;
		int? amountUserSpentToUnlock = charUserData.AmountUserSpentToUnlock;
		if (!amountUserSpentToUnlock.HasValue || (amountUserSpentToUnlock.HasValue && amountUserSpentToUnlock.Value <= 0))
		{
			StartCoroutine(LoadPortalLinkScreen());
		}
		else if (amountUserSpentToUnlock != -1)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(gemRefundDialogPrefab.gameObject);
			GemRefundDialog component = gameObject.GetComponent<GemRefundDialog>();
			string formatString = LocalizationManager.Instance.GetFormatString("GEM_REFUND_SKYLANDER", charUserData.AmountUserSpentToUnlock);
			component.Display(formatString, GemRefundPortalConfirm, GemRefundCancelled);
		}
	}

	private void GemRefundCancelled()
	{
		_log.Log("User Cancelled Gem Refund");
	}

	private void GemRefundConfirm()
	{
		_log.Log("User Confirmed Gem Refund");
		UIManager.instance.blockInput = true;
		StartCoroutine(LoadToyLinkScreen());
	}

	private void GemRefundPortalConfirm()
	{
		_log.Log("User Confirmed Gem Refund");
		UIManager.instance.blockInput = true;
		StartCoroutine(LoadPortalLinkScreen());
	}

	private IEnumerator LoadToyLinkScreen()
	{
		yield return new WaitForSeconds(0.5f);
		ToyLinkScreen toyLinkScreenInstance = (ToyLinkScreen)UnityEngine.Object.Instantiate(toyLinkScreenPrefab);
		toyLinkScreenInstance.LinkableData = charUserData.characterData;
		toyLinkScreenInstance.SuccessAction = UnlockSkylanderFromToyLink;
		toyLinkScreenInstance.DismissAction = UpdateSaleTags;
		UIManager.instance.blockInput = false;
		HeaderUI.Instance.ShowSocialButtons(false);
		HideSaleTags();
	}

	private IEnumerator LoadPortalLinkScreen()
	{
		yield return new WaitForSeconds(0.5f);
		PortalLinkScreen portalLinkScreenInstance = (PortalLinkScreen)UnityEngine.Object.Instantiate(portalLinkScreenPrefab);
		portalLinkScreenInstance.LinkableData = charUserData.characterData;
		portalLinkScreenInstance.SuccessAction = UnlockSkylanderFromPortalLink;
		portalLinkScreenInstance.DismissAction = UpdateSaleTags;
		int tmpAmountToUnlock = 0;
		if (charUserData.AmountUserSpentToUnlock.HasValue)
		{
			tmpAmountToUnlock = charUserData.AmountUserSpentToUnlock.Value;
		}
		portalLinkScreenInstance.gemsToReimburse = tmpAmountToUnlock;
		UIManager.instance.blockInput = false;
		HeaderUI.Instance.ShowSocialButtons(false);
		HideSaleTags();
	}

	private void UpdateSaleTags()
	{
		_log.LogDebug("UpdateSaleTags()");
		bool flag = true;
		_unlockSaleTag.IsVisible = (!flag && SelectedCharacterData.IsUnlockOnSale) || DebugSettingsUI.forceSaleIcons;
		_unlockSaleTag.SaleText = SelectedCharacterData.UnlockSaleText;
	}

	private void HideSaleTags()
	{
		_unlockSaleTag.IsVisible = false;
		_upgradeSaleTag.IsVisible = false;
	}

	private void UnlockSkylanderFromPortalLink()
	{
		_log.LogDebug("UnlockSkylanderFromPortalLink()");
		UIManager.instance.blockInput = true;
		if (!charUserData.AmountUserSpentToUnlock.HasValue)
		{
			_log.Log("No previous value for unlock gems found. Resetting Unlock State.");
			charUserData.ResetUnlockState();
		}
		StartUnlockSequence(true);
	}

	private void UnlockSkylanderFromToyClaim()
	{
		charUserData.characterData.UnlockFromToy((uint)charUserData.characterData.SubType);
		UnlockSkylanderFromToyLink();
	}

	private void UnlockSkylanderFromToyLink()
	{
		_log.LogDebug("UnlockSkylanderFromToyLink()");
		UIManager.instance.blockInput = true;
		elemUserData.Update();
		StartUnlockSequence(false);
	}

	public void OnUnlockUpgradePressed()
	{
		Debug.Log("Unlock Ability Pressed");
	}

	private void UpgradePurchaseConfirmed()
	{
		SoundEventManager.Instance.Play2D(upgradePurchaseConfirmationDialogOkSound);
		AchievementManager.Instance.TrackCoinsSpent(charUserData.characterData.UpgradeCost);
		UpdateVisuals();
		UpdateSaleTags();
		SwrveEventsPurchase.PassiveUpgrade(charUserData.characterData.charName, charUserData.characterData.passiveUpgrade.LocalizedName, charUserData.characterData.UpgradeCost);
	}

	private void UpgradePurchaseCancelled()
	{
		SoundEventManager.Instance.Play2D(upgradePurchaseConfirmationDialogCancelSound);
		SwrveEventsPurchase.PassiveUpgradeFailed(charUserData.characterData.charName, charUserData.characterData.passiveUpgrade.LocalizedName, charUserData.characterData.UpgradeCost);
	}

	private void LinkSkylanderConfirmed()
	{
		_log.LogDebug("LinkSkylanderConfirmed()");
		SoundEventManager.Instance.Play2D(upgradePurchaseConfirmationDialogOkSound);
		int gemsToReimburse = 0;
		if (charUserData.AmountUserSpentToUnlock.HasValue)
		{
			gemsToReimburse = charUserData.AmountUserSpentToUnlock.Value;
		}
		SwrveEventsProgression.PortalLinkLinkToRegisteredYes(charUserData.characterData.charName, gemsToReimburse);
		ToyLinkManager.Instance.BeginToyLink(charUserData.characterData, PortalManager.Instance.DetectedToyWebcode, LinkFromPortalSuccess);
	}

	private void LinkFromPortalSuccess()
	{
		_log.LogDebug("LinkFromPortalSuccess()");
		elemUserData.Update();
		UpdateVisuals();
	}

	private void OnUnlockBtnClick()
	{
		ConfirmationDialog confirmationDialog = (ConfirmationDialog)UnityEngine.Object.Instantiate(purchaseConfirmationDialogPrefab);
		confirmationDialog.OnNoDismissedSound = purchaseConfirmationDialogCancelSound2;
		confirmationDialog.BlockInputOnConfirm = true;
		string formatString = LocalizationManager.Instance.GetFormatString("SD_BUY_CONFIRM", SelectedCharacterData.name, _unlockPrice);
		StartCoroutine(confirmationDialog.Display(formatString, PurchaseConfirmed, PurchaseCancelled, true));
	}

	private void PurchaseConfirmed()
	{
		UIManager.instance.blockInput = true;
		charUserData.UnlockCharacter(_unlockPrice, CharacterUserData.ToyLink.None);
		elemUserData.Update();
		SwrveEventsPurchase.Skylander(charUserData.characterData.charName, _unlockPrice);
		StartUnlockSequence(false);
	}

	private void PurchaseCancelled()
	{
		SoundEventManager.Instance.Play2D(purchaseConfirmationDialogCancelSound);
		Debug.Log("Cancelled");
		SwrveEventsPurchase.SkylanderFailed(charUserData.characterData.charName, _unlockPrice);
	}

	private void StartUnlockSequence(bool unlockFromPortal)
	{
		_log.LogDebug("StartUnlockSequence({0})", unlockFromPortal);
		SoundEventManager.Instance.Play2D(purchaseConfirmationDialogOkSound);
		StartGameSettings.Instance.activeSkylander = charUserData.characterData;
		Application.LoadLevelAdditiveAsync(charUserData.characterData.magicMomentScene);
		if (unlockFromPortal)
		{
			MagicMoment.MagicMomentComplete += HandleMagicMomentPortalUsedComplete;
		}
		else
		{
			MagicMoment.MagicMomentComplete += HandleMagicMomentComplete;
		}
		InvokeHelper.InvokeSafe(FixAmbient, 2f, this);
	}

	private void FixAmbient()
	{
		RenderSettings.ambientLight = unlockedAmbient;
	}

	private void HandleMagicMomentComplete(object sender, EventArgs args)
	{
		_log.LogDebug("HandleMagicMomentComplete()");
		MagicMoment.MagicMomentComplete -= HandleMagicMomentComplete;
		UnlockSkylander();
	}

	private void HandleMagicMomentPortalUsedComplete(object sender, EventArgs args)
	{
		_log.LogDebug("HandleMagicMomentPortalUsedComplete(...)");
		MagicMoment.MagicMomentComplete -= HandleMagicMomentPortalUsedComplete;
		UnlockSkylander();
		if (Bedrock.isDeviceAnonymouslyLoggedOn())
		{
			_log.LogDebug("User is logged on as anonymous - skipping link dialog.");
			ToyLinkManager.Instance.BeginToyLink(charUserData.characterData, PortalManager.Instance.DetectedToyWebcode, LinkFromPortalSuccess);
		}
		else
		{
			_log.LogDebug("User is logged on as registered user - showing link dialog.");
			ToyLinkManager.Instance.CanToyLink(charUserData.characterData, PortalManager.Instance.DetectedToyWebcode, LinkSkylanderConfirmed);
		}
	}

	private void HandleCollectionVaultButtonPressed()
	{
		if (Bedrock.GetApplicationInstalled(Bedrock.brBedrockApplications.BR_APPLICATION_BEDROCKSAMPLEAPP))
		{
			_log.LogDebug("Application found - attempting to launch.");
			Application.OpenURL(ServerVariables.CollectionVaultOpenUrl);
		}
		else
		{
			_log.LogDebug("Application not found - attempting to launch store.");
			Application.OpenURL(ServerVariables.CollectionVaultStoreUrl);
		}
	}

	private void OnSelectBtnClick()
	{
		SelectSkylander();
	}

	private void PlayMovie()
	{
		if (OperatorMenu.Instance.m_ShowIntroVideo)
		{
			MoviePlayer.Instance.PlayMovie(StartGameSettings.Instance.activeSkylander.movieIntro);
		}
		else
		{
			TransitionController.Instance.StartTransitionFromFrontEnd();
		}
	}

	private void SelectSkylander()
	{
		UIManager.instance.blockInput = true;
		iTween.RotateTo(skylanderRoot, iTween.Hash("y", 0f, "islocal", true, "time", 1f));
		StartGameSettings.Instance.activeSkylander = SelectedCharacterData;
		UpdateVisuals();
		AnimationClip animationClip = PlaySkylanderLevelupAnimation();
		if (animationClip != null)
		{
			InvokeHelper.InvokeSafe(PlayMovie, animationClip.length, this);
		}
		else
		{
			TransitionController.Instance.StartTransitionFromFrontEnd();
		}
	}

	private void LoadSkylander()
	{
		riggedSkylander = (GameObject)UnityEngine.Object.Instantiate(SelectedCharacterData.GetRiggedModelPrefab());
		if (riggedSkylander != null)
		{
			HideAccessoriesOnRiggedSkylander();
		}
	}

	private void DisplaySkylander()
	{
		skylanderRoot.transform.localEulerAngles = Vector3.zero;
		riggedSkylander.transform.parent = skylanderRoot.transform;
		riggedSkylander.transform.localScale = SelectedCharacterData.detailsScale;
		riggedSkylander.transform.localEulerAngles = SelectedCharacterData.detailsRotation;
		riggedSkylander.transform.localPosition = SelectedCharacterData.detailsPosition;
		AnimationUtils.PlayClip(riggedSkylander.GetComponent<Animation>(), "idle");
		ManageLighting();
	}

	private void LoadoutTransition()
	{
		_animIsPlaying = false;
		StateManager.Instance.LoadAndActivateState("Loadout");
	}

	private void UnlockSkylander()
	{
		_log.LogDebug("UnlockSkylander()");
		UpdateVisuals();
		AchievementManager.Instance.autoSync = true;
		AchievementManager.Instance.IncrementStep(Achievements.BuySkylander);
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = false;
		CharacterData[] allReleasedSkylanders = ElementDataManager.Instance.characterDataList.GetAllReleasedSkylanders();
		CharacterData[] array = allReleasedSkylanders;
		foreach (CharacterData characterData in array)
		{
			CharacterUserData characterUserData = ElementDataManager.Instance.GetCharacterUserData(characterData);
			if (characterUserData.IsUnlocked)
			{
				_log.LogDebug("Type unlocked {0}", characterData.elementData.elementType);
				switch (characterData.elementData.elementType)
				{
				case Elements.Type.Air:
					flag = true;
					break;
				case Elements.Type.Earth:
					flag4 = true;
					break;
				case Elements.Type.Fire:
					flag5 = true;
					break;
				case Elements.Type.Life:
					flag2 = true;
					break;
				case Elements.Type.Magic:
					flag7 = true;
					break;
				case Elements.Type.Tech:
					flag8 = true;
					break;
				case Elements.Type.Undead:
					flag3 = true;
					break;
				case Elements.Type.Water:
					flag6 = true;
					break;
				}
			}
		}
		if (flag && flag2 && flag3 && flag4 && flag5 && flag6 && flag7 && flag8)
		{
			AchievementManager.Instance.SetStep(Achievements.BuyAll, 1);
		}
		AchievementManager.Instance.autoSync = false;
		if (charUserData.characterData.GetUnlockEffectPrefab() != null)
		{
			ManageLighting();
			_unlockFx = (GameObject)UnityEngine.Object.Instantiate(charUserData.characterData.GetUnlockEffectPrefab());
			_unlockFx.transform.position = new Vector3(riggedSkylander.transform.position.x, riggedSkylander.transform.position.y + 5f, riggedSkylander.transform.position.z);
			SoundEventManager.Instance.PlayNoDestoryOnLoad(charUserData.characterData.elementData.LoadPurchaseSfx());
		}
		StartGameSettings.Instance.activeSkylander = charUserData.characterData;
		PlaySkylanderLevelupAnimation();
		UpdateSaleTags();
		UIManager.instance.blockInput = false;
	}

	private void UnlockedTransition()
	{
		_animIsPlaying = false;
		AnimationUtils.PlayClip(riggedSkylander.GetComponent<Animation>(), "idle");
	}

	private void OnInteractBtnClick()
	{
		PlaySkylanderLevelupAnimation();
	}

	private AnimationClip PlaySkylanderLevelupAnimation()
	{
		if (!_animIsPlaying && riggedSkylander != null)
		{
			_animIsPlaying = true;
			AnimationClip animationClip = AnimationUtils.PlayClip(riggedSkylander.GetComponent<Animation>(), "levelUp");
			if (animationClip != null)
			{
				InvokeHelper.InvokeSafe(InteractTransition, animationClip.length, this);
			}
			else
			{
				InteractTransition();
			}
			SoundEventManager.Instance.Play2D(charUserData.characterData.elementData.LoadLevelupSfx());
			return animationClip;
		}
		return null;
	}

	private void InteractTransition()
	{
		_animIsPlaying = false;
		AnimationUtils.PlayClip(riggedSkylander.GetComponent<Animation>(), "idle");
	}

	private void HideAccessoriesOnRiggedSkylander()
	{
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
	}

	private void ManageLighting()
	{
		if (StateManager.Instance.CurrentStateName == "SkylanderDetails")
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("CharacterStage");
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				gameObject.SetActive(false);
			}
			lockedLighting.SetActive(false);
			unlockedLighting.SetActive(true);
			RenderSettings.ambientLight = unlockedAmbient;
			characterSpotlight.GetComponent<Renderer>().enabled = false;
		}
	}
}
