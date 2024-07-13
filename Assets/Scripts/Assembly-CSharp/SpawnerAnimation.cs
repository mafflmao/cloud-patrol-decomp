using UnityEngine;

public class SpawnerAnimation : MonoBehaviour
{
	public int X;

	public int Y;

	public int Z;

	private Vector3 direction;

	public float speed = 2f;

	public float xRangeHalved = 2f;

	private float maxXpos;

	private float minXpos;

	public float yRangeHalved = 2f;

	private float maxYpos;

	private float minYpos;

	private void Start()
	{
		maxXpos = base.transform.position.x + xRangeHalved;
		minXpos = base.transform.position.x - xRangeHalved;
		maxYpos = base.transform.position.y + yRangeHalved;
		minYpos = base.transform.position.y - yRangeHalved;
		direction = new Vector3(X, Y, Z);
	}

	private void Update()
	{
		move();
	}

	private void move()
	{
		Vector3 vector = new Vector3(direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime, direction.z * speed * Time.deltaTime);
		base.transform.position += vector;
		checkOnScreen();
	}

	private void checkOnScreen()
	{
		if (base.transform.position.y >= maxYpos || base.transform.position.y <= minYpos)
		{
			direction.y = 0f - direction.y;
		}
		if (base.transform.position.x >= maxXpos || base.transform.position.x <= minXpos)
		{
			direction.x = 0f - direction.x;
		}
	}
}
