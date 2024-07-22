using System.Collections;
using UnityEngine;

public class SplashScreenController : MonoBehaviour
{
	private IEnumerator Start()
	{
		Application.targetFrameRate = 60;
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Application.LoadLevel("ElementSelect");
	}
}
