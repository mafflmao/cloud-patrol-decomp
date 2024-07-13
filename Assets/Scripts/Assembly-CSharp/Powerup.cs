using System;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
	private bool _finishAndDestroyWhenCutsceneComplete;

	public float lifeTimeInSeconds;

	public bool manuallyUpdatesProgress;

	protected float _timeLastFrame;

	protected int m_DragMultiTargetIndex;

	protected bool IsCutsceneComplete { get; private set; }

	public bool IsTriggered { get; private set; }

	public bool IsBonus { get; set; }

	public float TimeLeft { get; protected set; }

	public int Level { get; protected set; }

	public PowerupHolder Holder { get; set; }

	public int DragMultiTargetIndex
	{
		set
		{
			m_DragMultiTargetIndex = value;
		}
	}

	public PowerupData PowerupData { get; set; }

	public static event EventHandler<PowerupEventArgs> Finished;

	public static event EventHandler<PowerupEventArgs> Triggered;

	public Powerup()
	{
		Level = 1;
	}

	public virtual void SetLevel(int newLevel, float newLifeTimeInSeconds)
	{
		Level = newLevel;
		lifeTimeInSeconds = newLifeTimeInSeconds;
		lifeTimeInSeconds += GetUpgradeModifier();
	}

	public float GetUpgradeModifier()
	{
		MagicItemEffectivnessUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<MagicItemEffectivnessUpgrade>();
		if (passiveUpgradeOrDefault != null && passiveUpgradeOrDefault.powerup == PowerupData)
		{
			return passiveUpgradeOrDefault.deltaValues[Level - 1];
		}
		return 0f;
	}

	protected virtual void OnEnable()
	{
		PowerupCutscene.TriggerPower += HandlePowerupCutsceneTrigger;
		PowerupCutscene.Completed += HandlePowerupCutsceneComplete;
	}

	protected virtual void OnDisable()
	{
		PowerupCutscene.TriggerPower -= HandlePowerupCutsceneTrigger;
		PowerupCutscene.Completed -= HandlePowerupCutsceneComplete;
		TimeLeft = 0f;
		MagicItemManager.IsMagicItemActive = false;
	}

	private void HandlePowerupCutsceneComplete(object sender, EventArgs e)
	{
		HandlePowerupCutsceneCompleteInternal();
	}

	private void HandlePowerupCutsceneCompleteInternal()
	{
		IsCutsceneComplete = true;
		HandleCutsceneComplete();
		if (_finishAndDestroyWhenCutsceneComplete)
		{
			DestroyAndFinish(false);
		}
	}

	private void HandlePowerupCutsceneTrigger(object sender, EventArgs e)
	{
		HandlePowerupCustceneTriggerInternal();
	}

	private void HandlePowerupCustceneTriggerInternal()
	{
		IsTriggered = true;
		HandleTriggered();
	}

	protected virtual void HandleTriggered()
	{
		MagicItemManager.IsMagicItemActive = true;
		TimeLeft = lifeTimeInSeconds;
		_timeLastFrame = Time.realtimeSinceStartup;
	}

	protected virtual void Update()
	{
		if (TimeLeft != 0f && !GameManager.Instance.IsPaused && !HealingElixirScreen.IsActive && !GameManager.Instance.IsGameOver)
		{
			TimeLeft -= Time.realtimeSinceStartup - _timeLastFrame;
			TimeLeft = Mathf.Clamp(TimeLeft, 0f, float.PositiveInfinity);
		}
		_timeLastFrame = Time.realtimeSinceStartup;
	}

	public virtual void DestroyAndFinish(bool waitForCutscene)
	{
		if (waitForCutscene)
		{
			if (IsCutsceneComplete)
			{
				DestroyAndFinish(false);
			}
			else
			{
				_finishAndDestroyWhenCutsceneComplete = true;
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
			MagicItemManager.IsMagicItemActive = false;
			OnFinished();
		}
	}

	protected virtual void HandleCutsceneComplete()
	{
	}

	protected void OnFinished()
	{
		if (Powerup.Finished != null)
		{
			Powerup.Finished(this, new PowerupEventArgs(PowerupData));
		}
	}

	public void OnPowerupTriggered()
	{
		if (Powerup.Triggered != null)
		{
			Powerup.Triggered(this, new PowerupEventArgs(PowerupData));
		}
		if (GetComponent<PowerupCutscene>() == null)
		{
			HandlePowerupCustceneTriggerInternal();
			HandlePowerupCutsceneCompleteInternal();
		}
	}
}
