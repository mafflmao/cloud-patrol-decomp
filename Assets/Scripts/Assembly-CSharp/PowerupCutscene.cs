using System;
using UnityEngine;

public class PowerupCutscene : MonoBehaviour
{
	public ParticleSystem cutsceneParticle;

	public float particleTriggerTime = 0.25f;

	public float powerTriggerTime = 0.5f;

	public SoundEventData magicItemSound;

	public float soundDelayTime;

	private float _deltaTime = 0.02f;

	private float _nextUpdateTime;

	private bool _cutsceneEnded;

	private bool _cutscenePaused;

	private bool _visible = true;

	private float _animTime;

	private bool _powerTriggered;

	private AnimationState _animState;

	public static event EventHandler<EventArgs> CutsceneStarted;

	public static event EventHandler<EventArgs> TriggerPower;

	public static event EventHandler<EventArgs> Completed;

	public virtual void Start()
	{
		_animState = base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name];
		SetVisibleChildren(false);
		base.transform.position = Camera.main.transform.position + new Vector3(0f, 0f, -2f);
		GameManager.Instance.PushPause(PauseReason.Cutscene);
		GameObjectUtils.SetLayerRecursive(base.gameObject, LayerMask.NameToLayer("LitHUD"));
		OnCutsceneStarted();
	}

	protected virtual void OnEnable()
	{
		GameManager.PauseStackChanged += HandleGameManagerPauseStackChanged;
	}

	protected virtual void OnDisable()
	{
		GameManager.PauseStackChanged -= HandleGameManagerPauseStackChanged;
		if (!_powerTriggered)
		{
			OnTriggerPower();
		}
	}

	private void HandleGameManagerPauseStackChanged(object sender, PauseStackChangeEventArgs e)
	{
		if (GameManager.Instance.IsPaused && GameManager.Instance.IsPauseReasonInStack(PauseReason.Cutscene) && GameManager.Instance.LastPauseReason != 0 && !_cutsceneEnded && _visible)
		{
			SetVisibleChildren(false);
		}
	}

	public void SetPaused(bool toggle)
	{
		_cutscenePaused = toggle;
		_nextUpdateTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		if (_cutsceneEnded || _cutscenePaused || _nextUpdateTime > Time.realtimeSinceStartup)
		{
			return;
		}
		_nextUpdateTime = Time.realtimeSinceStartup + _deltaTime;
		if (!GameManager.Instance.IsPaused || GameManager.Instance.LastPauseReason == PauseReason.Cutscene)
		{
			if (!_visible)
			{
				SetVisibleChildren(true);
				SoundEventManager.Instance.Play2D(magicItemSound, soundDelayTime);
			}
			UpdateAnimation();
			if (_animState.time > particleTriggerTime)
			{
				UpdateParticles();
			}
			if (!_powerTriggered && _animState.time > powerTriggerTime)
			{
				OnTriggerPower();
			}
		}
	}

	private void OnCutsceneStarted()
	{
		if (PowerupCutscene.CutsceneStarted != null)
		{
			PowerupCutscene.CutsceneStarted(this, new EventArgs());
		}
	}

	private void OnTriggerPower()
	{
		_powerTriggered = true;
		GameManager.Instance.PopPause(PauseReason.Cutscene);
		if (PowerupCutscene.TriggerPower != null)
		{
			PowerupCutscene.TriggerPower(this, new EventArgs());
		}
	}

	private void OnCompleted()
	{
		_cutsceneEnded = true;
		base.enabled = false;
		SetVisibleChildren(false);
		if (PowerupCutscene.Completed != null)
		{
			PowerupCutscene.Completed(this, new EventArgs());
		}
	}

	protected void SetVisibleChildren(bool isVisible)
	{
		_visible = isVisible;
		if (isVisible)
		{
			GameObjectUtils.ShowObject(base.gameObject);
		}
		else
		{
			GameObjectUtils.HideObject(base.gameObject);
		}
	}

	private void UpdateAnimation()
	{
		if (!base.GetComponent<Animation>().isPlaying)
		{
			base.GetComponent<Animation>().Play();
		}
		_animState.time = _animTime;
		_animTime += _deltaTime;
		if (_animTime >= _animState.length)
		{
			Debug.Log("Cutecene ending");
			base.GetComponent<Animation>().Stop();
			OnCompleted();
		}
	}

	private void UpdateParticles()
	{
		if (!(cutsceneParticle == null))
		{
			cutsceneParticle.Simulate(_deltaTime);
		}
	}
}
