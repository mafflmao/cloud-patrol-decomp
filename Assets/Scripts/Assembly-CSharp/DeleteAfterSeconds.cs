using UnityEngine;

public class DeleteAfterSeconds : MonoBehaviour
{
	public float deleteAfter = 3f;

	private bool started;

	private float timer;

	private void Update()
	{
		if (started)
		{
			if (timer < Time.time)
			{
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			started = true;
			timer = Time.time + deleteAfter;
		}
	}
}
