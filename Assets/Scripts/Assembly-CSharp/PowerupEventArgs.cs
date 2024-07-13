using System;

public class PowerupEventArgs : EventArgs
{
	public PowerupData PowerupData { get; private set; }

	public PowerupEventArgs(PowerupData powerupData)
	{
		PowerupData = powerupData;
	}
}
