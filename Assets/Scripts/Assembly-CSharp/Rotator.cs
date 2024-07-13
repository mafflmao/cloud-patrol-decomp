using UnityEngine;

public class Rotator : MonoBehaviour
{
	public Vector3 rotation = new Vector3(1f, 0f, 0f);

	public float speed = 2f;

	public int randomRotationX;

	public int randomRotationY;

	public int randomRotationZ;

	private int rand;

	private void Start()
	{
		if (randomRotationX != 0)
		{
			rand = Random.Range(-1, 2);
			rotation = new Vector3(rand, rotation.y, rotation.z);
		}
		if (randomRotationY != 0)
		{
			rand = Random.Range(-1, 2);
			rotation = new Vector3(rotation.x, rand, rotation.z);
		}
		if (randomRotationZ != 0)
		{
			rand = Random.Range(-1, 2);
			rotation = new Vector3(rotation.x, rotation.y, rand);
		}
	}

	private void LateUpdate()
	{
		if (!GameManager.Instance.IsGameOver && !GameManager.Instance.IsPaused)
		{
			if (rotation.z != 0f)
			{
				base.transform.Rotate(Vector3.forward * rotation.z * speed);
			}
			if (rotation.y != 0f)
			{
				base.transform.Rotate(Vector3.right * rotation.y * speed);
			}
			if (rotation.x != 0f)
			{
				base.transform.Rotate(Vector3.up * rotation.x * speed);
			}
		}
	}
}
