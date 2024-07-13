using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundEvent
{
	private SoundEventData _data;

	private int _lastSoundIndex = -1;

	private LinkedList<AudioSource> _playingSources = new LinkedList<AudioSource>();

	private HashSet<AudioSource> _pausedSounds = new HashSet<AudioSource>();

	public bool IsPaused { get; private set; }

	private bool IsLooping
	{
		get
		{
			foreach (SoundEventAudioSourceData audioSourceDatum in _data.audioSourceData)
			{
				if (audioSourceDatum.loop)
				{
					return true;
				}
			}
			return false;
		}
	}

	public SoundEvent(SoundEventData data)
	{
		_data = data;
		_data.volumeGroup.Initialize();
		_data.volumeGroup.CombinedVolumeChanged += Handle_datavolumeGroupCombinedVolumeChanged;
	}

	private void Handle_datavolumeGroupCombinedVolumeChanged(object sender, VolumeGroup.VolumeChangedEventArgs e)
	{
		DestroyFinishedAudioSources();
		foreach (AudioSource item in _playingSources.Concat(_pausedSounds))
		{
			item.volume *= _data.volumeGroup.RuntimeVolume / e.OldVolumeLevel;
		}
	}

	public GameObject Play(GameObject source, float pitchOverride, bool createDummySpeaker)
	{
		if (_data.audioSourceData.Count == 0)
		{
			return null;
		}
		float value = Random.value;
		if (value > _data.playbackPercentage)
		{
			return null;
		}
		AudioSource audioSource = GetAudioSource(source, createDummySpeaker);
		if (audioSource == null)
		{
			return null;
		}
		SoundEventAudioSourceData soundEventAudioSourceData = ChooseAudioSourceDataToPlay();
		InitializeAudioSource(soundEventAudioSourceData, audioSource);
		if (pitchOverride != 1f)
		{
			audioSource.pitch = pitchOverride;
		}
		if (audioSource.clip == null)
		{
			Debug.LogError("CANNOT PLAY CLIP FOR '" + audioSource.name + "'... NULL CLIP!");
			return null;
		}
		float length = audioSource.clip.length;
		audioSource.Play();
		GameObject gameObject = ((!createDummySpeaker) ? source : audioSource.gameObject);
		if (!audioSource.loop)
		{
			Object obj = ((!createDummySpeaker) ? ((Object)audioSource) : ((Object)gameObject));
			Object.Destroy(obj, length);
		}
		if (createDummySpeaker)
		{
			gameObject.name = string.Format("Speaker - {0} - {1}", _data.name, source.name);
		}
		return gameObject;
	}

	public void Stop(GameObject source)
	{
		foreach (AudioSource playingSource in _playingSources)
		{
			if (playingSource != null && WasAudioSourceStartedForGameObject(playingSource, source))
			{
				_pausedSounds.Remove(playingSource);
				playingSource.Stop();
			}
		}
		DestroyFinishedAudioSources();
	}

	public void StopAll()
	{
		foreach (AudioSource playingSource in _playingSources)
		{
			if (playingSource != null)
			{
				playingSource.Stop();
			}
		}
		DestroyFinishedAudioSources();
	}

	private bool WasAudioSourceStartedForGameObject(AudioSource audioSource, GameObject gameObject)
	{
		if (audioSource.gameObject == gameObject)
		{
			return true;
		}
		CopyPosition component = audioSource.gameObject.GetComponent<CopyPosition>();
		if (component != null)
		{
			GameObject objectToCopyPositionFrom = component.objectToCopyPositionFrom;
			if (objectToCopyPositionFrom == gameObject)
			{
				return true;
			}
		}
		return false;
	}

	public void HandleGameManagerPauseStackChanged(object sender, PauseStackChangeEventArgs e)
	{
		SoundEventData.PauseBehaviour pauseBehaviour = GetPauseBehaviour(PauseReason.System);
		SoundEventData.PauseBehaviour pauseBehaviour2 = GetPauseBehaviour(PauseReason.Cutscene);
		if (e.WasPush && !IsPaused)
		{
			SoundEventData.PauseBehaviour behaviour = ((e.PauseReason != 0) ? pauseBehaviour : pauseBehaviour2);
			Pause(behaviour);
		}
		else
		{
			if (e.WasPush || !IsPaused)
			{
				return;
			}
			bool flag = GameManager.Instance.IsPauseReasonInStack(PauseReason.System);
			bool flag2 = GameManager.Instance.IsPauseReasonInStack(PauseReason.Cutscene);
			if (!flag && e.PauseReason == PauseReason.System)
			{
				if (!flag2 || pauseBehaviour2 == SoundEventData.PauseBehaviour.Continue)
				{
					UnPause();
				}
			}
			else if (!flag2 && e.PauseReason == PauseReason.Cutscene)
			{
				if (!flag || pauseBehaviour == SoundEventData.PauseBehaviour.Continue)
				{
					UnPause();
				}
				else
				{
					Debug.LogWarning("Can't unpause after cutscene because of system pause. Is this possible?");
				}
			}
		}
	}

	private SoundEventData.PauseBehaviour GetPauseBehaviour(PauseReason reason)
	{
		SoundEventData.PauseBehaviour pauseBehaviour = _data.pauseBehaviour;
		if (reason == PauseReason.Cutscene && _data.overridePauseBehaviourInCutscene)
		{
			pauseBehaviour = _data.cutscenePauseBehaviour;
		}
		if (IsLooping && pauseBehaviour == SoundEventData.PauseBehaviour.Stop)
		{
			pauseBehaviour = SoundEventData.PauseBehaviour.Pause;
		}
		return pauseBehaviour;
	}

	public void Pause(SoundEventData.PauseBehaviour behaviour)
	{
		if (behaviour == SoundEventData.PauseBehaviour.Continue)
		{
			return;
		}
		foreach (AudioSource playingSource in _playingSources)
		{
			if (playingSource != null)
			{
				switch (behaviour)
				{
				case SoundEventData.PauseBehaviour.Pause:
					playingSource.Pause();
					_pausedSounds.Add(playingSource);
					break;
				case SoundEventData.PauseBehaviour.Stop:
					playingSource.Stop();
					break;
				}
			}
		}
		IsPaused = true;
		DestroyFinishedAudioSources();
	}

	public void UnPause()
	{
		foreach (AudioSource pausedSound in _pausedSounds)
		{
			if (pausedSound != null)
			{
				pausedSound.Play();
			}
		}
		_pausedSounds.Clear();
		IsPaused = false;
	}

	private void InitializeAudioSource(SoundEventAudioSourceData soundEventAudioSourceData, AudioSource sourceToInitialize)
	{
		sourceToInitialize.clip = soundEventAudioSourceData.clip;
		sourceToInitialize.mute = soundEventAudioSourceData.mute;
		sourceToInitialize.bypassEffects = soundEventAudioSourceData.bypassEffects;
		sourceToInitialize.playOnAwake = false;
		sourceToInitialize.loop = soundEventAudioSourceData.loop;
		sourceToInitialize.priority = soundEventAudioSourceData.priority;
		sourceToInitialize.volume = _data.GetVolumeAfterRandomness(soundEventAudioSourceData);
		sourceToInitialize.pitch = _data.GetPitchAfterRandomness(soundEventAudioSourceData);
		sourceToInitialize.spatialBlend = soundEventAudioSourceData.panLevel;
		sourceToInitialize.spread = soundEventAudioSourceData.spread;
		sourceToInitialize.dopplerLevel = soundEventAudioSourceData.dopplerLevel;
		sourceToInitialize.minDistance = soundEventAudioSourceData.minDistance;
		sourceToInitialize.maxDistance = soundEventAudioSourceData.maxDistance;
		sourceToInitialize.rolloffMode = ((soundEventAudioSourceData.rolloffMode == SoundEventAudioSourceData.RolloffMode.Linear) ? AudioRolloffMode.Linear : AudioRolloffMode.Logarithmic);
		sourceToInitialize.panStereo = soundEventAudioSourceData.pan2D;
	}

	private SoundEventAudioSourceData ChooseAudioSourceDataToPlay()
	{
		int num = 0;
		switch (_data.nextSoundBehavior)
		{
		case SoundEventData.SoundChoiceBehavior.Random:
			num = Random.Range(0, _data.audioSourceData.Count);
			break;
		case SoundEventData.SoundChoiceBehavior.RandomNoRepeat:
			if (_data.audioSourceData.Count == 1)
			{
				return _data.audioSourceData[0];
			}
			if (_lastSoundIndex == -1)
			{
				goto case SoundEventData.SoundChoiceBehavior.Random;
			}
			num = Random.Range(0, _data.audioSourceData.Count - 1);
			if (num >= _lastSoundIndex)
			{
				num++;
			}
			break;
		case SoundEventData.SoundChoiceBehavior.Sequential:
			num = (_lastSoundIndex + 1) % _data.audioSourceData.Count;
			break;
		}
		_lastSoundIndex = num;
		return _data.audioSourceData[num];
	}

	private void DestroyFinishedAudioSources()
	{
		LinkedListNode<AudioSource> linkedListNode = _playingSources.First;
		while (linkedListNode != null)
		{
			LinkedListNode<AudioSource> next = linkedListNode.Next;
			AudioSource value = linkedListNode.Value;
			if (value == null || (!value.isPlaying && !_pausedSounds.Contains(value)))
			{
				if (value != null)
				{
					if (value.loop && value.gameObject != null && value.gameObject.name.StartsWith("Speaker - "))
					{
						Object.Destroy(value.gameObject);
					}
					else
					{
						Object.Destroy(value);
					}
				}
				_playingSources.Remove(linkedListNode);
			}
			linkedListNode = next;
		}
	}

	private AudioSource GetAudioSource(GameObject source, bool createSpeaker)
	{
		DestroyFinishedAudioSources();
		if (_playingSources.Count >= _data.maxPlaybacks)
		{
			if (_data.playbackLimitBehavior == SoundEventData.LimitBehavior.Skip)
			{
				return null;
			}
			LinkedListNode<AudioSource> first = _playingSources.First;
			first.Value.Stop();
			Object.Destroy(first.Value);
			_playingSources.Remove(first);
		}
		GameObject gameObject = source;
		if (createSpeaker)
		{
			GameObject gameObject2 = new GameObject("Speaker");
			CopyPosition copyPosition = gameObject2.AddComponent<CopyPosition>();
			copyPosition.objectToCopyPositionFrom = source;
			gameObject2.transform.position = source.transform.position;
			gameObject = gameObject2;
		}
		AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
		_playingSources.AddLast(audioSource);
		return audioSource;
	}

	public void SetPitchOfPlayingSounds(float pitch)
	{
		foreach (AudioSource playingSource in _playingSources)
		{
			if (playingSource != null)
			{
				playingSource.pitch = pitch;
			}
		}
	}
}
