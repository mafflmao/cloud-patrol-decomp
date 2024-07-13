using System;
using System.Collections.Generic;

public class LogBuilder
{
	private static readonly ILogger NullLogger = new NullLogger();

	private static LogBuilder _instance;

	private Dictionary<string, ILogger> _customLoggers = new Dictionary<string, ILogger>();

	private static bool AllowLogging
	{
		get
		{
			return true;
		}
	}

	public static LogBuilder Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new LogBuilder();
			}
			return _instance;
		}
	}

	private LogBuilder()
	{
	}

	public ILogger GetLogger(Type loggingClass, LogLevel outputLevel)
	{
		return GetCustomLogger(loggingClass.Name, outputLevel);
	}

	public ILogger GetCustomLogger(string identifier, LogLevel outputLevel)
	{
		if (outputLevel == LogLevel.None)
		{
			return NullLogger;
		}
		ILogger value;
		if (!_customLoggers.TryGetValue(identifier, out value))
		{
			value = new UnityDebugLogger(identifier, outputLevel);
			_customLoggers.Add(identifier, value);
		}
		return value;
	}
}
