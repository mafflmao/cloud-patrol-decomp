using UnityEngine;

public class UnityDebugLogger : ILogger
{
	private string _identifier;

	public LogLevel OutputLevel { get; set; }

	public UnityDebugLogger(string identifier, LogLevel outputLevel)
	{
		_identifier = identifier;
		OutputLevel = outputLevel;
	}

	public void LogDebug(string formatString, params object[] args)
	{
		if (LogLevelExtensions.ShouldLogMessage(LogLevel.Debug, OutputLevel))
		{
			Debug.Log("[" + _identifier + "]: " + string.Format(formatString, args));
		}
	}

	public void Log(string formatString, params object[] args)
	{
		if (LogLevelExtensions.ShouldLogMessage(LogLevel.Log, OutputLevel))
		{
			Debug.Log("[" + _identifier + "]: " + string.Format(formatString, args));
		}
	}

	public void LogWarning(string formatString, params object[] args)
	{
		if (LogLevelExtensions.ShouldLogMessage(LogLevel.Warning, OutputLevel))
		{
			Debug.LogWarning("[" + _identifier + "]: " + string.Format(formatString, args));
		}
	}

	public void LogError(string formatString, params object[] args)
	{
		if (LogLevelExtensions.ShouldLogMessage(LogLevel.Error, OutputLevel))
		{
			Debug.LogError("[" + _identifier + "]: " + string.Format(formatString, args));
		}
	}
}
