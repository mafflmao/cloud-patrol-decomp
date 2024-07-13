using UnityEngine;

public class PrefabPlaceholder : MonoBehaviour
{
	public Vector3 previewDimensions = new Vector3(10f, 10f, 0f);

	public Vector3 previewOffset = new Vector3(0f, 0f, 0f);

	public GameObject prefab;

	private GameObject _prefabInstance;

	public bool instantiateOnAwake;

	public void Awake()
	{
		if (instantiateOnAwake)
		{
			InstantiatePrefab();
		}
	}

	public GameObject InstantiatePrefab()
	{
		if (prefab != null)
		{
			_prefabInstance = (GameObject)Object.Instantiate(prefab, base.transform.position, base.transform.rotation);
			_prefabInstance.transform.parent = base.transform;
			_prefabInstance.transform.localScale = Vector3.one;
			return _prefabInstance;
		}
		Debug.Log("ERROR-----------------" + base.gameObject.name);
		return null;
	}

	public void OnDrawGizmos()
	{
		Vector3 size = previewDimensions;
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(previewOffset, size);
		Gizmos.matrix = matrix;
	}
}
