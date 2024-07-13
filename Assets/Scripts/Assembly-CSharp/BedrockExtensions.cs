using System.Text;
using UnityEngine;

public static class BedrockExtensions
{
	public static void DebugPrint(this Bedrock.brChallengeInfo challengeInfo)
	{
		string format = "\t{0}: {1}";
		StringBuilder stringBuilder = new StringBuilder("Bedrock.brChallengeInfo:");
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat(format, "_challengeId", challengeInfo._challengeId).AppendLine();
		stringBuilder.AppendFormat(format, "_leaderboardId", challengeInfo._leaderboardId).AppendLine();
		stringBuilder.AppendFormat(format, "_startDate", challengeInfo._startDate).AppendLine();
		stringBuilder.AppendFormat(format, "_activeDuration", challengeInfo._activeDuration).AppendLine();
		stringBuilder.AppendFormat(format, "_inactiveDuration", challengeInfo._inactiveDuration).AppendLine();
		stringBuilder.AppendFormat(format, "_invalidDuration", challengeInfo._invalidDuration).AppendLine();
		stringBuilder.AppendFormat(format, "_numResets", challengeInfo._numResets).AppendLine();
		stringBuilder.AppendFormat(format, "_status", challengeInfo._status).AppendLine();
		stringBuilder.AppendFormat(format, "_isParticipating", challengeInfo._isParticipating).AppendLine();
		stringBuilder.AppendFormat(format, "_rawData", challengeInfo._rawData).AppendLine();
		Debug.Log(stringBuilder.ToString());
	}

	public static void DebugPrint(this Bedrock.brLeaderboardRow row)
	{
		string format = "\t{0}: {1}";
		StringBuilder stringBuilder = new StringBuilder("Bedrock.brLeaderboardRow:");
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat(format, "_leaderboardId", row._leaderboardId).AppendLine();
		stringBuilder.AppendFormat(format, "_userId", row._userId).AppendLine();
		stringBuilder.AppendFormat(format, "_writeType", row._writeType).AppendLine();
		stringBuilder.AppendFormat(format, "_rating", row._rating).AppendLine();
		stringBuilder.AppendFormat(format, "_rank", row._rank).AppendLine();
		Debug.Log(stringBuilder.ToString());
	}

	public static void DebugPrint(this Bedrock.IAPCatalogEntry entry)
	{
		string format = "    {0}: {1}";
		StringBuilder stringBuilder = new StringBuilder("Bedrock.IAPCatalogEntry:");
		stringBuilder.AppendLine();
		if (entry == null)
		{
			stringBuilder.AppendFormat(format, "Reference", "(null)").AppendLine();
		}
		else
		{
			stringBuilder.AppendFormat(format, "IAPProductCategory", entry.IAPProductCategory).AppendLine();
			stringBuilder.AppendFormat(format, "IAPProductStatus", entry.IAPProductStatus).AppendLine();
			stringBuilder.AppendFormat(format, "IAPProductVirtualCurrencyAmount", entry.IAPProductVirtualCurrencyAmount).AppendLine();
			stringBuilder.AppendFormat(format, "IAPProductRawPrice", entry.IAPProductRawPrice).AppendLine();
			stringBuilder.AppendFormat(format, "IAPProductID", entry.IAPProductID).AppendLine();
			stringBuilder.AppendFormat(format, "IAPLocalizedProductName", entry.IAPLocalizedProductName).AppendLine();
			stringBuilder.AppendFormat(format, "IAPLocalizedProductDescription", entry.IAPLocalizedProductDescription).AppendLine();
			stringBuilder.AppendFormat(format, "IAPLocalizedProductPrice", entry.IAPLocalizedProductPrice).AppendLine();
			stringBuilder.AppendFormat(format, "IAPProductCountryCode", entry.IAPProductCountryCode).AppendLine();
			stringBuilder.AppendFormat(format, "IAPProductCurrencyCode", entry.IAPProductCurrencyCode).AppendLine();
			stringBuilder.AppendFormat(format, "IAPProductVirtualCurrencyName", entry.IAPProductVirtualCurrencyName).AppendLine();
		}
		Debug.Log(stringBuilder.ToString());
	}

	public static bool IsOnline(this Bedrock.brUserConnectionStatus status)
	{
		return status == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_ONLINE || status == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_ANONYMOUSLY_ONLINE;
	}

	public static bool IsRegistered(this Bedrock.brUserConnectionStatus status)
	{
		return status == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_OFFLINE || status == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_ONLINE || status == Bedrock.brUserConnectionStatus.BR_LOGGING_IN_REGISTERED;
	}

	public static bool IsLoggingIn(this Bedrock.brUserConnectionStatus status)
	{
		return status == Bedrock.brUserConnectionStatus.BR_LOGGING_IN_ANONYMOUSLY || status == Bedrock.brUserConnectionStatus.BR_LOGGING_IN_REGISTERED;
	}

	public static bool IsLoggedIn(this Bedrock.brUserConnectionStatus status)
	{
		return status == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_OFFLINE || status == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_ONLINE || status == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_ANONYMOUSLY_OFFLINE || status == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_ANONYMOUSLY_ONLINE;
	}
}
