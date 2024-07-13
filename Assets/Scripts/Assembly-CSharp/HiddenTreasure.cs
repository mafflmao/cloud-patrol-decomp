using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenTreasure : Powerup
{
	private static int _comboBoost;

	private static float releaseTime = 2f;

	private bool _isFiringOnStart;

	public static bool IsActive { get; private set; }

	protected override void OnEnable()
	{
		base.OnEnable();
		_isFiringOnStart = ShipManager.instance.dragMultiTarget[m_DragMultiTargetIndex].targetQueue.Count > 0;
		BombController.BombControllerStarted += HandleBombControllerStarted;
		Shooter.ComboCompleted += HandleShooting;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		IsActive = false;
		StopAllCoroutines();
		BombController.BombControllerStarted -= HandleBombControllerStarted;
		Shooter.ComboCompleted -= HandleShooting;
	}

	private void HandleShooting(object sender, Shooter.ComboCompletedEventArgs args)
	{
		if (_isFiringOnStart)
		{
			_isFiringOnStart = false;
		}
		else
		{
			base.TimeLeft = Mathf.Clamp(base.TimeLeft - releaseTime, 0f, lifeTimeInSeconds);
		}
	}

	protected override void HandleTriggered()
	{
		base.HandleTriggered();
		StartCoroutine(AutoCollect());
		IsActive = true;
	}

	protected override void Update()
	{
		_timeLastFrame = Time.realtimeSinceStartup;
		if (base.IsTriggered && base.TimeLeft == 0f)
		{
			DestroyAndFinish(true);
		}
	}

	private void HandleBombControllerStarted(object sender, EventArgs args)
	{
		DestroyAndFinish(false);
	}

	private IEnumerable<Loot> GetNonCollectedMoney()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Magnetic");
		foreach (GameObject money in array)
		{
			if (money != null)
			{
				Loot lootScript = money.GetComponent<Loot>();
				if (lootScript == null)
				{
					Debug.LogWarning(money.name + " is tagged as magnetic, but doesn't have a loot component.");
				}
				else if (lootScript.amIMoney && !lootScript.IsCollected)
				{
					yield return lootScript;
				}
			}
		}
	}

	private IEnumerator AutoCollect()
	{
		while (base.gameObject != null)
		{
			foreach (Loot money in GetNonCollectedMoney())
			{
				money.Collect();
				yield return new WaitForSeconds(0.02f);
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	public static int ModifyComboCount(int comboCount)
	{
		if (_comboBoost > 0 && comboCount < 6)
		{
			return Mathf.Clamp(_comboBoost + comboCount, 2, 6);
		}
		return comboCount;
	}

	public override void DestroyAndFinish(bool waitForCutscene)
	{
		IsActive = false;
		StopAllCoroutines();
		base.DestroyAndFinish(waitForCutscene);
	}
}
