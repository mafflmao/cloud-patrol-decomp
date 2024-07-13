public class SwrveEventsUtil
{
	private static readonly ILogger EventLog = LogBuilder.Instance.GetLogger(typeof(SwrveEventsUtil), LogLevel.None);

	public static bool SendSwrveMessage(string eventName, Bedrock.brKeyValueArray payload)
	{
		EventLog.Log(eventName + " message sent");
		bool result = Bedrock.AnalyticsLogEvent(eventName, payload, false);
		Payload.LastEvent = eventName;
		return result;
	}

	public static bool SendCoinPurchase(string itemName, int cost)
	{
		return Bedrock.AnalyticsLogVirtualPurchase(itemName, (ulong)cost, 1uL, "Coins");
	}

	public static bool SendCoinPurchase(string itemName, ulong cost)
	{
		return Bedrock.AnalyticsLogVirtualPurchase(itemName, cost, 1uL, "Coins");
	}

	public static bool SendGemPurchase(string itemName, int cost)
	{
		return Bedrock.AnalyticsLogVirtualPurchase(itemName, (ulong)cost, 1uL, "Gems");
	}

	public static bool SendGemPurchase(string itemName, ulong cost)
	{
		return Bedrock.AnalyticsLogVirtualPurchase(itemName, cost, 1uL, "Gems");
	}
}
