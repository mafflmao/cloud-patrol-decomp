using System.Collections.Generic;
using UnityEngine;

public class PlaneCast : MonoBehaviour
{
	public static int DefaultMaxSegments = 2;

	public static float CastDistance = 10f;

	public static IEnumerable<RaycastHit> Comb(Vector2 startScreenPoint, Vector2 endScreenPoint, int layerMask)
	{
		return Comb(startScreenPoint, endScreenPoint, layerMask, DefaultMaxSegments);
	}

	public static IEnumerable<RaycastHit> Comb(Vector2 startScreenPoint, Vector2 endScreenPoint, int layerMask, int maxSegments)
	{
		float distRatio = 0f;
		if (maxSegments > 0)
		{
			distRatio = 1f / (float)maxSegments;
		}
		for (int i = 0; i <= maxSegments; i++)
		{
			float t = (float)i * distRatio;
			Vector2 screenPoint = Vector2.Lerp(startScreenPoint, endScreenPoint, t);
			Ray ray = Camera.main.ScreenPointToRay(screenPoint);
			RaycastHit[] tempHits = Physics.RaycastAll(ray, CastDistance, layerMask);
			RaycastHit[] array = tempHits;
			for (int j = 0; j < array.Length; j++)
			{
				yield return array[j];
			}
		}
	}
}
