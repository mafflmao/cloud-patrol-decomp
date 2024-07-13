using UnityEngine;

public class TransformUtil
{
	public static Transform FindRecursive(Transform input, string name)
	{
		if (input.name == name)
		{
			return input;
		}
		for (int i = 0; i < input.childCount; i++)
		{
			Transform transform = FindRecursive(input.GetChild(i), name);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	public static Vector3 MoveDistanceFromCamera(Vector3 location, float distance)
	{
		Vector3 vector = Camera.main.WorldToScreenPoint(location);
		return Camera.main.ScreenToWorldPoint(new Vector3(vector.x, vector.y, distance));
	}
}
