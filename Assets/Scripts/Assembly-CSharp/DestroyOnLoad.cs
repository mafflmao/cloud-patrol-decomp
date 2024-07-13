using UnityEngine;

public class DestroyOnLoad : MonoBehaviour
{
	public GameObject[] objectsToDestroy;

	private void OnStateLoaded()
	{
		if (StateManager.Instance.StateCount > 1)
		{
			GameObject[] array = objectsToDestroy;
			foreach (GameObject obj in array)
			{
				Object.Destroy(obj);
			}
		}
	}
}
