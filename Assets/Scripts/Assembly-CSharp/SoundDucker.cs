using UnityEngine;

public class SoundDucker : MonoBehaviour
{
	private enum FadingStates
	{
		Static = 0,
		FadingOut = 1,
		FadingIn = 2
	}

	private AudioSource _audioSource;

	private float _unduckedVolume;

	private FadingStates _state = FadingStates.FadingIn;

	private float _fadeStart;

	private float _fadeEnd;

	private float _totalFadeTime;

	private float _fadeTimeRemaining;

	public void StartDucking(float duckedVolume, float fadeInTimeInSeconds)
	{
		if (!DestroyIfAudioSourceIsDestroyed())
		{
			_state = FadingStates.FadingOut;
			_totalFadeTime = fadeInTimeInSeconds;
			_fadeTimeRemaining = fadeInTimeInSeconds;
			_fadeStart = _audioSource.volume;
			_fadeEnd = duckedVolume;
		}
	}

	public void StopDucking(float fadeOutTimeInSeconds)
	{
		if (!DestroyIfAudioSourceIsDestroyed())
		{
			_state = FadingStates.FadingIn;
			_totalFadeTime = fadeOutTimeInSeconds;
			_fadeTimeRemaining = fadeOutTimeInSeconds;
			_fadeStart = _audioSource.volume;
			_fadeEnd = _unduckedVolume;
		}
	}

	private bool DestroyIfAudioSourceIsDestroyed()
	{
		if (_audioSource == null)
		{
			Object.Destroy(this);
			return true;
		}
		return false;
	}

	private void Update()
	{
		if (DestroyIfAudioSourceIsDestroyed() || _state == FadingStates.Static)
		{
			return;
		}
		_fadeTimeRemaining -= Time.deltaTime;
		if (_fadeTimeRemaining <= 0f)
		{
			_fadeTimeRemaining = 0f;
			_audioSource.volume = _fadeEnd;
			if (_state == FadingStates.FadingIn)
			{
				Object.Destroy(this);
			}
			else
			{
				_state = FadingStates.Static;
			}
		}
		else
		{
			float num = (_totalFadeTime - _fadeTimeRemaining) / _totalFadeTime;
			float num2 = _fadeEnd - _fadeStart;
			_audioSource.volume = _fadeStart + num2 * num;
		}
	}

	public static void StartDucking(AudioSource source, float duckedVolume, float fadeTimeInSeconds)
	{
		GetDucker(source, true).StartDucking(duckedVolume, fadeTimeInSeconds);
	}

	public static void StopDucking(AudioSource source, float fadeTimeInSeconds)
	{
		SoundDucker ducker = GetDucker(source, false);
		if (ducker != null)
		{
			ducker.StopDucking(fadeTimeInSeconds);
		}
	}

	private static SoundDucker GetDucker(AudioSource source, bool autoCreate)
	{
		SoundDucker[] components = source.gameObject.GetComponents<SoundDucker>();
		foreach (SoundDucker soundDucker in components)
		{
			if (soundDucker._audioSource == source)
			{
				return soundDucker;
			}
		}
		if (autoCreate)
		{
			SoundDucker soundDucker2 = source.gameObject.AddComponent<SoundDucker>();
			soundDucker2._audioSource = source;
			soundDucker2._unduckedVolume = source.volume;
			return soundDucker2;
		}
		return null;
	}
}
