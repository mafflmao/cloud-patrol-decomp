using System;

public class LogOnEventArgs : EventArgs
{
	public bool AnonymousLogOn { get; private set; }

	public bool FirstTime { get; private set; }

	public LogOnEventArgs(bool anonymousLogOn, bool firstTime)
	{
		AnonymousLogOn = anonymousLogOn;
		FirstTime = firstTime;
	}
}
