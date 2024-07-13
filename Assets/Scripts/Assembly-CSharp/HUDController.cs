using System;
using UnityEngine;

public class HUDController : MonoBehaviour
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(HUDController), LogLevel.Log);

	public Renderer[] renderers;

	public bool?[] visibilityStates;

	public StateRoot stateRoot;

	public Animation m_WhiteFlashAnim;

	private void Start()
	{
		renderers = GetComponentsInChildren<Renderer>();
		visibilityStates = new bool?[renderers.Length];
	}

	private void OnEnable()
	{
		GameManager.PlayerTookDamage += HandleGameManagerPlayerTookDamage;
		GameManager.Revived += HandleGameManagerRevived;
		GameManager.PauseChanged += HandleGameManagerPauseChanged;
		PowerupCutscene.CutsceneStarted += HandlePowerupCutsceneStarted;
		PowerupCutscene.Completed += HandlePowerupCutsceneEnded;
		Hazard.HazardHurtPlayer += HandleHazardHazardHurtPlayer;
	}

	private void OnDisable()
	{
		GameManager.PlayerTookDamage -= HandleGameManagerPlayerTookDamage;
		GameManager.Revived -= HandleGameManagerRevived;
		GameManager.PauseChanged -= HandleGameManagerPauseChanged;
		PowerupCutscene.CutsceneStarted -= HandlePowerupCutsceneStarted;
		PowerupCutscene.Completed -= HandlePowerupCutsceneEnded;
		Hazard.HazardHurtPlayer -= HandleHazardHazardHurtPlayer;
	}

	private void HandlePowerupCutsceneStarted(object sender, EventArgs e)
	{
		HideHUD();
	}

	private void HandlePowerupCutsceneEnded(object sender, EventArgs e)
	{
		ShowHUD();
	}

	private void HideHUD()
	{
		_log.LogDebug("HideHUD()");
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			if (!(renderer == null) && !visibilityStates[i].HasValue)
			{
				_log.LogDebug("Saving '{0}' state ({1})", renderer.name, renderer.enabled);
				visibilityStates[i] = renderer.enabled;
				renderer.enabled = false;
			}
		}
	}

	private void ShowHUD()
	{
		_log.LogDebug("ShowHUD()");
		for (int i = 0; i < renderers.Length; i++)
		{
			bool? flag = visibilityStates[i];
			Renderer renderer = renderers[i];
			if (!(renderer == null) && flag.HasValue)
			{
				_log.LogDebug("Restoring '{0}' to state '{1}'", renderer.name, flag.Value);
				renderer.enabled = flag.Value;
			}
		}
		ClearVisibilityStates();
		ScoreKeeper.Instance.UpdateCombinedMultiplier();
	}

	private void ClearVisibilityStates()
	{
		_log.LogDebug("ClearVisibilityStates()");
		for (int i = 0; i < visibilityStates.Length; i++)
		{
			visibilityStates[i] = null;
		}
	}

	private void HandleGameManagerPlayerTookDamage(object sender, EventArgs args)
	{
		HideHUD();
	}

	private void HandleGameManagerRevived(object sender, EventArgs args)
	{
		ShowHUD();
	}

	private void HandleGameManagerPauseChanged(object sender, PauseChangeEventArgs args)
	{
		if (args.PauseReason == PauseReason.System)
		{
			if (GameManager.Instance.IsPaused)
			{
				HideHUD();
			}
			else if (StateManager.Instance.CurrentState == stateRoot)
			{
				ShowHUD();
			}
		}
	}

	private void HandleHazardHazardHurtPlayer(object sender, EventArgs e)
	{
		m_WhiteFlashAnim.Stop();
		m_WhiteFlashAnim.Play();
	}
}
