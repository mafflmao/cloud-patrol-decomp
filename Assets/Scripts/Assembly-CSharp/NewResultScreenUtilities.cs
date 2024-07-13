using UnityEngine;

public class NewResultScreenUtilities : MonoBehaviour
{
	public GameObject[] m_GameObjectsToHide;

	private void Awake()
	{
		GameObject[] gameObjectsToHide = m_GameObjectsToHide;
		foreach (GameObject gameObject in gameObjectsToHide)
		{
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = false;
			}
		}
	}
}
