using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEventManager : SingletonMonoBehaviour
{
	private Dictionary<SoundEventData, SoundEvent> _prefabToSoundEventMap = new Dictionary<SoundEventData, SoundEvent>();

	private List<SoundEvent> _pausedEvents;

	private bool _muteMusic;

	private bool _muteSoundEffects;

	public VolumeGroup musicVolumeGroup;

	public VolumeGroup musicStingersVolumeGroup;

	public SoundEventData activateLoginSound;

	public static SoundEventManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<SoundEventManager>();
		}
	}

	public bool MuteMusic
	{
		get
		{
			return _muteMusic;
		}
		set
		{
			_muteMusic = value;
			OnMusicMutedChanged();
		}
	}

	public bool MuteSoundEffects
	{
		get
		{
			return _muteSoundEffects;
		}
		set
		{
			_muteSoundEffects = value;
			OnSoundFxMutedChanged();
		}
	}

	public static event EventHandler SoundFxMutedChanged;

	public static event EventHandler MusicMutedChanged;

	public void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void OnEnable()
	{
		GameManager.PauseStackChanged += HandleGameManagerPauseStackChanged;
	}

	public void OnDisable()
	{
		GameManager.PauseStackChanged -= HandleGameManagerPauseStackChanged;
	}

	private void HandleGameManagerPauseStackChanged(object sender, PauseStackChangeEventArgs e)
	{
		StartCoroutine(HandleGameManagerPauseStackChangedDelayed(sender, e));
	}

	private IEnumerator HandleGameManagerPauseStackChangedDelayed(object sender, PauseStackChangeEventArgs e)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		foreach (SoundEvent soundEvent in GetAllSoundEvents())
		{
			soundEvent.HandleGameManagerPauseStackChanged(sender, e);
		}
	}

	public void Play2D(SoundEventData data)
	{
		Play(data, null, 0f, 1f);
	}

	public void Play2D(SoundEventData data, float delay)
	{
		Play(data, null, delay, 1f);
	}

	public void Play(SoundEventData data, GameObject source)
	{
		if (source == null)
		{
			Debug.LogError("Cannot play sound '" + data.ToString() + "' with null object as source.");
		}
		Play(data, source, 0f, 1f);
	}

	public void Play(SoundEventData data, GameObject source, float delay)
	{
		if (source == null)
		{
			Debug.LogError("Cannot play sound '" + data.ToString() + "' with null object as source.");
		}
		Play(data, source, delay, 1f);
	}

	private bool AllowPlayWithCurrentMuteSettings(SoundEventData data)
	{
		if (data == null)
		{
			return true;
		}
		if (_muteMusic && _muteSoundEffects)
		{
			return false;
		}
		if (_muteMusic)
		{
			if (data.volumeGroup == musicVolumeGroup)
			{
				return false;
			}
		}
		else if (_muteSoundEffects && data.volumeGroup != musicVolumeGroup && data.volumeGroup != musicStingersVolumeGroup)
		{
			return false;
		}
		return true;
	}

	public void Play(SoundEventData data, GameObject source, float delay, float pitch)
	{
		if (AllowPlayWithCurrentMuteSettings(data))
		{
			if (source == null)
			{
				source = base.gameObject;
			}
			if (delay != 0f)
			{
				StartCoroutine(PlayWithDelay(data, source, delay, pitch));
				return;
			}
			if (data == null)
			{
				string message = string.Format("No sound assigned while trying to play sound on '{0}'", source.name);
				Debug.LogWarning(message);
				return;
			}
			SoundEvent orCreateSoundEvent = GetOrCreateSoundEvent(data);
			bool createDummySpeaker = source != base.gameObject;
			float pitchOverride = ((!(Time.timeScale > 0f) || !data.effectedByTimeScale) ? pitch : (pitch * Time.timeScale));
			orCreateSoundEvent.Play(source, pitchOverride, createDummySpeaker);
		}
	}

	public void PlayNoDestoryOnLoad(SoundEventData data)
	{
		if (!AllowPlayWithCurrentMuteSettings(data))
		{
			return;
		}
		if (data == null)
		{
			Debug.LogError("Cannot play null sound..");
			return;
		}
		SoundEvent orCreateSoundEvent = GetOrCreateSoundEvent(data);
		GameObject gameObject = orCreateSoundEvent.Play(base.gameObject, 1f, true);
		if (gameObject != null)
		{
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}

	private IEnumerator PlayWithDelay(SoundEventData data, GameObject source, float delay, float pitch)
	{
		yield return new WaitForSeconds(delay);
		Play(data, source, 0f, pitch);
	}

	public void Stop2D(SoundEventData data)
	{
		Stop(data, base.gameObject);
	}

	public void Stop(SoundEventData data, GameObject source)
	{
		SoundEvent value;
		if (!(data == null) && _prefabToSoundEventMap.TryGetValue(data, out value))
		{
			value.Stop(source);
		}
	}

	public IEnumerable<SoundEvent> GetAllSoundEvents()
	{
		foreach (SoundEvent value in _prefabToSoundEventMap.Values)
		{
			yield return value;
		}
	}

	private SoundEvent GetOrCreateSoundEvent(SoundEventData data)
	{
		SoundEvent value;
		if (!_prefabToSoundEventMap.TryGetValue(data, out value))
		{
			value = new SoundEvent(data);
			if (value == null)
			{
				Debug.LogError("Unable to create sound - despite the lack of exceptions. Why you no instantiate SoundEvent?");
			}
			_prefabToSoundEventMap.Add(data, value);
		}
		return value;
	}

	private void OnSoundFxMutedChanged()
	{
		ReEvaluatePlayingSoundsOnMuteChange();
		if (SoundEventManager.SoundFxMutedChanged != null)
		{
			SoundEventManager.SoundFxMutedChanged(this, new EventArgs());
		}
	}

	private void OnMusicMutedChanged()
	{
		ReEvaluatePlayingSoundsOnMuteChange();
		if (SoundEventManager.MusicMutedChanged != null)
		{
			SoundEventManager.MusicMutedChanged(this, new EventArgs());
		}
	}

	private void ReEvaluatePlayingSoundsOnMuteChange()
	{
		if (!MuteMusic && !MuteSoundEffects)
		{
			return;
		}
		foreach (KeyValuePair<SoundEventData, SoundEvent> item in _prefabToSoundEventMap)
		{
			SoundEventData key = item.Key;
			SoundEvent value = item.Value;
			if (!AllowPlayWithCurrentMuteSettings(key))
			{
				value.StopAll();
			}
		}
	}

	public void SetPitchOfPlayingSounds(SoundEventData data, float pitch)
	{
		SoundEvent value;
		if (_prefabToSoundEventMap.TryGetValue(data, out value))
		{
			value.SetPitchOfPlayingSounds(pitch);
		}
	}
}