using System;

public class PowerupStateChangeEventArgs : EventArgs
{
	public PowerupStates OldState { get; private set; }

	public PowerupStateChangeEventArgs(PowerupStates oldState)
	{
		OldState = oldState;
	}
}
