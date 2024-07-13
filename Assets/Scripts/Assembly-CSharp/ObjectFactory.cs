using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{
	private static ObjectFactory staticFactoryInstance;

	public List<ObjectPool> objectPools;

	private Dictionary<GameObject, ObjectPool> prefabToPoolMap;

	private Dictionary<GameObject, ObjectPool> gameobjectToPoolMap;

	public static ObjectFactory instance
	{
		get
		{
			if (staticFactoryInstance == null)
			{
				staticFactoryInstance = Object.FindObjectOfType(typeof(ObjectFactory)) as ObjectFactory;
			}
			if (staticFactoryInstance == null)
			{
				GameObject gameObject = new GameObject("ObjectFactory");
				staticFactoryInstance = gameObject.AddComponent(typeof(ObjectFactory)) as ObjectFactory;
				Debug.Log("Object Factory was not in scene. ObjectFactory generated.");
			}
			return staticFactoryInstance;
		}
	}

	private void Awake()
	{
		prefabToPoolMap = new Dictionary<GameObject, ObjectPool>();
		gameobjectToPoolMap = new Dictionary<GameObject, ObjectPool>();
		foreach (ObjectPool objectPool in objectPools)
		{
			objectPool.Build();
			prefabToPoolMap.Add(objectPool.gameObject, objectPool);
		}
	}

	private void Start()
	{
	}

	public GameObject PoolInstantiate(GameObject somePrefab)
	{
		GameObject gameObject = null;
		foreach (ObjectPool objectPool2 in objectPools)
		{
			if (objectPool2.gameObject == somePrefab)
			{
				Debug.Log("Found in pool " + somePrefab.name);
				gameObject = objectPool2.TakeFromPool();
				gameobjectToPoolMap.Add(gameObject, objectPool2);
				return gameObject;
			}
		}
		ObjectPool objectPool = new ObjectPool();
		objectPools.Add(objectPool);
		objectPool.gameObject = somePrefab;
		objectPool.Build();
		objectPool.name = somePrefab.name;
		gameObject = objectPool.TakeFromPool();
		gameobjectToPoolMap.Add(gameObject, objectPool);
		return gameObject;
	}

	public GameObject PoolInstantiate(GameObject somePrefab, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = PoolInstantiate(somePrefab);
		gameObject.transform.position = position;
		gameObject.transform.localRotation = rotation;
		return gameObject;
	}

	public ObjectPool PoolBelongingTo(GameObject someObject)
	{
		ObjectPool value;
		gameobjectToPoolMap.TryGetValue(someObject, out value);
		Debug.Log("Object " + someObject.name + " belongs to pool " + value.name);
		return value;
	}

	public void PoolDestroy(GameObject someGameObject)
	{
		Debug.Log("PoolDestroy " + someGameObject.name);
		ObjectPool objectPool = PoolBelongingTo(someGameObject);
		objectPool.ReturnToPool(someGameObject);
	}

	public void PoolDestroy(GameObject someGameObject, float time)
	{
		Debug.Log("PoolDestroy " + someGameObject.name);
		StartCoroutine(DelayedDestroy(someGameObject, time));
	}

	private IEnumerator DelayedDestroy(GameObject someGameObject, float time)
	{
		Debug.Log("PoolDestroy (delayed) " + someGameObject.name);
		yield return new WaitForSeconds(time);
		PoolDestroy(someGameObject);
	}

	private void OnApplicationQuit()
	{
		staticFactoryInstance = null;
	}
}
