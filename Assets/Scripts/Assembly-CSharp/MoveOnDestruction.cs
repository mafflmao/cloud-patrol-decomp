using System.Collections.Generic;
using UnityEngine;

public class MoveOnDestruction : MonoBehaviour
{
	public List<Transform> startLevelObj;

	public GameObject[] spawners;

	public GameObject objectToMove;

	public bool destroyWhenDone;

	public float startTimer;

	private void Update()
	{
		if (startLevelObj.Count > 0)
		{
			Transform item = null;
			foreach (Transform item2 in startLevelObj)
			{
				if (item2 == null)
				{
					item = item2;
				}
			}
			startLevelObj.Remove(item);
			return;
		}
		objectToMove.SendMessage("StartMoving", SendMessageOptions.DontRequireReceiver);
		if (spawners.Length > 0)
		{
			for (int i = 0; i < spawners.Length; i++)
			{
				ObjectSpawner component = spawners[i].GetComponent<ObjectSpawner>();
				component.SendMessage("StartSpawn");
			}
		}
		if (destroyWhenDone)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
