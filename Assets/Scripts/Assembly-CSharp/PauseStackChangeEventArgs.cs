using System;

public class PauseStackChangeEventArgs : EventArgs
{
	public PauseReason PauseReason { get; private set; }

	public bool WasPush { get; private set; }

	public PauseStackChangeEventArgs(PauseReason pauseReason, bool wasPush)
	{
		PauseReason = pauseReason;
		WasPush = wasPush;
	}
}
