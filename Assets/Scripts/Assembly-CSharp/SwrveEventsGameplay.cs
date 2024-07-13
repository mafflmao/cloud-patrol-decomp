public class SwrveEventsGameplay
{
	public static void GameEnd()
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "CurrentArea", Payload.CurrentArea, "CurrentFlightCoinTotal", Payload.CurrentFlightCoinTotal, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "DeathAI", Payload.DeathAI, "DeathType", Payload.DeathType, "DeathScreenLocation", Payload.DeathScreenLocation, "SingleCoinsSpawned", Payload.SingleCoinsSpawned, "SingleCoinsCollected", Payload.SingleCoinsCollected, "Combo2CoinsSpawned", Payload.Combo2CoinsSpawned, "Combo3CoinsSpawned", Payload.Combo3CoinsSpawned, "Combo4CoinsSpawned", Payload.Combo4CoinsSpawned, "Combo5CoinsSpawned", Payload.Combo5CoinsSpawned, "Combo6CoinsSpawned", Payload.Combo6CoinsSpawned, "Combo2CoinsCollected", Payload.Combo2CoinsCollected, "Combo3CoinsCollected", Payload.Combo3CoinsCollected, "Combo4CoinsCollected", Payload.Combo4CoinsCollected, "Combo5CoinsCollected", Payload.Combo5CoinsCollected, "Combo6CoinsCollected", Payload.Combo6CoinsCollected, "TotalAreaSkipsUsed", Payload.TotalAreaSkipsUsed, "TotalElixirsUsed", Payload.TotalElixirUsed, "AreasCleared", Payload.AreasCleared, "GlobalDifficulty", Payload.GlobalDifficulty);
		SwrveEventsUtil.SendSwrveMessage("Gameplay.Game.GameEnded", payload);
	}

	public static void MagicItemMissed(string flyingMagicItem)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "EquipedMagicItems", Payload.EquippedMagicItems, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "CurrentFlightCoinTotal", Payload.CurrentFlightCoinTotal, "CurrentArea", Payload.CurrentArea, "AreasCleared", Payload.AreasCleared, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "FlyingMagicItem", flyingMagicItem);
		SwrveEventsUtil.SendSwrveMessage("Gameplay.Game.MagicItemMissed", payload);
	}

	public static void MagicItemCollected(string flyingMagicItem)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "EquipedMagicItems", Payload.EquippedMagicItems, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "CurrentFlightCoinTotal", Payload.CurrentFlightCoinTotal, "CurrentArea", Payload.CurrentArea, "AreasCleared", Payload.AreasCleared, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "FlyingMagicItem", flyingMagicItem);
		SwrveEventsUtil.SendSwrveMessage("Gameplay.Game.MagicItemCollected", payload);
	}

	public static void MagicItemActivated(string magicItemName, int magicItemLevel)
	{
		SwrveEventsUtil.SendSwrveMessage("Gameplay.Game.MagicItemActivated", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActivatedMagicItem", magicItemName, "EquipedMagicItems", Payload.EquippedMagicItems, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "CurrentFlightCoinTotal", Payload.CurrentFlightCoinTotal, "CurrentArea", Payload.CurrentArea, "AreasCleared", Payload.AreasCleared, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "MagicItemLevel", magicItemLevel.ToString(), "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal));
	}

	public static void MagicItemWildcardTriggered(string magicItemName)
	{
		SwrveEventsUtil.SendSwrveMessage("Gameplay.Game.MagicItemWildcardTriggered", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "AcquiredWildcardMagicItem", magicItemName, "EquipedMagicItems", Payload.EquippedMagicItems, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "CurrentFlightCoinTotal", Payload.CurrentFlightCoinTotal, "CurrentArea", Payload.CurrentArea, "AreasCleared", Payload.AreasCleared, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal));
	}

	public static void GamePaused()
	{
		SwrveEventsUtil.SendSwrveMessage("Gameplay.Game.GamePaused", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "EquipedMagicItems", Payload.EquippedMagicItems, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "CurrentFlightCoinTotal", Payload.CurrentFlightCoinTotal, "CurrentArea", Payload.CurrentArea, "AreasCleared", Payload.AreasCleared, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal));
	}

	public static void PresentMissed()
	{
		SwrveEventsUtil.SendSwrveMessage("Gameplay.Game.PresentMissed", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "EquipedMagicItems", Payload.EquippedMagicItems, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "CurrentFlightCoinTotal", Payload.CurrentFlightCoinTotal, "CurrentArea", Payload.CurrentArea, "AreasCleared", Payload.AreasCleared, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal));
	}

	public static void PresentCollected(string presentType)
	{
		SwrveEventsUtil.SendSwrveMessage("Gameplay.Game.PresentCollected", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "PresentType", presentType, "EquipedMagicItems", Payload.EquippedMagicItems, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "CurrentFlightCoinTotal", Payload.CurrentFlightCoinTotal, "CurrentArea", Payload.CurrentArea, "AreasCleared", Payload.AreasCleared, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal));
	}
}
