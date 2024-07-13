using UnityEngine;

public class StringNotificationPanelSettings : NotificationPanel.Settings
{
	public string Message { get; private set; }

	public StringNotificationPanel.Size Size { get; set; }

	public StringNotificationPanelSettings(string message)
	{
		Message = message;
	}

	public override GameObject CreateInstanceForDisplay()
	{
		StringNotificationPanel stringNotificationPanel = StringNotificationPanel.Instance;
		if (stringNotificationPanel == null)
		{
			stringNotificationPanel = StringNotificationPanel.GetOrCreateSingletonInstance();
		}
		else
		{
			stringNotificationPanel.SetHidden(false);
		}
		stringNotificationPanel.SetString(Message);
		stringNotificationPanel.SetSize(Size);
		return stringNotificationPanel.gameObject;
	}

	public override void DestroyInstance()
	{
		StringNotificationPanel instance = StringNotificationPanel.Instance;
		if (instance != null)
		{
			instance.SetHidden(true);
		}
	}

	public override void SetHidden(bool isHidden)
	{
		StringNotificationPanel instance = StringNotificationPanel.Instance;
		if (instance != null)
		{
			instance.SetHidden(isHidden);
		}
	}

	public static StringNotificationPanelSettings BuildDismissOnRoomTransition(string message)
	{
		StringNotificationPanelSettings stringNotificationPanelSettings = new StringNotificationPanelSettings(message);
		stringNotificationPanelSettings.PrefabToAddToNotifyPanel = null;
		stringNotificationPanelSettings.RestTime = -1f;
		stringNotificationPanelSettings.DismissOnRoomTransition = true;
		stringNotificationPanelSettings.VerticalOffset = -125f;
		return stringNotificationPanelSettings;
	}

	public static StringNotificationPanelSettings BuildDismissAfterTime(string message, float restTime)
	{
		StringNotificationPanelSettings stringNotificationPanelSettings = new StringNotificationPanelSettings(message);
		stringNotificationPanelSettings.PrefabToAddToNotifyPanel = null;
		stringNotificationPanelSettings.RestTime = restTime;
		stringNotificationPanelSettings.VerticalOffset = -125f;
		return stringNotificationPanelSettings;
	}
}
