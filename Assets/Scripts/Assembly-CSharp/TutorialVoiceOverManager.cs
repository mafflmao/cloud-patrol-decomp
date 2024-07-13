using System.Collections;
using UnityEngine;

public class TutorialVoiceOverManager : SingletonMonoBehaviour
{
	public SoundEventData TapTrolls;

	public SoundEventData SlideTrolls;

	public SoundEventData TouchCoins;

	public SoundEventData TargetMaxCombo;

	public SoundEventData TouchCoinsMaxCombo;

	public SoundEventData TouchMagicItem;

	public SoundEventData ActivateMagicItem;

	public SoundEventData Outtro;

	public SoundEventData NegativeFeedback;

	public SoundEventData WhyTouch;

	public SoundEventData WhySwipe;

	public SoundEventData WhyMagicItems;

	private SoundEventData _currentlyPlayingVo;

	public static TutorialVoiceOverManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<TutorialVoiceOverManager>();
		}
	}

	public void PlayTapTrolls()
	{
		Play(TapTrolls, true);
	}

	public void PlaySlideTrolls()
	{
		Play(SlideTrolls, true);
	}

	public void PlayTouchCoins()
	{
		Play(TouchCoins, true);
	}

	public void PlayTargetMaxCombo()
	{
		Play(TargetMaxCombo, true);
	}

	public void PlayTouchCoinsMaxCombo()
	{
		Play(TouchCoinsMaxCombo, true);
	}

	public void PlayTouchMagicItem()
	{
		Play(TouchMagicItem, true);
	}

	public void PlayActivateMagicItem()
	{
		Play(ActivateMagicItem, true);
	}

	public void PlayOuttro()
	{
		Play(Outtro, true);
	}

	public void PlayNegativeFeedback()
	{
		Play(NegativeFeedback, false);
	}

	public void PlayWhyTouch()
	{
		Play(WhyTouch, true);
	}

	public void PlayWhySwipe()
	{
		Play(WhySwipe, true);
	}

	public void PlayWhyMagicItems()
	{
		Play(WhyMagicItems, true);
	}

	private void Play(SoundEventData soundEventData, bool stopPlayingSound)
	{
		if (LocalizationManager.Instance.IsEnglish)
		{
			if (stopPlayingSound)
			{
				Stop();
				StartCoroutine(PlayDelayed(soundEventData, 0.2f));
				_currentlyPlayingVo = soundEventData;
			}
			else if (_currentlyPlayingVo == null)
			{
				Stop();
				StartCoroutine(PlayDelayed(soundEventData, 0.2f));
				_currentlyPlayingVo = soundEventData;
			}
		}
	}

	private void SoundComplete()
	{
		_currentlyPlayingVo = null;
	}

	private IEnumerator PlayDelayed(SoundEventData soundEventData, float delay)
	{
		yield return new WaitForSeconds(delay);
		float clipLength2 = 0f;
		float maxLength = 0f;
		foreach (SoundEventAudioSourceData asd in soundEventData.audioSourceData)
		{
			if (asd.clip.length > maxLength)
			{
				maxLength = asd.clip.length;
			}
		}
		clipLength2 = maxLength;
		InvokeHelper.InvokeSafe(SoundComplete, clipLength2, this);
		SoundEventManager.Instance.Play2D(soundEventData);
	}

	public void Stop()
	{
		StopAllCoroutines();
		if (_currentlyPlayingVo != null)
		{
			SoundEventManager.Instance.Stop2D(_currentlyPlayingVo);
		}
		_currentlyPlayingVo = null;
	}
}
