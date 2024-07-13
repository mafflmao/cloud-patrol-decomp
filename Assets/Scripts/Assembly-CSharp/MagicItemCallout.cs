using System;
using UnityEngine;

[RequireComponent(typeof(Callout))]
public class MagicItemCallout : MonoBehaviour
{
	[HideInInspector]
	public Callout callout;

	[HideInInspector]
	public PowerupData powerupData;

	public SpriteText title;

	public SpriteText description;

	public SpriteText upgrade;

	public GameObject powerupButtonsParent;

	public UIButtonComposite btnUnlock;

	public UIButtonComposite btnUpgrade;

	public GameObject consumableButtonsParent;

	public UIButtonComposite btnConsumableGems;

	public UIButtonComposite btnConsumableCoins;

	public PackedSprite consumableCoinIcon;

	public PackedSprite consumableGemIcon;

	public PackedSprite iconCoin;

	public PackedSprite iconGem;

	public UIButtonComposite collectionVaultButton;

	public PackedSprite collectionVaultIcon;

	public UIButtonComposite linkPortalButton;

	public PackedSprite iconPortal;

	private void Awake()
	{
		callout = GetComponent<Callout>();
	}

	private void OnEnable()
	{
		Bedrock.UnlockContentChanged += HandleUnlockContentChanged;
	}

	private void OnDisable()
	{
		Bedrock.UnlockContentChanged -= HandleUnlockContentChanged;
	}

	private void HandleUnlockContentChanged(object sender, EventArgs e)
	{
		UpdateGraphics();
	}

	public void Show()
	{
		callout.Show();
		UpdateGraphics();
	}

	public void Hide()
	{
		if (callout.isShowing)
		{
			callout.Hide();
		}
	}

	public void UpdateGraphics()
	{
		if (!(powerupData != null))
		{
			return;
		}
		bool isLocked = powerupData.IsLocked;
		bool isToyClaimable = powerupData.IsToyClaimable;
		string localizedName = powerupData.LocalizedName;
		string text = powerupData.Description;
		string upgradeDescription = powerupData.UpgradeDescription;
		title.text = localizedName;
		title.Text = localizedName;
		description.text = text;
		description.Text = text;
		description.UpdateMesh();
		Debug.Log("Setting upgrade text to " + upgradeDescription);
		upgrade.text = upgradeDescription;
		upgrade.Text = upgradeDescription;
		bool isLinkable = powerupData.IsLinkable;
		bool flag = powerupData.IsPurchasable && !isToyClaimable;
		bool flag2 = powerupData.IsPurchaseAvailable && !isToyClaimable;
		int purchaseCost = powerupData.PurchaseCost;
		PowerupData.CostType purchaseCostType = powerupData.PurchaseCostType;
		string text2 = purchaseCost.ToString("n0");
		string text3 = powerupData.AltPurchaseCost.ToString("n0");
		if (powerupData.Type == PowerupData.ItemType.Powerup && powerupData.IsAtMaxLevel)
		{
			text2 = string.Format("  {0}", LocalizationManager.Instance.GetString("MAGIC_ITEM_UPGRADE_BUTTON_MAX"));
		}
		else if (powerupData.Type == PowerupData.ItemType.Consumable)
		{
			if (powerupData.consumablesHeld + powerupData.purchaseConsumablePackCount > powerupData.maxConsumablesHeld)
			{
				upgrade.Text = LocalizationManager.Instance.GetString("MAGIC_ITEM_CONSUMABLE_MAXED");
			}
			else if (powerupData.purchaseConsumablePackCount <= 1)
			{
			}
		}
		if (powerupData.Type == PowerupData.ItemType.Powerup)
		{
			powerupButtonsParent.SetActive(true);
			consumableButtonsParent.SetActive(false);
			btnUnlock.UIButton3D.controlIsEnabled = isLocked || isToyClaimable;
			btnUnlock.GetComponent<Collider>().enabled = isLocked || isToyClaimable;
			btnUnlock.Hide(!isLocked && !isToyClaimable);
			if (isToyClaimable)
			{
				btnUnlock.UIButton3D.Text = LocalizationManager.Instance.GetString("GENERIC_CLAIM");
			}
			else
			{
				btnUnlock.UIButton3D.Text = LocalizationManager.Instance.GetString("MAGIC_ITEM_UNLOCK");
			}
			upgrade.Hide(isLocked || isToyClaimable);
			btnUpgrade.Hide(!flag);
			btnUpgrade.UIButton3D.controlIsEnabled = flag2;
			btnUpgrade.GetComponent<Collider>().enabled = flag2;
			btnUpgrade.SetColor((!flag2) ? UIButtonComposite.DisabledColor : UIButtonComposite.EnabledColor);
			btnUpgrade.UIButton3D.spriteText.SetColor((!flag2) ? Color.gray : Color.white);
			btnUpgrade.UIButton3D.Text = text2;
			btnUpgrade.UIButton3D.controlIsEnabled = flag2;
			iconCoin.Hide(!flag2 || purchaseCostType != PowerupData.CostType.Coins);
			iconCoin.SetColor((!flag2) ? Color.grey : Color.white);
			iconGem.Hide(!flag2 || purchaseCostType != PowerupData.CostType.Gems);
			iconGem.SetColor((!flag2) ? Color.grey : Color.white);
			collectionVaultButton.UIButton3D.controlIsEnabled = isLinkable;
			collectionVaultIcon.SetColor((!isLinkable) ? Color.gray : Color.white);
			if (ServerVariables.HidePortalButton)
			{
				linkPortalButton.Hide(true);
			}
			bool hidePortalButton = ServerVariables.HidePortalButton;
			linkPortalButton.UIButton3D.controlIsEnabled = !hidePortalButton;
			iconPortal.Hide(hidePortalButton);
		}
		else
		{
			upgrade.Hide(isLocked);
			powerupButtonsParent.SetActive(false);
			consumableButtonsParent.SetActive(true);
			btnConsumableCoins.UIButton3D.controlIsEnabled = flag2;
			btnConsumableGems.UIButton3D.Text = text2;
			btnConsumableCoins.UIButton3D.Text = text3;
			btnConsumableGems.UIButton3D.controlIsEnabled = flag2;
			if (flag2)
			{
				btnConsumableCoins.UIButton3D.spriteText.SetColor(Color.white);
				btnConsumableGems.UIButton3D.spriteText.SetColor(Color.white);
				consumableCoinIcon.Color = Color.white;
				consumableGemIcon.Color = Color.white;
			}
			else
			{
				btnConsumableCoins.UIButton3D.spriteText.SetColor(Color.gray);
				btnConsumableGems.UIButton3D.spriteText.SetColor(Color.gray);
				consumableCoinIcon.Color = Color.gray;
				consumableGemIcon.Color = Color.gray;
			}
		}
	}
}
