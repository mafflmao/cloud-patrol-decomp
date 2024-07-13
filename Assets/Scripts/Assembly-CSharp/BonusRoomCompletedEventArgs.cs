using System;

public class BonusRoomCompletedEventArgs : EventArgs
{
	public int RewardNumber { get; private set; }

	public BonusRoomCompletedEventArgs(int rewardNumber)
	{
		RewardNumber = rewardNumber;
	}
}
