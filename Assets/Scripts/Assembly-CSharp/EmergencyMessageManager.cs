using System;
using UnityEngine;

public class EmergencyMessageManager : SingletonMonoBehaviour
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(EmergencyMessageManager), LogLevel.Debug);

	public static EmergencyMessageManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<EmergencyMessageManager>();
		}
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void OnEnable()
	{
		Bedrock.EmergencyMessageAvailable += HandleBedrockEmergencyMessageAvailable;
		Bedrock.EmergencyMessageInvalid += HandleBedrockEmergencyMessageInvalid;
	}

	public void OnDisable()
	{
		Bedrock.EmergencyMessageAvailable += HandleBedrockEmergencyMessageAvailable;
		Bedrock.EmergencyMessageInvalid += HandleBedrockEmergencyMessageInvalid;
	}

	private void HandleBedrockEmergencyMessageAvailable(object sender, EventArgs e)
	{
		_log.LogDebug("HandleBedrockEmergencyMessageAvailable");
		string message;
		if (Bedrock.TryGetEmergencyMessage(LocalizationManager.Instance.CurrentLanguageCode, out message))
		{
			_log.Log("Got error message '{0}'. Displaying notification.", message);
			NotificationPanel.Instance.Display(new ActivateNotificationPanelSettings(message, 4f));
		}
		else
		{
			_log.LogError("Failed to get emergency message!");
		}
	}

	private void HandleBedrockEmergencyMessageInvalid(object sender, EventArgs e)
	{
		_log.LogDebug("HandleBedrockEmergencyMessageAvailable.");
		string message = "Activate service has been restored. Sorry for the inconvenience.";
		NotificationPanel.Instance.Display(new ActivateNotificationPanelSettings(message, 4f));
	}
}
