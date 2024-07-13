using UnityEngine;

public class SaleTag : MonoBehaviour
{
	public enum SaleTagType
	{
		All = 0,
		Skylander = 1,
		MagicItem = 2,
		Custom = 3,
		Gems = 4,
		Coins = 5
	}

	public SimpleSprite background;

	public SpriteText text;

	public SaleTagType type = SaleTagType.Custom;

	public PowerupList powerupList;

	public CharacterDataList characterDataList;

	private bool _isVisible = true;

	public bool IsVisible
	{
		get
		{
			return _isVisible;
		}
		set
		{
			_isVisible = value;
			background.Hide(!_isVisible);
			text.Hide(!_isVisible);
		}
	}

	public string SaleText
	{
		get
		{
			return text.Text;
		}
		set
		{
			if (string.IsNullOrEmpty(value) || value == "0")
			{
				value = "SALETAG_SALE";
			}
			text.Text = LocalizationManager.Instance.GetString(value);
		}
	}

	public bool AnySkylandersOnSale
	{
		get
		{
			return false;
		}
	}

	public bool AnyMagicItemsOnSale
	{
		get
		{
			if (powerupList != null)
			{
				foreach (PowerupData powerup in powerupList.powerups)
				{
					if ((powerup.isUnlockOnSale && powerup.IsLocked) || (powerup.isUpgradeOnSale && !powerup.IsAtMaxLevel))
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public bool AnyCoinPackOnSale
	{
		get
		{
			return SwrveEconomy.GetCoinStoreItemData(SwrveEconomy.CoinPack.Pack1).IsOnSale || SwrveEconomy.GetCoinStoreItemData(SwrveEconomy.CoinPack.Pack2).IsOnSale || SwrveEconomy.GetCoinStoreItemData(SwrveEconomy.CoinPack.Pack3).IsOnSale || SwrveEconomy.GetCoinStoreItemData(SwrveEconomy.CoinPack.Pack4).IsOnSale;
		}
	}

	public bool AnyGemPacksOnSale
	{
		get
		{
			return SwrveEconomy.GetGemPackOnSale(SwrveEconomy.GemPack.Pack1) || SwrveEconomy.GetGemPackOnSale(SwrveEconomy.GemPack.Pack2) || SwrveEconomy.GetGemPackOnSale(SwrveEconomy.GemPack.Pack3) || SwrveEconomy.GetGemPackOnSale(SwrveEconomy.GemPack.Pack4);
		}
	}

	public void UpdateVisibility()
	{
		bool anySkylandersOnSale = AnySkylandersOnSale;
		bool anyMagicItemsOnSale = AnyMagicItemsOnSale;
		bool anyGemPacksOnSale = AnyGemPacksOnSale;
		bool anyCoinPackOnSale = AnyCoinPackOnSale;
		switch (type)
		{
		case SaleTagType.All:
			IsVisible = anySkylandersOnSale || anyMagicItemsOnSale || anyCoinPackOnSale || anyGemPacksOnSale;
			break;
		case SaleTagType.MagicItem:
			IsVisible = anyMagicItemsOnSale;
			break;
		case SaleTagType.Skylander:
			IsVisible = anySkylandersOnSale;
			break;
		case SaleTagType.Custom:
			break;
		case SaleTagType.Coins:
			IsVisible = anyCoinPackOnSale;
			break;
		case SaleTagType.Gems:
			IsVisible = anyGemPacksOnSale;
			break;
		}
	}
}
