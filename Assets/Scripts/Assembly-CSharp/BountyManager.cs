using System;

public class BountyManager : SingletonMonoBehaviour
{
	private static BountyManager _instance;

	public static BountyManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<BountyManager>();
		}
	}

	public void Start()
	{
	}

	public void OnEnable()
	{
		Bounty.BountyComplete += HandleBountyComplete;
		GameManager.GameOver += HandleGameOver;
	}

	public void OnDisable()
	{
		GameManager.GameOver -= HandleGameOver;
		Bounty.BountyComplete -= HandleBountyComplete;
	}

	private void HandleGameOver(object sender, EventArgs e)
	{
		for (int i = 0; i < BountyChooser.Instance.ActiveBounties.Length; i++)
		{
		}
	}

	private void HandleBountyComplete(object sender, EventArgs e)
	{
		Bounty bounty = (Bounty)sender;
		NotificationPanel.Instance.Display(new BountyNotificationPanelSettings(bounty));
		SwrveEventsProgression.GoalCompleted(bounty.bountyData.Id, bounty.bountyData.Reward);
	}
}
