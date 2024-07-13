using UnityEngine;

public class Enemy : MonoBehaviour
{
	public Vector3 direction = new Vector3(1f, 0f, 0f).normalized;

	public float speed = 10f;

	private void Start()
	{
		AnimationStates component = GetComponent<AnimationStates>();
		if (component != null)
		{
			component.SendMessage("Walk");
		}
	}

	private void Update()
	{
		Vector3 vector = new Vector3(direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime, direction.z * speed * Time.deltaTime);
		base.transform.position += vector;
	}
}
