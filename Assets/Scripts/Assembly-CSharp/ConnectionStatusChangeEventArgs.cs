using System;

public class ConnectionStatusChangeEventArgs : EventArgs
{
	public Bedrock.brUserConnectionStatus OldStatus { get; private set; }

	public Bedrock.brUserConnectionStatus NewStatus { get; private set; }

	public ConnectionStatusChangeEventArgs(Bedrock.brUserConnectionStatus oldStatus, Bedrock.brUserConnectionStatus newStatus)
	{
		OldStatus = oldStatus;
		NewStatus = newStatus;
	}
}
