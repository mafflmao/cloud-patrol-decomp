using System;

public class FriendInviteCountEventArgs : EventArgs
{
	public uint NumFriendInvites { get; private set; }

	public FriendInviteCountEventArgs(uint numFriendInvites)
	{
		NumFriendInvites = numFriendInvites;
	}
}
