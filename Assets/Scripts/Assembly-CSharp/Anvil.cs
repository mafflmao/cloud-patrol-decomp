using UnityEngine;

public class Anvil : MonoBehaviour
{
	[HideInInspector]
	public bool hasBounced;

	public SoundEventData anvilCollisionSound;

	private TrailRenderer anvilTrail;

	private void OnEnable()
	{
		hasBounced = false;
		base.GetComponent<Rigidbody>().velocity = new Vector3(0f, -5f, 0f);
		anvilTrail = GetComponent<TrailRenderer>();
	}

	private void Update()
	{
		if (anvilTrail.enabled && hasBounced && anvilTrail.time > 0f)
		{
			anvilTrail.time -= 0.02f;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		base.GetComponent<Rigidbody>().AddForce(base.GetComponent<Rigidbody>().velocity * -2f + new Vector3(0f, 0f, 1f), ForceMode.VelocityChange);
		Vector3 torque = new Vector3(Random.Range(70f, 100f), Random.Range(-45f, 45f), Random.Range(-45f, 45f));
		base.GetComponent<Rigidbody>().AddTorque(torque);
		base.GetComponent<Collider>().enabled = false;
		hasBounced = true;
		PlayCollideSFX();
	}

	public void PlayCollideSFX()
	{
		SoundEventManager.Instance.Play(anvilCollisionSound, base.gameObject);
	}
}
