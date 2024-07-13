using UnityEngine;

public class ConsumableItemStoreButton : BaseItemStoreButton
{
	public SpriteText countText;

	public override void UpdateAppearance()
	{
		base.UpdateAppearance();
		if (_itemData != null)
		{
			int consumablesHeld = _itemData.consumablesHeld;
			countText.Hide(_itemData.IsLocked);
			countText.Color = Color.white;
			if (consumablesHeld == 0)
			{
				countText.Text = LocalizationManager.Instance.GetString("MAGIC_ITEM_EMPTY_CONSUMABLE");
				countText.Color = Color.gray;
			}
			else if (consumablesHeld < 999)
			{
				countText.text = LocalizationManager.Instance.GetFormatString("MAGIC_ITEM_USES_FOR_CONSUMABLES", consumablesHeld.ToString("n0"));
			}
			else
			{
				countText.Text = "999+";
			}
			if (_itemData.isUnlockOnSale)
			{
				saleTag.IsVisible = true;
				saleTag.SaleText = _itemData.unlockSaleText;
			}
			else if (_itemData.isUpgradeOnSale && !_itemData.IsAtMaxLevel)
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
