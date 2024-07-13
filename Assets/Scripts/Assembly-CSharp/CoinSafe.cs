using System.Collections;
using UnityEngine;

public class CoinSafe : MonoBehaviour
{
	private Vector3 orbitPoint;

	private Vector3 reflectedDragtargetPosition;

	private Vector3 destination;

	private float speed;

	public float topSpeed = 0.6f;

	public float minSpeed = 0.05f;

	public float dragTargetDistanceThreshold = 0.5f;

	public float idleSpeed = 6f;

	private bool doRetreat;

	private float lastDragtargetDistance = 100000f;

	private Vector3 dragTargetPosition;

	private void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
		StartCoroutine(CheckDragtargetDistance());
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
	}

	private void Update()
	{
		Debug.Log("LTDS: " + lastDragtargetDistance);
		orbitPoint.x = Mathf.Sin(Time.time * idleSpeed);
		orbitPoint.y = Mathf.Cos(Time.time * idleSpeed);
		orbitPoint.z = 0f;
		orbitPoint = base.transform.TransformPoint(orbitPoint);
		if (doRetreat)
		{
			speed = topSpeed;
			destination = reflectedDragtargetPosition;
			return;
		}
		if (speed >= minSpeed)
		{
			speed *= 0.9f;
		}
		else
		{
			speed = minSpeed;
		}
		destination = orbitPoint;
	}

	private IEnumerator CheckDragtargetDistance()
	{
		while (true)
		{
			if (ShipManager.instance.dragMultiTarget[0].isSelecting)
			{
				Vector3 myScreenspacePosition = Camera.main.WorldToScreenPoint(base.transform.position);
				Vector3 dragTargetScreenPos = ShipManager.instance.dragMultiTarget[0].previousScreenPos;
				dragTargetPosition = Camera.main.ScreenToWorldPoint(new Vector3(dragTargetScreenPos.x, dragTargetScreenPos.y, myScreenspacePosition.z));
				reflectedDragtargetPosition = dragTargetPosition - base.transform.position;
				reflectedDragtargetPosition = -reflectedDragtargetPosition;
				reflectedDragtargetPosition *= 1f / reflectedDragtargetPosition.magnitude;
				reflectedDragtargetPosition += base.transform.position;
				lastDragtargetDistance = Vector3.Distance(dragTargetPosition, base.transform.position);
				if (lastDragtargetDistance < dragTargetDistanceThreshold)
				{
					doRetreat = true;
				}
				else
				{
					doRetreat = false;
				}
			}
			else
			{
				yield return new WaitForSeconds(1f);
				doRetreat = false;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void LateUpdate()
	{
		base.transform.position = Vector3.MoveTowards(base.transform.position, destination, speed);
	}
}
