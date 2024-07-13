public static class LogLevelExtensions
{
	public static bool ForceAllLogsEnabled;

	public static bool ShouldLogMessage(LogLevel messageLogLevel, LogLevel outputLevel)
	{
		return ForceAllLogsEnabled || messageLogLevel >= outputLevel;
	}
}
