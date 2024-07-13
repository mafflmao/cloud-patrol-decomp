using System;
using UnityEngine;

public class SwrveEventsPurchase
{
	public static string localCurrencyCode = "USD";

	public static string paymentProvider = "Apple";

	public static void ElixirUsed()
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "CoinCost", HealingElixir.GetCoinCost().ToString(), "CurrentArea", Payload.CurrentArea, "AreasCleared", Payload.AreasCleared, "DeathType", Payload.DeathType, "DeathAI", Payload.DeathAI, "DeathScreenLocation", Payload.DeathScreenLocation, "EquipedMagicItems", Payload.EquippedMagicItems, "CurrentFlightCoinTotal", Payload.CurrentFlightCoinTotal, "LastEventSent", Payload.LastEvent, "ElixirLevel", HealingElixir.UnlockedLevel.ToString());
		SwrveEventsUtil.SendCoinPurchase("ElixirUsed", HealingElixir.GetCoinCost());
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.ElixirUsed", payload);
	}

	public static void ElixirUsedFailed()
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "CoinCost", HealingElixir.GetCoinCost().ToString(), "CurrentArea", Payload.CurrentArea, "AreasCleared", Payload.AreasCleared, "DeathType", Payload.DeathType, "DeathAI", Payload.DeathAI, "DeathScreenLocation", Payload.DeathScreenLocation, "EquipedMagicItems", Payload.EquippedMagicItems, "CurrentFlightCoinTotal", Payload.CurrentFlightCoinTotal, "LastEventSent", Payload.LastEvent, "ElixirLevel", HealingElixir.UnlockedLevel.ToString());
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.ElixirUsedFailed", payload);
	}

	public static void ElixirUsedWithGems()
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "GemCost", HealingElixir.GetGemCost().ToString(), "CurrentArea", Payload.CurrentArea, "AreasCleared", Payload.AreasCleared, "DeathType", Payload.DeathType, "DeathAI", Payload.DeathAI, "DeathScreenLocation", Payload.DeathScreenLocation, "EquipedMagicItems", Payload.EquippedMagicItems, "CurrentFlightCoinTotal", Payload.CurrentFlightCoinTotal, "LastEventSent", Payload.LastEvent, "ElixirLevel", HealingElixir.UnlockedLevel.ToString());
		SwrveEventsUtil.SendGemPurchase("ElixirUsed", HealingElixir.GetGemCost());
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.ElixirUsedWithGems", payload);
	}

	public static void ElixirUsedWithGemsFailed()
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "GemCost", HealingElixir.GetGemCost().ToString(), "CurrentArea", Payload.CurrentArea, "AreasCleared", Payload.AreasCleared, "DeathType", Payload.DeathType, "DeathAI", Payload.DeathAI, "DeathScreenLocation", Payload.DeathScreenLocation, "EquipedMagicItems", Payload.EquippedMagicItems, "CurrentFlightCoinTotal", Payload.CurrentFlightCoinTotal, "LastEventSent", Payload.LastEvent, "ElixirLevel", HealingElixir.UnlockedLevel.ToString());
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.ElixirUsedWithGemsFailed", payload);
	}

	public static void GemPackPurchased(SwrveEconomy.GemPack gemPackType, ulong cost, ulong virtualCurrencyAmount)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "GemPack", "Gem" + gemPackType, "LastEventSent", Payload.LastEvent);
		if (!Application.genuineCheckAvailable || Application.genuine)
		{
			SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.GemPack", payload);
		}
	}

	public static void GemPackFailed()
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.GemPackFailed", payload);
	}

	public static void GemPackCancelled()
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.GemPackCancelled", payload);
	}

	public static void CoinPack(int coinPackType, ulong gemCost, ulong virtualCurrencyAmount)
	{
		SwrveEconomy.CoinPack coinPack = SwrveEconomy.CoinPack.None;
		if (Enum.IsDefined(typeof(SwrveEconomy.CoinPack), coinPackType))
		{
			coinPack = (SwrveEconomy.CoinPack)coinPackType;
		}
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "GemCost", gemCost.ToString(), "CoinPack", "Coin" + coinPack, "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendGemPurchase("CoinPack" + coinPackType, gemCost);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.CoinPack", payload);
	}

	public static void CoinPackFailed(int coinPackType, ulong gemCost)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "GemCost", gemCost.ToString(), "CoinPack", coinPackType.ToString(), "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.CoinPackFailed", payload);
	}

	public static void MagicItem(PowerupData magicItem)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "GemCost", magicItem.cost.ToString(), "ChosenMagicItem", magicItem.storageKey, "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendGemPurchase(magicItem.storageKey, magicItem.cost);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.MagicItem", payload);
	}

	public static void MagicItemFailed(PowerupData magicItem)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "GemCost", magicItem.cost.ToString(), "ChosenMagicItem", magicItem.storageKey, "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.MagicItemFailed", payload);
	}

	public static void MagicItemUpgrade(PowerupData magicItem, int upgradeCost, int upgradeLevel)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "CoinCost", upgradeCost.ToString(), "ChosenMagicItem", magicItem.storageKey, "MagicItemUpgradeLevel", upgradeLevel.ToString(), "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendCoinPurchase(magicItem.storageKey, upgradeCost);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.MagicItemUpgrade", payload);
	}

	public static void MagicItemUpgradeFailed(PowerupData magicItem, int upgradeCost, int upgradeLevel)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "CoinCost", upgradeCost.ToString(), "ChosenMagicItem", magicItem.storageKey, "MagicItemUpgradeLevel", upgradeLevel.ToString(), "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.MagicItemUpgradeFailed", payload);
	}

	public static void GoalSkip(int goalID, bool goalWasNew, int coinCost)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "GoalID", goalID.ToString(), "GoalWasNew", goalWasNew.ToString(), "CoinCost", coinCost.ToString(), "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendCoinPurchase("GoalSkip", coinCost);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.GoalSkip", payload);
	}

	public static void GoalSkipFailed(int goalID, bool goalWasNew, int coinCost)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "GoalID", goalID.ToString(), "GoalWasNew", goalWasNew.ToString(), "CoinCost", coinCost.ToString(), "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.GoalSkipFailed", payload);
	}

	public static void Skylander(string chosenSkylander, int gemCost)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "GemCost", gemCost.ToString(), "ChosenSkylander", chosenSkylander, "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendGemPurchase(chosenSkylander, gemCost);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.Skylander", payload);
	}

	public static void SkylanderFailed(string chosenSkylander, int gemCost)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "GemCost", gemCost.ToString(), "ChosenSkylander", chosenSkylander, "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.SkylanderFailed", payload);
	}

	public static void ProductListRecieved()
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("Device", Payload.Device, "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.StoreProductCount", payload);
	}

	public static void PassiveUpgrade(string chosenSkylander, string upgradeName, int coinCost)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "CoinCost", coinCost.ToString(), "ChosenSkylander", chosenSkylander, "Name", upgradeName, "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendCoinPurchase(chosenSkylander, coinCost);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.PassiveUpgrade", payload);
	}

	public static void PassiveUpgradeFailed(string chosenSkylander, string upgradeName, int coinCost)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "CoinCost", coinCost.ToString(), "ChosenSkylander", chosenSkylander, "Name", upgradeName, "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.PassiveUpgradeFailed", payload);
	}

	public static void RocketBoosterUsed()
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "CoinCost", SwrveEconomy.RocketBoosterCoinCost.ToString(), "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendCoinPurchase("rocketBooster", SwrveEconomy.RocketBoosterCoinCost);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.RocketBoosterUsed", payload);
	}

	public static void RocketBoosterUsedFailed()
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "CoinCost", SwrveEconomy.RocketBoosterCoinCost.ToString(), "LastEventSent", Payload.LastEvent);
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.RocketBoosterUsedFailed", payload);
	}

	public static void MagicItemFactoryChargePurchase(string resourceKey, int cost, PowerupData.CostType costType, bool isOnSale)
	{
		Bedrock.brKeyValueArray payload = default(Bedrock.brKeyValueArray);
		switch (costType)
		{
		case PowerupData.CostType.Coins:
			payload = BedrockUtils.Hash("CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "CoinCost", cost.ToString(), "Is On Sale", (!isOnSale) ? "No" : "Yes", "LastEventSent", Payload.LastEvent);
			SwrveEventsUtil.SendCoinPurchase(resourceKey, cost);
			break;
		case PowerupData.CostType.Gems:
			payload = BedrockUtils.Hash("CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "GemCost", cost.ToString(), "Is On Sale", (!isOnSale) ? "No" : "Yes", "LastEventSent", Payload.LastEvent);
			SwrveEventsUtil.SendGemPurchase(resourceKey, cost);
			break;
		}
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.MagicItemFactoryCharge", payload);
	}

	public static void MagicItemFactoryChargePurchaseFailed(int cost, PowerupData.CostType costType, bool isOnSale)
	{
		Bedrock.brKeyValueArray payload = default(Bedrock.brKeyValueArray);
		switch (costType)
		{
		case PowerupData.CostType.Coins:
			payload = BedrockUtils.Hash("CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "CoinCost", cost.ToString(), "Is On Sale", (!isOnSale) ? "No" : "Yes", "LastEventSent", Payload.LastEvent);
			break;
		case PowerupData.CostType.Gems:
			payload = BedrockUtils.Hash("CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "GemCost", cost.ToString(), "Is On Sale", (!isOnSale) ? "No" : "Yes", "LastEventSent", Payload.LastEvent);
			break;
		}
		SwrveEventsUtil.SendSwrveMessage("Progression.Purchase.MagicItemFactoryChargeFailed", payload);
	}
}
