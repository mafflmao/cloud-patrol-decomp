using System;
using System.Collections;
using UnityEngine;

public static class InvokeHelper
{
	public static void InvokeSafe(Action action, float time, MonoBehaviour behaviour)
	{
		if (behaviour == null)
		{
			Debug.LogError("NULL BEHVAIOUR");
		}
		if (action == null)
		{
			Debug.LogError("NULL ACTION");
		}
		else
		{
			behaviour.StartCoroutine(CoroutineInvoke(action, time, behaviour));
		}
	}

	private static IEnumerator CoroutineInvoke(Action action, float time, MonoBehaviour behaviour)
	{
		yield return new WaitForSeconds(time);
		if (behaviour != null)
		{
			action();
		}
		else
		{
			Debug.LogError("CRISIS AVERTED!");
		}
	}
}
