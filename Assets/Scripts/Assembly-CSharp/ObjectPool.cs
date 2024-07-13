using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPool
{
	public string name;

	public int currentNumber;

	public int maxNumber;

	public GameObject gameObject;

	public List<GameObject> gameObjects;

	public void Build()
	{
		Debug.Log("Building Pool with prefab: " + this.gameObject.name);
		for (int i = 0; i < maxNumber; i++)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.gameObject);
			gameObjects.Add(gameObject);
			gameObject.SetActive(false);
		}
	}

	public int GetNumberAvailable()
	{
		return gameObjects.Count;
	}

	public GameObject TakeFromPool()
	{
		foreach (GameObject gameObject2 in gameObjects)
		{
			if (!gameObject2.activeSelf)
			{
				gameObject2.SetActive(true);
				return gameObject2;
			}
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.gameObject);
		gameObjects.Add(gameObject);
		gameObject.SetActive(true);
		return gameObject;
	}

	public void ReturnToPool(GameObject someObject)
	{
		Debug.Log("Before: " + someObject.activeSelf);
		someObject.SetActive(false);
		Debug.Log("After: " + someObject.activeSelf);
		Debug.Log("Returning to pool");
		if (gameObjects.Contains(someObject))
		{
			Debug.Log("Contained!");
			someObject.SetActive(false);
			return;
		}
		Debug.LogError(string.Concat("Tried to incorrectly return ", someObject, " to pool ", name, "."));
	}
}
