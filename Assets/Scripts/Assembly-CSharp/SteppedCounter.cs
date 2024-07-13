using System;
using UnityEngine;

[Serializable]
public class SteppedCounter
{
	public int stepsPerIncrement = 2;

	public float value;

	public float targetValue = 5f;

	public float incrementAmount = 0.5f;

	private bool initialized;

	private bool finished;

	private int stepsRemainingBeforeIncrement;

	public void Step()
	{
		if (!initialized)
		{
			stepsRemainingBeforeIncrement = stepsPerIncrement;
			if (incrementAmount == 0f)
			{
				Debug.LogError("Target value will never be reached because increment value is 0.");
			}
			else if ((value < targetValue && incrementAmount < 0f) || (value > targetValue && incrementAmount > 0f))
			{
				Debug.LogError("Target value will never be reached because increment amount has wrong sign.");
			}
			initialized = true;
		}
		if (finished)
		{
			return;
		}
		stepsRemainingBeforeIncrement--;
		if (stepsRemainingBeforeIncrement <= 0)
		{
			value += incrementAmount;
			stepsRemainingBeforeIncrement = stepsPerIncrement;
			if ((incrementAmount > 0f && targetValue <= value) || (incrementAmount < 0f && targetValue >= value))
			{
				finished = true;
				value = targetValue;
			}
		}
	}
}
