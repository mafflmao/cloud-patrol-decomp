using UnityEngine;

public class ParticleTrigger : MonoBehaviour
{
	public Transform[] particles;

	public SoundEventData soundParticle;

	public float delaySFX;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other == Camera.main.GetComponent<Collider>())
		{
			for (int i = 0; i < particles.Length; i++)
			{
				Object.Instantiate(particles[i], base.transform.position, Quaternion.identity);
			}
		}
		SoundEventManager.Instance.Play(soundParticle, base.gameObject, delaySFX);
	}
}
