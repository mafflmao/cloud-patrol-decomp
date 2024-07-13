using UnityEngine;

public class ExplosionDelete : MonoBehaviour
{
	public float deleteAfter = 3f;

	private bool started;

	private float timer;

	private void OnExplosionStart()
	{
		started = true;
		timer = Time.time + deleteAfter;
	}

	private void Update()
	{
		if (started && timer < Time.time)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
