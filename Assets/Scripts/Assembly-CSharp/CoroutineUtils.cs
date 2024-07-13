using System.Collections;
using UnityEngine;

public static class CoroutineUtils
{
	public static IEnumerator WaitForWallTime(float time)
	{
		float finishTime = Time.realtimeSinceStartup + time;
		while (Time.realtimeSinceStartup < finishTime)
		{
			yield return new WaitForEndOfFrame();
		}
	}
}
