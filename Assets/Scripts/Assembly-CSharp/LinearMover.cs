using UnityEngine;

public class LinearMover : MonoBehaviour
{
	public float timeToDestination = 1f;

	public Vector3 endPosition;

	private Vector3 startPosition;

	private float elapsedTime;

	private void Start()
	{
		startPosition = base.transform.position;
	}

	private void Update()
	{
		if (timeToDestination > elapsedTime)
		{
			float t = 1f - (timeToDestination - elapsedTime) / timeToDestination;
			base.transform.position = Vector3.Lerp(startPosition, endPosition, t);
		}
		else
		{
			base.transform.position = endPosition;
			MoverWithSpeed.OnMoveCompleteHack(this);
			Object.Destroy(this);
		}
		elapsedTime += Time.deltaTime;
	}
}
