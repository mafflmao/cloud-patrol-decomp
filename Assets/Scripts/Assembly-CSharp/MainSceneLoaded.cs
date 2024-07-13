using UnityEngine;

public class MainSceneLoaded : MonoBehaviour
{
	private void Start()
	{
		GameObject gameObject = GameObject.Find("/GameLoadCamera");
		if (gameObject != null)
		{
			Object.Destroy(gameObject);
		}
		gameObject = GameObject.Find("/MainSceneLoader");
		if (gameObject != null)
		{
			Object.Destroy(gameObject);
		}
	}
}
