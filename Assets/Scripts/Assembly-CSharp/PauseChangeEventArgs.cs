using System;

public class PauseChangeEventArgs : EventArgs
{
	public PauseReason PauseReason { get; private set; }

	public PauseChangeEventArgs(PauseReason reason)
	{
		PauseReason = reason;
	}
}
