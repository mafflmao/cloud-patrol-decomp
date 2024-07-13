using System;
using System.Collections;
using UnityEngine;

public class TimeTwister : Powerup
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(TimeTwister), LogLevel.Debug);

	public float timeSlowdown = 0.25f;

	public GameObject activeTimeParticle;

	public SoundEventData sfxIn2D;

	public SoundEventData sfxOut2D;

	public static bool IsActive = false;

	private GameObject _timeParticle;

	private float _originalFixedDelta;

	private SoundEventData _slowedMusic;

	private static float releaseTime = 2f;

	protected override void HandleTriggered()
	{
		base.HandleTriggered();
		_originalFixedDelta = Time.fixedDeltaTime;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		FingerGestures.OnFingerDown += HandleFingerGesturesOnFingerDown;
		FingerGestures.OnFingerUp += HandleFingerGesturesOnFingerUp;
		GameManager.PlayerTookDamage += HandlePlayerTookDamage;
		BombController.BombControllerStarted += HandleBombControllerStarted;
		Shooter.ComboCompleted += HandleShooting;
		GameManager.GameStateChanged += HandleGameManagerGameStateChanged;
		GameManager.PauseStackChanged += HandleGameManagerPauseStackChanged;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		IsActive = false;
		FingerGestures.OnFingerDown -= HandleFingerGesturesOnFingerDown;
		FingerGestures.OnFingerUp -= HandleFingerGesturesOnFingerUp;
		GameManager.PlayerTookDamage -= HandlePlayerTookDamage;
		BombController.BombControllerStarted -= HandleBombControllerStarted;
		Shooter.ComboCompleted -= HandleShooting;
		GameManager.GameStateChanged -= HandleGameManagerGameStateChanged;
		GameManager.PauseStackChanged -= HandleGameManagerPauseStackChanged;
	}

	private void HandleShooting(object sender, Shooter.ComboCompletedEventArgs args)
	{
		base.TimeLeft = Mathf.Clamp(base.TimeLeft - releaseTime, 0f, lifeTimeInSeconds);
	}

	protected override void Update()
	{
		_timeLastFrame = Time.realtimeSinceStartup;
		if (base.IsTriggered && base.TimeLeft == 0f)
		{
			DestroyAndFinish(true);
		}
	}

	public override void DestroyAndFinish(bool waitForCutscene)
	{
		RestoreTime();
		StopAllCoroutines();
		base.DestroyAndFinish(waitForCutscene);
	}

	private void HandleFingerGesturesOnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		_log.LogDebug("HandleFingerGesturesOnFingerDown");
		if (Time.timeScale != timeSlowdown && !GameManager.Instance.IsPaused && GameManager.gameState == GameManager.GameState.Playing)
		{
			SlowTime();
		}
	}

	private void HandleFingerGesturesOnFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
	{
		_log.LogDebug("HandleFingerGesturesOnFingerUp");
		if (Time.timeScale != 1f && !GameManager.Instance.IsPaused && GameManager.gameState == GameManager.GameState.Playing)
		{
			RestoreTime();
		}
		else if (GameManager.Instance.IsPaused)
		{
			UnityEngine.Object.Destroy(_timeParticle);
		}
	}

	private void HandleBombControllerStarted(object sender, EventArgs args)
	{
		_log.LogDebug("HandleBombControllerStarted");
		DestroyAndFinish(false);
	}

	private void HandlePlayerTookDamage(object sender, EventArgs args)
	{
		_log.LogDebug("HandlePlayerTookDamage");
		RestoreTime();
	}

	private void HandleGameManagerGameStateChanged(object sender, GameManager.GameStateChangedEventArgs e)
	{
		_log.LogDebug("HandleGameManagerGameStateChanged");
		if (IsActive)
		{
			RestoreTime();
		}
	}

	private void HandleGameManagerPauseStackChanged(object sender, PauseStackChangeEventArgs e)
	{
		_log.LogDebug("HandleGameManagerPauseStackChanged");
		if (e.WasPush && IsActive)
		{
			RestoreTime();
		}
	}

	private void SlowTime()
	{
		_log.Log("Slowing time..");
		Time.timeScale = timeSlowdown;
		Time.fixedDeltaTime = _originalFixedDelta * timeSlowdown;
		_slowedMusic = MusicManager.Instance.CurrentMusic;
		if (_slowedMusic != null)
		{
			SoundEventManager.Instance.SetPitchOfPlayingSounds(_slowedMusic, timeSlowdown);
		}
		SoundEventManager.Instance.Stop2D(sfxOut2D);
		SoundEventManager.Instance.Play2D(sfxIn2D);
		StartCoroutine(TimeShiftFX());
		_timeLastFrame = Time.realtimeSinceStartup;
		IsActive = true;
	}

	private void RestoreTime()
	{
		_log.LogDebug("Restoring Time!");
		Time.timeScale = 1f;
		Time.fixedDeltaTime = _originalFixedDelta;
		SoundEventManager.Instance.Stop2D(sfxIn2D);
		SoundEventManager.Instance.Play2D(sfxOut2D);
		if (_slowedMusic != null)
		{
			SoundEventManager.Instance.SetPitchOfPlayingSounds(_slowedMusic, 1f);
		}
		StopAllCoroutines();
		IsActive = false;
	}

	private IEnumerator TimeShiftFX()
	{
		yield return new WaitForEndOfFrame();
		while (IsActive)
		{
			if (_timeParticle == null && !GameManager.Instance.IsPaused && GameManager.gameState == GameManager.GameState.Playing)
			{
				_log.LogDebug("Spawning Time Particle.");
				_timeParticle = (GameObject)UnityEngine.Object.Instantiate(activeTimeParticle, ShipManager.instance.dragMultiTarget[m_DragMultiTargetIndex].transform.position, Quaternion.identity);
			}
			yield return new WaitForSeconds(0.125f);
		}
	}
}
