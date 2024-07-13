using System.Collections.Generic;
using UnityEngine;

public class SoundEventData : ScriptableObject
{
	public enum SoundChoiceBehavior
	{
		Sequential = 0,
		Random = 1,
		RandomNoRepeat = 2
	}

	public enum LimitBehavior
	{
		ReplaceOldest = 0,
		Skip = 1
	}

	public enum PauseBehaviour
	{
		Pause = 0,
		Stop = 1,
		Continue = 2
	}

	public int maxPlaybacks = 2;

	public float playbackPercentage = 1f;

	public float randomVolumePercent;

	public float randomPitchPercent;

	public LimitBehavior playbackLimitBehavior;

	public SoundChoiceBehavior nextSoundBehavior = SoundChoiceBehavior.Random;

	public VolumeGroup volumeGroup;

	public PauseBehaviour pauseBehaviour;

	public PauseBehaviour cutscenePauseBehaviour = PauseBehaviour.Continue;

	public bool overridePauseBehaviourInCutscene;

	public bool effectedByTimeScale = true;

	[HideInInspector]
	public List<SoundEventAudioSourceData> audioSourceData;

	public float GetVolumeAfterRandomness(SoundEventAudioSourceData sourceData)
	{
		float num = sourceData.volume * ((!(volumeGroup != null)) ? 1f : volumeGroup.GetCurrentCombinedRuntimeVolume());
		float num2 = num * ((!sourceData.overrideVolumeRandomness) ? randomVolumePercent : sourceData.randomVolumePercentOverride);
		float num3 = Random.Range(0f - num2, num2);
		return num + num3;
	}

	public float GetPitchAfterRandomness(SoundEventAudioSourceData sourceData)
	{
		float num = sourceData.pitch * ((!sourceData.overridePitchRandomness) ? randomPitchPercent : sourceData.randomPitchPercentOverride);
		float num2 = Random.Range(0f - num, num);
		return sourceData.pitch + num2;
	}
}
