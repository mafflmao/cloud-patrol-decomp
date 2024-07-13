using System;
using UnityEngine;

[Serializable]
public class SoundEventAudioSourceData
{
	public enum RolloffMode
	{
		Linear = 0,
		Logorithmic = 1
	}

	public AudioClip clip;

	public bool mute;

	public bool bypassEffects;

	public bool loop;

	public int priority = 128;

	public float volume = 1f;

	public float pitch = 1f;

	public float panLevel = 1f;

	public float spread;

	public float dopplerLevel;

	public float minDistance = 40f;

	public float maxDistance = 80f;

	public RolloffMode rolloffMode;

	public float pan2D;

	public bool overridePitchRandomness;

	public float randomPitchPercentOverride;

	public bool overrideVolumeRandomness;

	public float randomVolumePercentOverride;

	[NonSerialized]
	public bool expanded;

	[NonSerialized]
	public bool threeDeeExpanded;

	[NonSerialized]
	public bool twoDeeExpanded;

	[NonSerialized]
	public bool overridesExpanded;

	public SoundEventAudioSourceData Clone()
	{
		SoundEventAudioSourceData soundEventAudioSourceData = new SoundEventAudioSourceData();
		CloneUtility.CopyPublicFields(this, soundEventAudioSourceData);
		return soundEventAudioSourceData;
	}
}
