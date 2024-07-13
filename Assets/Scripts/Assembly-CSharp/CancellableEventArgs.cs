using System;

public class CancellableEventArgs : EventArgs
{
	public bool IsCancelled { get; private set; }

	public void Cancel()
	{
		DebugScreen.Log("FireAtTargetsCanceled");
		IsCancelled = true;
	}
}
