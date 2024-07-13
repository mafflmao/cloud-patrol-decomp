using UnityEngine;

public class WildcardItemStoreButton : BaseItemStoreButton
{
	public PackedSprite[] gemIcons;

	public SpriteText[] amountTexts;

	public override void UpdateAppearance()
	{
		base.UpdateAppearance();
		if (_itemData != null)
		{
			int consumablesHeld = _itemData.consumablesHeld;
			int purchaseConsumablePackCount = _itemData.purchaseConsumablePackCount;
			int maxConsumablesHeld = _itemData.maxConsumablesHeld;
			int num = Mathf.CeilToInt(consumablesHeld / purchaseConsumablePackCount) * (gemIcons.Length / (maxConsumablesHeld / purchaseConsumablePackCount));
			for (int i = num; i < gemIcons.Length; i++)
			{
				gemIcons[i].PlayAnim(1);
			}
			for (int j = 0; j < amountTexts.Length; j++)
			{
				int num2 = Mathf.Min(purchaseConsumablePackCount, consumablesHeld - j * purchaseConsumablePackCount);
				amountTexts[j].text = num2.ToString();
				amountTexts[j].Text = num2.ToString();
				amountTexts[j].Hide(num2 <= 0);
			}
		}
	}
}
