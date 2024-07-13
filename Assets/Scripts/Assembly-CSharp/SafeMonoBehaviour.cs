using System.Collections;
using UnityEngine;

public class SafeMonoBehaviour : MonoBehaviour
{
	protected static bool IsShuttingDown { get; private set; }

	protected virtual void OnApplicationQuit()
	{
		IsShuttingDown = true;
	}

	protected Coroutine WaitForSecondsAndGame(float timeInSeconds)
	{
		return StartCoroutine(Wait_Internal(timeInSeconds, false));
	}

	protected Coroutine WaitForGame()
	{
		return StartCoroutine(Wait_Internal(0f, false));
	}

	protected Coroutine WaitForEndOfFrameAndGame()
	{
		return StartCoroutine(Wait_Internal(0f, true));
	}

	private IEnumerator Wait_Internal(float timeInSeconds, bool waitForFrame)
	{
		if (timeInSeconds != 0f)
		{
			yield return new WaitForSeconds(timeInSeconds);
		}
		if (waitForFrame)
		{
			yield return new WaitForEndOfFrame();
		}
		while (GameManager.gameState != GameManager.GameState.Playing)
		{
			yield return new WaitForEndOfFrame();
		}
	}
}
