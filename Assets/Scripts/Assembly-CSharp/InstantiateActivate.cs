using UnityEngine;

public class InstantiateActivate : MonoBehaviour
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
		_prefabInstance = (GameObject)Object.Instantiate(prefab, base.transform.position, base.transform.rotation);
		_prefabInstance.transform.parent = base.transform;
		_prefabInstance.transform.localScale = Vector3.one;
		return _prefabInstance;
	}

	public void OnDrawGizmos()
	{
		Vector3 size = previewDimensions;
		size.Scale(base.transform.localScale);
		Gizmos.DrawWireCube(base.transform.position + previewOffset, size);
	}
}
