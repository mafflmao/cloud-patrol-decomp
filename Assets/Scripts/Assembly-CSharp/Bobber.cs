using UnityEngine;

public class Bobber : MonoBehaviour
{
	public float bobMagnitude = 0.03f;

	public float maxMovementPerFrame = 0.05f;

	private Vector3 myOriginalLocalPosition;

	private Vector3 myBobbingPosition;

	private void Start()
	{
		myOriginalLocalPosition = base.transform.localPosition;
	}

	private void LateUpdate()
	{
		myBobbingPosition = myOriginalLocalPosition + new Vector3(0f, bobMagnitude * Mathf.Sin(Time.time), 0f);
		base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, myBobbingPosition, maxMovementPerFrame);
	}
}
