using UnityEngine;

public class Enemy_Inflated : MonoBehaviour
{
	public Vector3 direction = new Vector3(1f, 0f, 0f).normalized;

	public float speed = 10f;

	private void Start()
	{
		Transform transform = base.transform.Find("GoblinInflated");
		transform.GetComponent<Animation>().PlayQueued("Loop");
	}

	private void Update()
	{
		Vector3 vector = new Vector3(direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime, direction.z * speed * Time.deltaTime);
		base.transform.position += vector;
	}
}
