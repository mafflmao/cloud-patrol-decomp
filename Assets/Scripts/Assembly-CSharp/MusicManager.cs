using System;
using System.Collections;
using UnityEngine;

public class MusicManager : SingletonMonoBehaviour
{
	public SoundEventData[] music;

	public SoundEventData TitleMusic;

	public SoundEventData MenuLoop;

	public SoundEventData[] bossMusic;

	public SoundEventData GameOverMusic;

	public SoundEventData idleMusic;

	public SoundEventData bonusMusic;

	public SoundEventData windMusic;

	public SoundEventData introMusic;

	public float GameOverMusicDelay = 4.7f;

	public SoundEventData GameOverStinger;

	private SoundEventData _currentMusic;

	private int _gameplayMusicIndex;

	private int _bossMusicIndex;

	public static MusicManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<MusicManager>();
		}
	}

	public SoundEventData CurrentMusic
	{
		get
		{
			return _currentMusic;
		}
	}

	protected override void AwakeOnce()
	{
		base.AwakeOnce();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void OnEnable()
	{
		GameManager.GameStarted += HandleGameStarted;
		SoundEventManager.MusicMutedChanged += HandleMusicMutedChanged;
	}

	private void OnDisable()
	{
		GameManager.GameStarted -= HandleGameStarted;
		SoundEventManager.MusicMutedChanged -= HandleMusicMutedChanged;
	}

	private void HandleMusicMutedChanged(object sender, EventArgs e)
	{
		if (!SoundEventManager.Instance.MuteMusic || !SoundEventManager.Instance.MuteSoundEffects)
		{
			StopMusic_Internal();
			PlayTitleMusic();
		}
	}

	private void HandleGameStarted(object sender, EventArgs e)
	{
	}

	public void PlayGameOverStingerAndMusic()
	{
		StopMusic_Internal();
		SoundEventManager.Instance.Play2D(GameOverStinger);
		InvokeHelper.InvokeSafe(PlayEndGameMusic, GameOverMusicDelay, this);
	}

	public void StopMusic()
	{
		if (!(_currentMusic == idleMusic))
		{
			StopMusic_Internal();
			SoundEventManager.Instance.Play2D(idleMusic);
			_currentMusic = idleMusic;
		}
	}

	public void StopCurrentMusic()
	{
		StopMusic_Internal();
	}

	public void StopMusicAndDontStartTheWind()
	{
	}

	public void StopMusic_Internal()
	{
		SoundEventManager.Instance.Stop2D(_currentMusic);
		_currentMusic = null;
	}

	public void PlayNextGameplayMusic()
	{
	}

	public void PlayCurrentGameplayMusic()
	{
	}

	public void PlayNextBossMusic()
	{
	}

	public void PlayCurrentBossMusic()
	{
	}

	public void PlayTitleMusic()
	{
		if (_currentMusic != TitleMusic)
		{
			Play(TitleMusic);
			StartCoroutine(PlayMenuLoopDelayed(TitleMusic.audioSourceData[0].clip.length));
		}
	}

	private void CancelPendingMusicChanges()
	{
		StopAllCoroutines();
	}

	public IEnumerator PlayMenuLoopDelayed(float time)
	{
		yield return new WaitForSeconds(time);
	}

	public void PlayMenuLoop()
	{
	}

	public void PlayEndGameMusic()
	{
		Play(GameOverMusic);
	}

	public void PlayBonusMusic()
	{
	}

	public void PlayIntroMusic()
	{
		Play(introMusic);
	}

	private void Play(SoundEventData musicData)
	{
		Debug.Log("Playing - " + musicData);
		CancelPendingMusicChanges();
		StopMusic_Internal();
		SoundEventManager.Instance.PlayNoDestoryOnLoad(musicData);
		_currentMusic = musicData;
	}
}