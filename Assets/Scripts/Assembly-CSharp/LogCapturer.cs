using System.Text;
using UnityEngine;

public class LogCapturer : SingletonMonoBehaviour
{
	private StringBuilder output = new StringBuilder();

	public static LogCapturer Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<LogCapturer>();
		}
	}

	public StringBuilder Output
	{
		get
		{
			return output;
		}
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void OnEnable()
	{
		Application.RegisterLogCallback(HandleLog);
	}

	private void OnDisable()
	{
		Application.RegisterLogCallback(null);
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		if (type != LogType.Log)
		{
			output.AppendFormat("[{0}] ", type);
		}
		output.AppendLine(logString);
		output.AppendLine(stackTrace);
		output.AppendLine();
	}
}