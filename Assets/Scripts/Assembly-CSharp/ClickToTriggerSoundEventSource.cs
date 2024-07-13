using UnityEngine;

public class ClickToTriggerSoundEventSource : MonoBehaviour
{
	public SoundEventData mouse0Sound;

	public SoundEventData mouse1Sound;

	public VolumeGroup volumeGroup;

	public float volumeGroupVolume = 0.5f;

	private bool volumeGroupToggle;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			SoundEventManager.Instance.Play(mouse0Sound, base.gameObject);
		}
		if (Input.GetMouseButtonDown(1))
		{
			SoundEventManager.Instance.Play(mouse1Sound, base.gameObject);
		}
		if (Input.GetMouseButtonDown(2) && volumeGroup != null)
		{
			volumeGroup.Initialize();
			volumeGroupToggle = !volumeGroupToggle;
			if (volumeGroupToggle)
			{
				volumeGroup.RuntimeVolume = volumeGroupVolume;
			}
			else
			{
				volumeGroup.RuntimeVolume = volumeGroup.volume;
			}
		}
	}
}
