using System;
using UnityEngine;

public class BombController : MonoBehaviour
{
	public float seconds = 3f;

	public float playbackSpeed = 0.8f;

	public SoundEventData triggerSFX;

	public GameObject smoke;

	public GameObject shadow;

	public static event EventHandler<EventArgs> BombControllerStarted;

	private void Awake()
	{
		OnBombControllerStarted();
	}

	private void Start()
	{
		if (shadow != null && shadow.GetComponent<Renderer>() != null)
		{
			shadow.GetComponent<Renderer>().enabled = false;
		}
		SoundEventManager.Instance.Play2D(triggerSFX);
		InvokeHelper.InvokeSafe(Play, seconds, this);
	}

	private void Play()
	{
		foreach (AnimationState item in base.GetComponent<Animation>())
		{
			item.speed = playbackSpeed;
		}
		base.GetComponent<Animation>().Play();
		UnityEngine.Object.Destroy(base.gameObject, base.GetComponent<Animation>().clip.length);
	}

	public void Smoke()
	{
	}

	protected void OnBombControllerStarted()
	{
		Debug.Log("Sending Bombcontrolelrstarted event");
		if (BombController.BombControllerStarted != null)
		{
			BombController.BombControllerStarted(this, new EventArgs());
		}
	}
}
