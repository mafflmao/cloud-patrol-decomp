public interface ILogger
{
	LogLevel OutputLevel { get; set; }

	void LogDebug(string formatString, params object[] args);

	void Log(string formatString, params object[] args);

	void LogWarning(string formatString, params object[] args);

	void LogError(string formatString, params object[] args);
}
