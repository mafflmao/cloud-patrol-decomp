using System;
using System.Text;
using UnityEngine;

public static class TimeUtils
{
	public static readonly long UnixEpochTicks = new DateTime(1970, 1, 1).Ticks;

	public static readonly DateTime UnixEpoch = new DateTime(UnixEpochTicks, DateTimeKind.Utc);

	public static DateTime GetDateTimeFromUnixUtcTime(uint secondsSinceUnixEpoch)
	{
		long ticks = UnixEpochTicks + (long)secondsSinceUnixEpoch * 10000000L;
		return new DateTime(ticks, DateTimeKind.Utc);
	}

	public static uint GetSecondsSinceUnixEpoch(DateTime dateTime)
	{
		DateTime dateTime2 = dateTime.ToUniversalTime();
		double num = Math.Round((dateTime2 - UnixEpoch).TotalSeconds);
		if (num < 0.0)
		{
			Debug.LogError(string.Concat("Cannot convert time '", dateTime, "' that was before unix epoch!"));
			return 0u;
		}
		return Convert.ToUInt32(num);
	}

	public static string GetFuzzyTimeStringFromSeconds(uint seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		if (timeSpan.TotalDays >= 1.0)
		{
			string @string = LocalizationManager.Instance.GetString((timeSpan.Days != 1) ? "TIME_DAYS" : "TIME_DAY");
			return string.Format("{0} {1}", timeSpan.Days, @string);
		}
		if (timeSpan.TotalHours > 1.0)
		{
			double num = Math.Ceiling(timeSpan.TotalHours);
			return string.Format("{0} {1}", num, LocalizationManager.Instance.GetString("TIME_HOURS"));
		}
		if (timeSpan.TotalMinutes > 1.0)
		{
			double num2 = Math.Ceiling(timeSpan.TotalMinutes);
			return string.Format("{0} {1}", num2, LocalizationManager.Instance.GetString("TIME_MINUTES"));
		}
		if (timeSpan.Seconds > 1)
		{
			return string.Format("{0} {1}", timeSpan.Seconds, LocalizationManager.Instance.GetString("TIME_SECONDS"));
		}
		return LocalizationManager.Instance.GetString("TIME_NOW");
	}

	public static string GetShortFuzzyTimeStringFromSeconds(uint seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		if (timeSpan.TotalDays >= 1.0)
		{
			return string.Format("{0}d", timeSpan.Days);
		}
		if (timeSpan.TotalHours > 1.0)
		{
			double num = Math.Ceiling(timeSpan.TotalHours);
			return string.Format("{0}{1}", num, LocalizationManager.Instance.GetString("TIME_DAY_ABBREV"));
		}
		if (timeSpan.TotalMinutes > 1.0)
		{
			double num2 = Math.Ceiling(timeSpan.TotalMinutes);
			return string.Format("{0}{1}", num2, LocalizationManager.Instance.GetString("TIME_MINUTE_ABBREV"));
		}
		return string.Format("{0}{1}", timeSpan.Seconds, LocalizationManager.Instance.GetString("TIME_SECOND_ABBREV"));
	}

	public static string GetLongTimeStringFromSeconds(long seconds)
	{
		StringBuilder stringBuilder = new StringBuilder();
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		AppendLocalizedTimeText(stringBuilder, "TIME_DAY", "TIME_DAYS", timeSpan.Days);
		AppendLocalizedTimeText(stringBuilder, "TIME_HOUR", "TIME_HOURS", timeSpan.Hours);
		AppendLocalizedTimeText(stringBuilder, "TIME_MINUTE", "TIME_MINUTES", timeSpan.Minutes);
		AppendLocalizedTimeText(stringBuilder, "TIME_SECOND", "TIME_SECONDS", timeSpan.Seconds);
		return stringBuilder.ToString();
	}

	private static void AppendLocalizedTimeText(StringBuilder buffer, string singularIdentifier, string pluralIdentifier, int wholeAmount)
	{
		if (wholeAmount > 1)
		{
			if (buffer.Length > 0)
			{
				buffer.Append(" ");
			}
			buffer.AppendFormat("{0} {1}", wholeAmount, LocalizationManager.Instance.GetString(pluralIdentifier));
		}
		else if (wholeAmount == 1)
		{
			if (buffer.Length > 0)
			{
				buffer.Append(" ");
			}
			buffer.AppendFormat("{0} {1}", wholeAmount, LocalizationManager.Instance.GetString(singularIdentifier));
		}
	}

	public static string GetShortTimeStringFromSeconds(long seconds)
	{
		StringBuilder stringBuilder = new StringBuilder();
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		if (timeSpan.TotalDays >= 1.0)
		{
			return stringBuilder.Append("> ").Append(timeSpan.Days).Append(LocalizationManager.Instance.GetString("TIME_DAY_ABBREV"))
				.ToString();
		}
		if (timeSpan.TotalHours >= 1.0)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(timeSpan.Hours).Append(LocalizationManager.Instance.GetString("TIME_HOUR_ABBREV"));
		}
		if (timeSpan.TotalMinutes >= 1.0)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(timeSpan.Minutes).Append(LocalizationManager.Instance.GetString("TIME_MINUTE_ABBREV"));
		}
		if (timeSpan.Seconds > 0)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(timeSpan.Seconds).Append(LocalizationManager.Instance.GetString("TIME_SECOND_ABBREV"));
		}
		return stringBuilder.ToString();
	}

	public static uint GetSecondsSince(uint startTime, uint currentTime)
	{
		return (currentTime > startTime) ? (currentTime - startTime) : 0u;
	}
}
