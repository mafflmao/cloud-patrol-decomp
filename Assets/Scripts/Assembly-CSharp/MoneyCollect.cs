using UnityEngine;

public class MoneyCollect : MonoBehaviour
{
	private Transform destinationPoint;

	public float travelTime;

	private float startingTime;

	private Vector3 startingPosition;

	private void Start()
	{
		destinationPoint = ShipManager.instance.moneyDestinationPoint;
		startingPosition = base.transform.position;
		startingTime = Time.time;
		Object.Destroy(base.gameObject, travelTime);
	}

	private void Update()
	{
		float t = (Time.time - startingTime) / travelTime;
		base.transform.position = Vector3.Lerp(startingPosition, destinationPoint.position, t);
	}
}
