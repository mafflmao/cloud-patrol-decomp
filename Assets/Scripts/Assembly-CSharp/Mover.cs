using UnityEngine;

public class Mover : MonoBehaviour
{
	public Vector3 direction = new Vector3(1f, 0f, 0f).normalized;

	public float speed = 10f;

	public bool moveNow;

	private int frameNumber;

	private void Start()
	{
	}

	private void Update()
	{
		frameNumber++;
		if (frameNumber >= 5 && moveNow)
		{
			Vector3 vector = new Vector3(direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime, direction.z * speed * Time.deltaTime);
			base.transform.position += vector;
		}
	}

	public void StartMoving()
	{
		moveNow = true;
	}

	public void StopMoving()
	{
		moveNow = false;
	}
}
