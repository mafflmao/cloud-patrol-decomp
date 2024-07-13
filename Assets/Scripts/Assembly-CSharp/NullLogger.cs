public class NullLogger : ILogger
{
	public LogLevel OutputLevel { get; set; }

	public void LogDebug(string formatString, params object[] args)
	{
	}

	public void Log(string formatString, params object[] args)
	{
	}

	public void LogWarning(string formatString, params object[] args)
	{
	}

	public void LogError(string formatString, params object[] args)
	{
	}
}
