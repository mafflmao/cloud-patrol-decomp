using UnityEngine;

public class CollectionNotificationPanelSettings : NotificationPanel.Settings
{
	public string Message { get; protected set; }

	public CollectionNotificationPanelSettings(string message)
	{
		Message = message;
		base.CanBeEnqueued = true;
		base.RestTime = 4f;
		base.DismissOnRoomTransition = false;
		base.SuppressPreviouslyQueued = false;
	}

	public override GameObject CreateInstanceForDisplay()
	{
		CollectionNotificationPanel collectionNotificationPanel = CollectionNotificationPanel.Instance;
		if (collectionNotificationPanel == null)
		{
			collectionNotificationPanel = CollectionNotificationPanel.GetOrCreateInstance();
		}
		else
		{
			collectionNotificationPanel.SetHidden(false);
		}
		collectionNotificationPanel.Message = Message;
		return collectionNotificationPanel.gameObject;
	}

	public override void DestroyInstance()
	{
		SetHidden(true);
	}

	public override void SetHidden(bool isHidden)
	{
		CollectionNotificationPanel.Instance.SetHidden(isHidden);
	}
}
