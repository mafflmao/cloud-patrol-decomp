using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerupHolder : MonoBehaviour
{
	public Vector3 punchScaleAmount = new Vector3(1f, 1f, 1f);

	public float punchScaleTime = 0.5f;

	public float loopingPulseTime = 0.33f;

	public Color originalColor = new Color(0.61960787f, 0.61960787f, 0.61960787f, 1f);

	public Color pulseColor = new Color(1f, 0.52156866f, 1f, 1f);

	public Animation cooldownAnimation;

	public GameObject readyParticle;

	public SoundEventData readySFX;

	public SoundEventData activateSFX;

	public SoundEventData deactivateSFX;

	private List<Powerup> _powerupInstance;

	private PowerupData _powerupData;

	private bool _powerupDataIsBonus;

	private PowerupData _queuedPowerupData;

	private bool _queuedPowerupIsBonus;

	private AnimationState _cooldownAnimation;

	private PowerupStates _state = PowerupStates.hidden;

	private bool _allowInput = true;

	public List<Powerup> OverridePowerup
	{
		set
		{
			_powerupInstance = value;
		}
	}

	public string EquippedPowerupName
	{
		get
		{
			return (!(_powerupData == null)) ? _powerupData.LocalizedName : "none";
		}
	}

	public DateTime LastQueuedDateTime { get; private set; }

	public bool IsUsable
	{
		get
		{
			if (State == PowerupStates.ready)
			{
				return true;
			}
			return false;
		}
	}

	public PowerupData CurrentPowerup
	{
		get
		{
			return _powerupData;
		}
	}

	public PowerupStates State
	{
		get
		{
			return _state;
		}
		private set
		{
			PowerupStates state = _state;
			if (state != value)
			{
				Debug.Log(string.Concat(base.name, " state changed from - ", state, " to ", value));
				_state = value;
				OnStateChanged(state);
			}
		}
	}

	private bool HasPowerupSet
	{
		get
		{
			return _powerupData != null || _queuedPowerupData != null;
		}
	}

	public static event EventHandler<PowerupStateChangeEventArgs> StateChanged;

	public void Start()
	{
		cooldownAnimation.Play("HUD_MagicItemCount_Down");
		_cooldownAnimation = cooldownAnimation["HUD_MagicItemCount_Down"];
		_cooldownAnimation.speed = 0f;
		_cooldownAnimation.wrapMode = WrapMode.Loop;
		UpdateVisualsFromState();
	}

	private void OnEnable()
	{
		Powerup.Finished += HandlePowerupInstanceFinished;
		GameManager.GameOver += HandleGameManagerGameOver;
		GameManager.GameStateChanged += HandleGameManagerGameStateChanged;
		FingerGestures.OnFingerDown += HandleTouchScreenGesturesOnFingerDown;
		PowerupHolder.StateChanged = (EventHandler<PowerupStateChangeEventArgs>)Delegate.Combine(PowerupHolder.StateChanged, new EventHandler<PowerupStateChangeEventArgs>(HandlePowerupHolderStateChanged));
	}

	private void OnDisable()
	{
		Powerup.Finished -= HandlePowerupInstanceFinished;
		GameManager.GameOver -= HandleGameManagerGameOver;
		GameManager.GameStateChanged -= HandleGameManagerGameStateChanged;
		FingerGestures.OnFingerDown -= HandleTouchScreenGesturesOnFingerDown;
		PowerupHolder.StateChanged = (EventHandler<PowerupStateChangeEventArgs>)Delegate.Remove(PowerupHolder.StateChanged, new EventHandler<PowerupStateChangeEventArgs>(HandlePowerupHolderStateChanged));
		if (SoundEventManager.Instance != null)
		{
			SoundEventManager.Instance.Stop(activateSFX, base.gameObject);
			SoundEventManager.Instance.Stop(deactivateSFX, base.gameObject);
			SoundEventManager.Instance.Stop(readySFX, base.gameObject);
		}
	}

	private void OnStateChanged(PowerupStates oldState)
	{
		UpdateVisualsFromState();
		if (PowerupHolder.StateChanged != null)
		{
			PowerupHolder.StateChanged(this, new PowerupStateChangeEventArgs(oldState));
		}
	}

	private void HandleGameManagerGameOver(object sender, EventArgs e)
	{
		State = PowerupStates.hidden;
		base.enabled = false;
		_powerupInstance = null;
		_powerupData = null;
		_powerupDataIsBonus = false;
		_queuedPowerupData = null;
		_queuedPowerupIsBonus = false;
	}

	private void HandleTouchScreenGesturesOnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		if (!_allowInput || fingerIndex != 0 || State != 0 || HealingElixirScreen.IsActive || RocketBooster.IsActive || GameManager.gameState != GameManager.GameState.Playing || GameManager.Instance.IsPaused)
		{
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(fingerPos);
		RaycastHit[] array = Physics.RaycastAll(ray);
		RaycastHit[] array2 = array;
		foreach (RaycastHit raycastHit in array2)
		{
			if (raycastHit.collider == base.GetComponent<Collider>())
			{
				ActivatePowerup();
				break;
			}
		}
	}

	public void ActivatePowerup()
	{
		State = PowerupStates.active;
		if ((bool)activateSFX)
		{
			SoundEventManager.Instance.Play(activateSFX, base.gameObject);
		}
		iTween.PunchScale(base.gameObject, iTween.Hash("amount", punchScaleAmount, "time", punchScaleTime / 2f, "ignoretimescale", true, "oncomplete", "StartPulsing"));
		_powerupInstance = _powerupData.Trigger(this, _powerupDataIsBonus);
		SwrveEventsGameplay.MagicItemActivated(_powerupData.storageKey, _powerupData.GetLevel());
	}

	private void StartPulsing()
	{
		iTween.ScaleBy(base.gameObject, iTween.Hash("amount", new Vector3(1.25f, 1.25f, 1.25f), "time", loopingPulseTime, "loopType", iTween.LoopType.pingPong, "easeType", "easeInOutQuad"));
		iTween.ColorTo(base.gameObject, iTween.Hash("color", pulseColor, "time", loopingPulseTime, "loopType", iTween.LoopType.pingPong, "easeType", "easeInOutQuad"));
	}

	private void DoCollectionPulse()
	{
		iTween.PunchScale(base.gameObject, iTween.Hash("amount", punchScaleAmount, "time", punchScaleTime));
		if ((bool)readyParticle)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(readyParticle, base.transform.position, base.transform.rotation) as GameObject;
			gameObject.transform.parent = base.transform;
		}
		if ((bool)readySFX)
		{
			SoundEventManager.Instance.Play2D(readySFX);
		}
	}

	private void UpdateVisualsFromState()
	{
		cooldownAnimation.GetComponent<Renderer>().enabled = false;
		switch (State)
		{
		case PowerupStates.ready:
			StopActivePulse();
			base.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
			SetVisible(true);
			break;
		case PowerupStates.active:
			SetVisible(true);
			base.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
			break;
		case PowerupStates.disabled:
			SetVisible(true);
			base.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 1f));
			base.GetComponent<Collider>().enabled = false;
			break;
		default:
			StopActivePulse();
			SetVisible(false);
			break;
		}
	}

	private void StopActivePulse()
	{
		iTween.Stop(base.gameObject);
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		base.gameObject.GetComponent<Renderer>().material.color = originalColor;
	}

	public void QueuePowerup(PowerupData powerup, bool isBonus)
	{
		LastQueuedDateTime = DateTime.Now;
		if (State != PowerupStates.active)
		{
			base.GetComponent<Renderer>().material.mainTexture = powerup.inGameButtonTexture;
			_powerupData = powerup;
			_powerupDataIsBonus = isBonus;
			_queuedPowerupData = null;
			_queuedPowerupIsBonus = false;
			State = (MagicItemManager.IsMagicItemActive ? PowerupStates.disabled : PowerupStates.ready);
			DoCollectionPulse();
		}
		else
		{
			_queuedPowerupData = powerup;
			_queuedPowerupIsBonus = isBonus;
		}
	}

	public void UpdateTime(float percentLeft)
	{
		_cooldownAnimation.normalizedTime = Mathf.Clamp01(percentLeft);
		cooldownAnimation.GetComponent<Renderer>().enabled = true;
	}

	public void Update()
	{
		if (State != PowerupStates.active || _powerupInstance == null)
		{
			return;
		}
		bool flag = false;
		float num = 999f;
		for (int i = 0; i < _powerupInstance.Count && !_powerupInstance[i].manuallyUpdatesProgress; i++)
		{
			if (_powerupInstance[i].TimeLeft != 0f && _powerupInstance[i].TimeLeft < num)
			{
				num = _powerupInstance[i].TimeLeft;
				flag = true;
			}
		}
		if (flag)
		{
			UpdateTime(1f - num / _powerupInstance[0].lifeTimeInSeconds);
		}
	}

	public void SetVisible(bool isVisible)
	{
		base.GetComponent<Collider>().enabled = isVisible;
		base.GetComponent<Renderer>().enabled = isVisible;
		cooldownAnimation.GetComponent<Renderer>().enabled = false;
	}

	private void HandlePowerupHolderStateChanged(object sender, PowerupStateChangeEventArgs e)
	{
		PowerupHolder powerupHolder = (PowerupHolder)sender;
		if (powerupHolder != this && State != PowerupStates.hidden)
		{
			if (e.OldState == PowerupStates.active)
			{
				State = ((!HasPowerupSet) ? PowerupStates.hidden : PowerupStates.ready);
			}
			else if (powerupHolder.State == PowerupStates.active)
			{
				State = PowerupStates.disabled;
			}
		}
	}

	private void HandleGameManagerGameStateChanged(object sender, GameManager.GameStateChangedEventArgs e)
	{
		_allowInput = e.NewState == GameManager.GameState.Playing;
		if (e.NewState != GameManager.GameState.Playing || _powerupInstance == null)
		{
			return;
		}
		int num = 0;
		while (_powerupInstance != null && num < _powerupInstance.Count)
		{
			if (_powerupInstance[num].IsTriggered)
			{
				_powerupInstance[num].DestroyAndFinish(true);
			}
			num++;
		}
	}

	private void HandlePowerupInstanceFinished(object sender, EventArgs e)
	{
		if (_powerupInstance != null && _powerupInstance.Contains((Powerup)sender))
		{
			ClearCurrentMagicItem(true);
		}
		else if (sender is RocketBooster && State == PowerupStates.disabled)
		{
			State = PowerupStates.ready;
		}
		_powerupInstance = null;
	}

	private void ClearCurrentMagicItem(bool playDeactivateSfx)
	{
		State = PowerupStates.hidden;
		if (_queuedPowerupData != null)
		{
			QueuePowerup(_queuedPowerupData, _queuedPowerupIsBonus);
		}
		else if (!GameManager.Instance.IsGameOver && !HealingElixirScreen.IsActive && playDeactivateSfx)
		{
			SoundEventManager.Instance.Play(deactivateSFX, base.gameObject);
		}
	}
}
