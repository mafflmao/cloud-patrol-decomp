using System;

public class IntegerChangeEventArgs : EventArgs
{
	public int StartValue
	{
		get
		{
			return FinalValue - Delta;
		}
	}

	public int Delta { get; private set; }

	public int FinalValue { get; private set; }

	public IntegerChangeEventArgs(int delta, int finalValue)
	{
		FinalValue = finalValue;
		Delta = delta;
	}
}
