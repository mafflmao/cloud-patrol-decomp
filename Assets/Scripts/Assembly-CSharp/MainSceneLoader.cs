using System.Collections;
using UnityEngine;

public class MainSceneLoader : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		Application.LoadLevelAdditiveAsync("MainScene");
	}
}
