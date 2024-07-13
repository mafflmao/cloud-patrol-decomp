using System.Collections.Generic;
using UnityEngine;

public class ConeCast : MonoBehaviour
{
	private static float _storedSlope = 0f;

	private static Vector3 _cameraPosition = Vector3.zero;

	public static IEnumerable<RaycastHit> GetHits(Ray ray, float radius, float raycastDistance, int layerMask)
	{
		_cameraPosition = Camera.main.transform.position;
		float distanceFromCamera = Vector3.Distance(ShipManager.instance.dragMultiTarget[0].transform.position, _cameraPosition);
		_storedSlope = radius / distanceFromCamera;
		float maxRadius = raycastDistance * _storedSlope;
		RaycastHit[] tempHits = Physics.SphereCastAll(ray, maxRadius, raycastDistance, layerMask);
		RaycastHit[] array = tempHits;
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit hit = array[i];
			if (IsInBounds(hit.transform.position, ray))
			{
				yield return hit;
			}
		}
	}

	public static IEnumerable<RaycastHit> GetHits(Ray ray, Vector3 objectPos, float radius, float raycastDistance, int layerMask)
	{
		_cameraPosition = Camera.main.transform.position;
		float distanceFromCamera = Vector3.Distance(objectPos, _cameraPosition);
		_storedSlope = radius / distanceFromCamera;
		float maxRadius = raycastDistance * _storedSlope;
		RaycastHit[] tempHits = Physics.SphereCastAll(ray, maxRadius, raycastDistance, layerMask);
		RaycastHit[] array = tempHits;
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit hit = array[i];
			if (IsInBounds(hit.transform.position, ray))
			{
				yield return hit;
			}
		}
	}

	private static bool IsInBounds(Vector3 point, Ray ray)
	{
		float sqrMagnitude = (point - _cameraPosition).sqrMagnitude;
		float num = _storedSlope * _storedSlope * sqrMagnitude;
		float num2 = SquareDistanceToRay(ray, point);
		if (num2 < num)
		{
			return true;
		}
		return false;
	}

	public static float SquareDistanceToRay(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).sqrMagnitude;
	}
}
