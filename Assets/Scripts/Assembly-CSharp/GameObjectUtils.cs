using System.Collections.Generic;
using UnityEngine;

public class GameObjectUtils : MonoBehaviour
{
	public static void HideObject(GameObject obj)
	{
		if ((bool)obj)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = false;
			}
		}
	}

	public static void ShowObject(GameObject obj)
	{
		if ((bool)obj)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = true;
			}
		}
	}

	public static void EmitObject(GameObject obj)
	{
	}

	public static void DontEmitObject(GameObject obj)
	{
	
	}

	public static void ShowCharacterWithoutAccessories(GameObject character)
	{
		if ((bool)character)
		{
			Renderer[] componentsInChildren = character.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = true;
			}
			HideAccessory[] components = character.GetComponents<HideAccessory>();
			HideAccessory[] array2 = components;
			foreach (HideAccessory hideAccessory in array2)
			{
				hideAccessory.Hide();
			}
		}
	}

	public static void SetLayerRecursive(GameObject obj, int layer)
	{
		if ((bool)obj)
		{
			obj.layer = layer;
			Transform[] componentsInChildren = obj.transform.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				transform.gameObject.layer = layer;
			}
		}
	}

	public static void ManageLighting()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("CharacterStage");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.SetActive(false);
		}
	}

	public static void ManageLighting(GameObject activeCharacterLight, string currentStateName)
	{
		if (StateManager.Instance.CurrentStateName == currentStateName)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("CharacterStage");
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				gameObject.SetActive(false);
			}
			activeCharacterLight.SetActive(true);
		}
	}

	public static void ResetAmbientLight()
	{
		Color ambientLight = new Color(0.353f, 0.353f, 0.353f, 1f);
		RenderSettings.ambientLight = ambientLight;
	}

	public static string GetPathToTransform(Transform transform)
	{
		List<string> list = new List<string>();
		while (transform != null)
		{
			list.Add(transform.name);
			transform = transform.parent;
		}
		list.Reverse();
		return string.Join("/", list.ToArray());
	}

	public static void SetActiveRecursiveHack(GameObject root, bool isActive)
	{
		root.SetActive(isActive);
		int childCount = root.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject root2 = root.transform.GetChild(i).gameObject;
			SetActiveRecursiveHack(root2, isActive);
		}
	}
}
