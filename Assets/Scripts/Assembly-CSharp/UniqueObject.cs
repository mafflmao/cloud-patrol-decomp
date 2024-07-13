using System.Collections.Generic;
using UnityEngine;

public class UniqueObject : MonoBehaviour
{
	private static Dictionary<string, GameObject> _instances = new Dictionary<string, GameObject>();

	public string ObjectID = "objectType";

	private void Awake()
	{
		GameObject value;
		if (!_instances.TryGetValue(ObjectID, out value) || value == null)
		{
			_instances[ObjectID] = base.gameObject;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
