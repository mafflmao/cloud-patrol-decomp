using UnityEngine;

public class PowerupItemStoreButton : BaseItemStoreButton
{
	public PackedSprite toyLinkIcon;

	public PackedSprite purchaseNotifyBackground;

	public SpriteText purchaseNotifyText;

	public PackedSprite purchaseCostBackground;

	public PackedSprite purchaseCostCoinIcon;

	public PackedSprite purchaseCostGemIcon;

	public SpriteText purchaseCostText;

	public SpriteText claimableText;

	public PackedSprite levelBar;

	public override void UpdateAppearance()
	{
		base.UpdateAppearance();
		if (!(_itemData != null))
		{
			return;
		}
		bool flag = base.powerupData.IsLocked;
		bool isToyLinked = base.powerupData.IsToyLinked;
		bool isToyClaimable = base.powerupData.IsToyClaimable;
		bool flag2 = base.powerupData.IsPurchaseAvailable && !isToyClaimable;
		int purchaseCost = base.powerupData.PurchaseCost;
		PowerupData.CostType purchaseCostType = base.powerupData.PurchaseCostType;
		toyLinkIcon.Hide(flag);
		claimableText.Hide(!isToyClaimable);
		if (!flag)
		{
			toyLinkIcon.PlayAnim((!isToyLinked) ? "Unlinked" : "Linked");
		}
		purchaseNotifyBackground.Hide(!flag2);
		purchaseNotifyText.Hide(!flag2);
		purchaseCostBackground.Hide(!flag2);
		purchaseCostCoinIcon.Hide(!flag2 || purchaseCostType != PowerupData.CostType.Coins);
		purchaseCostGemIcon.Hide(!flag2 || purchaseCostType != PowerupData.CostType.Gems);
		purchaseCostText.Hide(!flag2);
		purchaseCostText.text = purchaseCost.ToString("n0");
		purchaseCostText.Text = purchaseCost.ToString("n0");
		levelBar.Hide(flag || !base.powerupData.canUpgrade || isToyClaimable);
		levelBar.PlayAnim(Mathf.Clamp(_itemData.GetLevel() - 1, 0, 5));
		if (_itemData.Type == PowerupData.ItemType.Powerup)
		{
			if (_itemData.IsLocked && _itemData.isUnlockOnSale)
			{
				saleTag.SaleText = _itemData.unlockSaleText;
			}
			else if (!_itemData.IsLocked && !_itemData.IsAtMaxLevel && _itemData.isUpgradeOnSale)
			{
				saleTag.SaleText = _itemData.upgradeSaleText;
			}
			else
			{
				saleTag.SaleText = null;
			}
		}
		else if (_itemData.Type == PowerupData.ItemType.Consumable)
		{
			if (_itemData.isUnlockOnSale || (_itemData.isUpgradeOnSale && !_itemData.IsAtMaxLevel))
			{
				saleTag.IsVisible = true;
				saleTag.SaleText = _itemData.upgradeSaleText;
			}
			else
			{
				saleTag.IsVisible = false;
			}
		}
	}
}
