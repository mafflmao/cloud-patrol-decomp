using System;

public class BountyChangeEventArgs : EventArgs
{
	public int BountyNumber { get; private set; }

	public Bounty OldBounty { get; private set; }

	public Bounty NewBounty { get; private set; }

	public BountyChangeEventArgs(int bountyNumber, Bounty oldBounty, Bounty newBounty)
	{
		BountyNumber = bountyNumber;
		OldBounty = oldBounty;
		NewBounty = newBounty;
	}
}
