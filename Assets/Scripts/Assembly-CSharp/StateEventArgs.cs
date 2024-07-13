using System;

public class StateEventArgs : EventArgs
{
	public string StateName { get; private set; }

	public StateEventArgs(string stateName)
	{
		StateName = stateName;
	}
}
