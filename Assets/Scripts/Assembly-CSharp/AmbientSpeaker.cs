using System.Collections.Generic;
using UnityEngine;

public class AmbientSpeaker : SafeMonoBehaviour
{
	public SoundEventData ambience;

	public bool useObjectListToTurnOff;

	public List<GameObject> objectList;

	private bool isPlaying;

	private void Update()
	{
		if (!useObjectListToTurnOff)
		{
			return;
		}
		for (int num = objectList.Count - 1; num >= 0; num--)
		{
			if (objectList[num] == null)
			{
				objectList.RemoveAt(num);
			}
		}
		if (objectList.Count == 0)
		{
			Object.Destroy(this);
		}
	}

	private void OnEnable()
	{
		Play();
	}

	private void OnDisable()
	{
		Stop();
	}

	private void OnDestroy()
	{
		if (!SafeMonoBehaviour.IsShuttingDown && SoundEventManager.Instance == null)
		{
			Stop();
		}
	}

	public void Play()
	{
		if (!isPlaying)
		{
			SoundEventManager.Instance.Play(ambience, base.gameObject);
			isPlaying = true;
		}
	}

	public void Stop()
	{
		if (isPlaying)
		{
			SoundEventManager.Instance.Stop(ambience, base.gameObject);
			isPlaying = false;
		}
	}
}
