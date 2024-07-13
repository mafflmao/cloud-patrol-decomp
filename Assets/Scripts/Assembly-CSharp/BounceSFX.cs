using UnityEngine;

public class BounceSFX : MonoBehaviour
{
	public SoundEventData bounceSFX;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (bounceSFX != null)
		{
			SoundEventManager.Instance.Play(bounceSFX, base.gameObject);
		}
	}
}
