using System.Collections.Generic;

public class SwrveEconomy
{
	public enum CoinPack
	{
		Pack1 = 0,
		Pack2 = 1,
		Pack3 = 2,
		Pack4 = 3,
		None = 4
	}

	public enum GemPack
	{
		Pack1 = 0,
		Pack2 = 1,
		Pack3 = 2,
		Pack4 = 3,
		None = 4
	}

	public const string itemID = "EconomyVariables";

	private const string coin_pack1Amount = "coin_pack1Amount";

	private const string coin_pack2Amount = "coin_pack2Amount";

	private const string coin_pack3Amount = "coin_pack3Amount";

	private const string coin_pack4Amount = "coin_pack4Amount";

	private const string coin_pack1Price = "coin_pack1Price";

	private const string coin_pack2Price = "coin_pack2Price";

	private const string coin_pack3Price = "coin_pack3Price";

	private const string coin_pack4Price = "coin_pack4Price";

	private const string coin_goalSkipCoinCost = "coin_goalSkipCoinCost";

	private const string coin_comboCoin2Payout = "coin_comboCoin2Payout";

	private const string coin_comboCoin3Payout = "coin_comboCoin3Payout";

	private const string coin_comboCoin4Payout = "coin_comboCoin4Payout";

	private const string coin_comboCoin5Payout = "coin_comboCoin5Payout";

	private const string coin_comboCoinMaxPayout = "coin_comboCoinMaxPayout";

	private const string coin_startingCoins = "coin_startingCoins";

	private const string coin_minPresentCoins = "coin_minPresentCoins";

	private const string coin_maxPresentCoins = "coin_maxPresentCoins";

	private const string coin_saleTextFormatString = "coin_pack{0}SaleText";

	private const string gem_pack1Amount = "gem_pack1Amount";

	private const string gem_pack2Amount = "gem_pack2Amount";

	private const string gem_pack3Amount = "gem_pack3Amount";

	private const string gem_pack4Amount = "gem_pack4Amount";

	private const string gem_pack1Price = "gem_pack1Price";

	private const string gem_pack2Price = "gem_pack2Price";

	private const string gem_pack3Price = "gem_pack3Price";

	private const string gem_pack4Price = "gem_pack4Price";

	private const string gem_startingGems = "gem_startingGems";

	private const string gem_minPresentGems = "gem_minPresentGems";

	private const string gem_maxPresentGems = "gem_maxPresentGems";

	private const string gem_rankGemsAwarded = "gem_rankGemsAwarded";

	private const string gem_gemsCollectedPerRank = "gem_gemsCollectedPerRank";

	private const string gem_saleTextFormatString = "gem_pack{0}SaleText";

	private static Dictionary<string, string> _resourceDictionary;

	private static int _coin_pack1Price = 10;

	private static int _coin_pack2Price = 20;

	private static int _coin_pack3Price = 40;

	private static int _coin_pack4Price = 80;

	private static int _coin_pack1Amount = 5000;

	private static int _coin_pack2Amount = 15000;

	private static int _coin_pack3Amount = 40000;

	private static int _coin_pack4Amount = 120000;

	private static int _coin_goalSkipCoinCost = 500;

	private static int _coin_startingCoins;

	private static int _coin_comboCoin2Payout = 10;

	private static int _coin_comboCoin3Payout = 20;

	private static int _coin_comboCoin4Payout = 35;

	private static int _coin_comboCoin5Payout = 50;

	private static int _coin_comboCoinMaxPayout = 100;

	private static int _coin_minPresentCoins = 10;

	private static int _coin_maxPresentCoins = 25;

	private static int _coin_rocketBooster = 1000;

	private static string _coin_Pack1SaleText = "0";

	private static string _coin_Pack2SaleText = "0";

	private static string _coin_Pack3SaleText = "0";

	private static string _coin_Pack4SaleText = "0";

	private static int _gem_pack1Price = 199;

	private static int _gem_pack2Price = 499;

	private static int _gem_pack3Price = 1499;

	private static int _gem_pack4Price = 4999;

	private static int _gem_pack1Amount = 25;

	private static int _gem_pack2Amount = 75;

	private static int _gem_pack3Amount = 300;

	private static int _gem_pack4Amount = 1500;

	private static string _gem_pack1SaleText = "0";

	private static string _gem_pack2SaleText = "0";

	private static string _gem_pack3SaleText = "0";

	private static string _gem_pack4SaleText = "0";

	private static int _gem_startingGems = 25;

	private static int _gem_minPresentGems = 1;

	private static int _gem_maxPresentGems = 3;

	private static int _gem_rankGemsAwarded = 5;

	private static int _gem_gemsCollectedPerRank = 6;

	private static int _gem_skyLanderPrice = 40;

	private static int _gem_skylanderGiantPrice = 60;

	public static int GoalSkipCoinCost
	{
		get
		{
			return _coin_goalSkipCoinCost;
		}
		private set
		{
			_coin_goalSkipCoinCost = value;
		}
	}

	public static int RocketBoosterCoinCost
	{
		get
		{
			return _coin_rocketBooster;
		}
		private set
		{
			_coin_rocketBooster = value;
		}
	}

	public static int StartingCoins
	{
		get
		{
			return _coin_startingCoins;
		}
	}

	public static int StartingGems
	{
		get
		{
			return _gem_startingGems;
		}
	}

	public static int GlobalSkylanderGemCost
	{
		get
		{
			return _gem_skyLanderPrice;
		}
	}

	public static int GlobalSkylanderGiantGemCost
	{
		get
		{
			return _gem_skylanderGiantPrice;
		}
	}

	public static int GemCapForCurrentRank
	{
		get
		{
			return RankDataManager.Instance.CurrentRank.Rank.RankNumber * _gem_gemsCollectedPerRank;
		}
	}

	public static int RankGemsAwarded
	{
		get
		{
			return _gem_rankGemsAwarded;
		}
	}

	public static CoinStoreItemData GetCoinStoreItemData(CoinPack cp)
	{
		CoinStoreItemData coinStoreItemData = new CoinStoreItemData();
		switch (cp)
		{
		case CoinPack.Pack1:
			coinStoreItemData.coins = _coin_pack1Amount;
			coinStoreItemData.gemCost = _coin_pack1Price;
			coinStoreItemData.graphic = CoinStoreItemData.Graphic.Small;
			coinStoreItemData.saleText = _coin_Pack1SaleText;
			break;
		case CoinPack.Pack2:
			coinStoreItemData.coins = _coin_pack2Amount;
			coinStoreItemData.gemCost = _coin_pack2Price;
			coinStoreItemData.graphic = CoinStoreItemData.Graphic.Medium;
			coinStoreItemData.saleText = _coin_Pack2SaleText;
			break;
		case CoinPack.Pack3:
			coinStoreItemData.coins = _coin_pack3Amount;
			coinStoreItemData.gemCost = _coin_pack3Price;
			coinStoreItemData.graphic = CoinStoreItemData.Graphic.Large;
			coinStoreItemData.saleText = _coin_Pack3SaleText;
			break;
		case CoinPack.Pack4:
			coinStoreItemData.coins = _coin_pack4Amount;
			coinStoreItemData.gemCost = _coin_pack4Price;
			coinStoreItemData.graphic = CoinStoreItemData.Graphic.XLarge;
			coinStoreItemData.saleText = _coin_Pack4SaleText;
			break;
		case CoinPack.None:
			return null;
		}
		return coinStoreItemData;
	}

	public static int GetGemPackPrice(GemPack gp)
	{
		switch (gp)
		{
		case GemPack.Pack1:
			return _gem_pack1Price;
		case GemPack.Pack2:
			return _gem_pack2Price;
		case GemPack.Pack3:
			return _gem_pack3Price;
		case GemPack.Pack4:
			return _gem_pack4Price;
		default:
			return 0;
		}
	}

	public static int GetGemPackAmount(GemPack gp)
	{
		switch (gp)
		{
		case GemPack.Pack1:
			return _gem_pack1Amount;
		case GemPack.Pack2:
			return _gem_pack2Amount;
		case GemPack.Pack3:
			return _gem_pack3Amount;
		case GemPack.Pack4:
			return _gem_pack4Amount;
		default:
			return 0;
		}
	}

	public static bool GetGemPackOnSale(GemPack gp)
	{
		string gemPackOnSaleText = GetGemPackOnSaleText(gp);
		return gemPackOnSaleText != "0";
	}

	public static string GetGemPackOnSaleText(GemPack gp)
	{
		switch (gp)
		{
		case GemPack.Pack1:
			return _gem_pack1SaleText;
		case GemPack.Pack2:
			return _gem_pack2SaleText;
		case GemPack.Pack3:
			return _gem_pack3SaleText;
		case GemPack.Pack4:
			return _gem_pack4SaleText;
		default:
			return "0";
		}
	}

	public static int GetComboCoinPayout(int number)
	{
		switch (number)
		{
		case 2:
			return _coin_comboCoin2Payout;
		case 3:
			return _coin_comboCoin3Payout;
		case 4:
			return _coin_comboCoin4Payout;
		case 5:
			return _coin_comboCoin5Payout;
		case 6:
			return _coin_comboCoinMaxPayout;
		default:
			return 1;
		}
	}

	public static int GetMaxPresentCoins()
	{
		return _coin_maxPresentCoins;
	}

	public static int GetMinPresentCoins()
	{
		return _coin_minPresentCoins;
	}

	public static int GetMaxPresentGems()
	{
		return _gem_maxPresentGems;
	}

	public static int GetMinPresentGems()
	{
		return _gem_minPresentGems;
	}

	public static void UpdateAllFromSwrve()
	{
		UpdateStartingAwardsFromSwrve(true);
		UpdateCoinPacksFromSwrve(false);
		UpdateGemPacksFromSwrve(false);
		UpdateGoalSkipCoinCostFromSwrve(false);
		UpdateComboCoinPayoutsFromSwrve(false);
		UpdatePresentBoxFromSwrve(false);
		UpdateRankGemsFromSwrve(false);
		UpdateHealingElixirCosts();
		UpdateRocketBoosterCost();
		_gem_skyLanderPrice = Bedrock.GetRemoteVariableAsInt("SkylanderUnlockPrice", _gem_skyLanderPrice);
		_gem_skylanderGiantPrice = Bedrock.GetRemoteVariableAsInt("SkylanderGiantUnlockPrice", _gem_skylanderGiantPrice);
		if (_resourceDictionary != null)
		{
			_resourceDictionary.Clear();
		}
	}

	public static void UpdateHealingElixirCosts()
	{
		HealingElixir.rateOfIncreasePerRoom = Bedrock.GetRemoteVariableAsInt("HealingElixirRatePerRoom", HealingElixir.rateOfIncreasePerRoom);
		HealingElixir.baseCost = Bedrock.GetRemoteVariableAsInt("HealingElixirBaseCost", HealingElixir.baseCost);
		HealingElixir.numUses = Bedrock.GetRemoteVariableAsInt("HealingElixirNumUses", HealingElixir.numUses);
		HealingElixir.gemCost = Bedrock.GetRemoteVariableAsInt("HealingElixirGemCost", HealingElixir.gemCost);
	}

	public static void UpdateRocketBoosterCost()
	{
		Dictionary<string, string> resourceDictionary = new Dictionary<string, string>();
		if (Bedrock.GetRemoteUserResources("rocketBooster", out resourceDictionary))
		{
			_coin_rocketBooster = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "CoinCost", _coin_rocketBooster);
		}
	}

	public static void UpdateStartingAwardsFromSwrve(bool getDictionary)
	{
		if (getDictionary)
		{
			Bedrock.GetRemoteUserResources("EconomyVariables", out _resourceDictionary);
		}
		SetInt(ref _coin_startingCoins, "coin_startingCoins");
		SetInt(ref _gem_startingGems, "gem_startingGems");
	}

	public static void UpdateRankGemsFromSwrve(bool getDictionary)
	{
		if (getDictionary)
		{
			Bedrock.GetRemoteUserResources("EconomyVariables", out _resourceDictionary);
		}
		SetInt(ref _gem_rankGemsAwarded, "gem_rankGemsAwarded");
		SetInt(ref _gem_gemsCollectedPerRank, "gem_gemsCollectedPerRank");
	}

	public static void UpdateCoinPacksFromSwrve(bool getDictionary)
	{
		if (getDictionary)
		{
			Bedrock.GetRemoteUserResources("EconomyVariables", out _resourceDictionary);
		}
		SetInt(ref _coin_pack1Amount, "coin_pack1Amount");
		SetInt(ref _coin_pack2Amount, "coin_pack2Amount");
		SetInt(ref _coin_pack3Amount, "coin_pack3Amount");
		SetInt(ref _coin_pack4Amount, "coin_pack4Amount");
		SetInt(ref _coin_pack1Price, "coin_pack1Price");
		SetInt(ref _coin_pack2Price, "coin_pack2Price");
		SetInt(ref _coin_pack3Price, "coin_pack3Price");
		SetInt(ref _coin_pack4Price, "coin_pack4Price");
		SetString(ref _coin_Pack1SaleText, string.Format("coin_pack{0}SaleText", 1));
		SetString(ref _coin_Pack2SaleText, string.Format("coin_pack{0}SaleText", 2));
		SetString(ref _coin_Pack3SaleText, string.Format("coin_pack{0}SaleText", 3));
		SetString(ref _coin_Pack4SaleText, string.Format("coin_pack{0}SaleText", 4));
	}

	public static void UpdateGoalSkipCoinCostFromSwrve(bool getDictionary)
	{
		if (getDictionary)
		{
			Bedrock.GetRemoteUserResources("EconomyVariables", out _resourceDictionary);
		}
		SetInt(ref _coin_goalSkipCoinCost, "coin_goalSkipCoinCost");
	}

	public static void UpdateComboCoinPayoutsFromSwrve(bool getDictionary)
	{
		if (getDictionary)
		{
			Bedrock.GetRemoteUserResources("EconomyVariables", out _resourceDictionary);
		}
		SetInt(ref _coin_comboCoin2Payout, "coin_comboCoin2Payout");
		SetInt(ref _coin_comboCoin3Payout, "coin_comboCoin3Payout");
		SetInt(ref _coin_comboCoin4Payout, "coin_comboCoin4Payout");
		SetInt(ref _coin_comboCoin5Payout, "coin_comboCoin5Payout");
		SetInt(ref _coin_comboCoinMaxPayout, "coin_comboCoinMaxPayout");
	}

	public static void UpdateGemPacksFromSwrve(bool getDictionary)
	{
		if (getDictionary)
		{
			Bedrock.GetRemoteUserResources("EconomyVariables", out _resourceDictionary);
		}
		SetInt(ref _gem_pack1Amount, "gem_pack1Amount");
		SetInt(ref _gem_pack2Amount, "gem_pack2Amount");
		SetInt(ref _gem_pack3Amount, "gem_pack3Amount");
		SetInt(ref _gem_pack4Amount, "gem_pack4Amount");
		SetString(ref _gem_pack1SaleText, string.Format("gem_pack{0}SaleText", 1));
		SetString(ref _gem_pack2SaleText, string.Format("gem_pack{0}SaleText", 2));
		SetString(ref _gem_pack3SaleText, string.Format("gem_pack{0}SaleText", 3));
		SetString(ref _gem_pack4SaleText, string.Format("gem_pack{0}SaleText", 4));
	}

	public static void UpdatePresentBoxFromSwrve(bool getDictionary)
	{
		if (getDictionary)
		{
			Bedrock.GetRemoteUserResources("EconomyVariables", out _resourceDictionary);
		}
		SetInt(ref _coin_minPresentCoins, "coin_minPresentCoins");
		SetInt(ref _coin_maxPresentCoins, "coin_maxPresentCoins");
		SetInt(ref _gem_minPresentGems, "gem_minPresentGems");
		SetInt(ref _gem_maxPresentGems, "gem_maxPresentGems");
	}

	public static void SetInt(ref int val, string key)
	{
		if (_resourceDictionary != null)
		{
			val = Bedrock.GetFromResourceDictionaryAsInt(_resourceDictionary, key, val);
		}
	}

	public static void SetFloat(ref float val, string key)
	{
		if (_resourceDictionary != null)
		{
			val = Bedrock.GetFromResourceDictionaryAsFloat(_resourceDictionary, key, val);
		}
	}

	public static void SetString(ref string val, string key)
	{
		if (_resourceDictionary != null)
		{
			val = Bedrock.GetFromResourceDictionaryAsString(_resourceDictionary, key, val);
		}
	}
}
