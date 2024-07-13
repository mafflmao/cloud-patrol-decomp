using UnityEngine;

public class PurchaseNotificationPanelSettings : NotificationPanel.Settings
{
	public string Message { get; protected set; }

	public PurchaseNotificationPanelSettings(string message)
	{
		Message = message;
		base.CanBeEnqueued = true;
		base.RestTime = 4f;
		base.DismissOnRoomTransition = false;
		base.SuppressPreviouslyQueued = false;
	}

	public override GameObject CreateInstanceForDisplay()
	{
		PurchaseNotificationPanel purchaseNotificationPanel = PurchaseNotificationPanel.Instance;
		if (purchaseNotificationPanel == null)
		{
			purchaseNotificationPanel = PurchaseNotificationPanel.GetOrCreateInstance();
		}
		else
		{
			purchaseNotificationPanel.SetHidden(false);
		}
		purchaseNotificationPanel.Message = Message;
		return purchaseNotificationPanel.gameObject;
	}

	public override void DestroyInstance()
	{
		SetHidden(true);
	}

	public override void SetHidden(bool isHidden)
	{
		PurchaseNotificationPanel.Instance.SetHidden(isHidden);
	}
}
