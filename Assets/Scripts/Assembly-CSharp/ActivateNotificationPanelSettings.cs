using UnityEngine;

public class ActivateNotificationPanelSettings : NotificationPanel.Settings
{
	public string Message { get; protected set; }

	public ActivateNotificationPanelSettings(string message, float restTime)
	{
		Message = message;
		base.CanBeEnqueued = true;
		base.RestTime = restTime;
		base.DismissOnRoomTransition = false;
		base.SuppressPreviouslyQueued = true;
	}

	public override GameObject CreateInstanceForDisplay()
	{
		ActivateNotificationPanel activateNotificationPanel = ActivateNotificationPanel.Instance;
		if (activateNotificationPanel == null)
		{
			activateNotificationPanel = ActivateNotificationPanel.GetOrCreateInstance();
		}
		else
		{
			activateNotificationPanel.SetHidden(false);
		}
		activateNotificationPanel.Message = Message;
		return activateNotificationPanel.gameObject;
	}

	public override void DestroyInstance()
	{
		SetHidden(true);
	}

	public override void SetHidden(bool isHidden)
	{
		ActivateNotificationPanel.Instance.SetHidden(isHidden);
	}
}
