using UnityEngine;

public class PlaySoundOnStart : MonoBehaviour
{
	public SoundEventData soundToPlay;

	public bool StopOnDestroy;

	private void Start()
	{
		if (soundToPlay != null)
		{
			SoundEventManager.Instance.Play(soundToPlay, base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (StopOnDestroy)
		{
			SoundEventManager.Instance.Stop(soundToPlay, base.gameObject);
		}
	}
}
