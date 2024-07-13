using UnityEngine;

public class BountyNotificationPanelSettings : NotificationPanel.Settings
{
	public Bounty Bounty { get; private set; }

	public override SoundEventData DisplaySound
	{
		get
		{
			return BountyNotificationPanel.Instance.bountyCompleteSound;
		}
	}

	public BountyNotificationPanelSettings(Bounty bounty)
	{
		Bounty = bounty;
		base.RestTime = 2f;
		base.CanBeEnqueued = true;
	}

	public override GameObject CreateInstanceForDisplay()
	{
		BountyNotificationPanel bountyNotificationPanel = BountyNotificationPanel.Instance;
		if (bountyNotificationPanel == null)
		{
			bountyNotificationPanel = BountyNotificationPanel.GetOrCreateInstance();
		}
		else
		{
			bountyNotificationPanel.SetHidden(false);
		}
		bountyNotificationPanel.Bounty = Bounty;
		return bountyNotificationPanel.gameObject;
	}

	public override void DestroyInstance()
	{
		SetHidden(true);
	}

	public override void SetHidden(bool isHidden)
	{
		BountyNotificationPanel instance = BountyNotificationPanel.Instance;
		if (instance != null)
		{
			instance.SetHidden(isHidden);
		}
	}
}
