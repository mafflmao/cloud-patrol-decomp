using UnityEngine;

public class DestoryWhenPrefabISEmpty : MonoBehaviour
{
	private void Update()
	{
		if (base.transform.childCount == 0)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
