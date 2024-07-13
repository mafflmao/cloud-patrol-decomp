public class SwrveEventsRewards
{
	public class AwardReason
	{
		public const string ElementOfTheDayBonus = "ElementOfTheDayBonus";

		public const string CollectedInFlight = "CollectedInFlight";

		public const string MagicItemRefund = "MagicItemRefund";

		public const string SkylanderRefund = "SkylanderRefund";

		public const string RankReward = "RankReward";

		public const string StartingReward = "StartingReward";
	}

	public static void AwardGems(int numGems, string reason)
	{
		if (numGems > 0)
		{
			Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("Delta", numGems.ToString(), "CurrentGemTotal", Payload.CurrentGemTotal, "AwardReason", reason);
			Bedrock.AnalyticsLogVirtualCurrencyAwarded((ulong)numGems, "Gems", parameters);
		}
	}

	public static void AwardCoins(int numCoins, string reason)
	{
		if (numCoins > 0)
		{
			Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("Delta", numCoins.ToString(), "CurrentCoinTotal", Payload.CurrentCoinTotal, "AwardReason", reason);
			Bedrock.AnalyticsLogVirtualCurrencyAwarded((ulong)numCoins, "Coins", parameters);
		}
	}
}
