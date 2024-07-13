using System.Collections;
using UnityEngine;

public class HealingElixir : Powerup
{
	public static int baseCost = 21;

	public static int rateOfIncreasePerRoom = 4;

	public static string storageKey = "healingelixir";

	public static float percentCost = 1f;

	public static int gemCost = 5;

	public static int numUses = 2;

	private static bool _wasUsed;

	public static int UnlockedLevel
	{
		get
		{
			return MagicItemManager.Instance.GetPowerupLevelFromString(storageKey);
		}
	}

	public static bool WasUsed
	{
		get
		{
			if (!MagicItemManager.Instance.IsPowerupUnlockedFromString(storageKey))
			{
				return false;
			}
			return _wasUsed;
		}
		set
		{
			_wasUsed = value;
		}
	}

	public static bool IsUsable
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	public static int GetCoinCost()
	{
		return Mathf.RoundToInt((float)(baseCost + rateOfIncreasePerRoom * LevelManager.Instance.RoomsCleared * LevelManager.Instance.RoomsCleared) * percentCost);
	}

	public static int GetGemCost()
	{
		return gemCost;
	}

	public static void Reset()
	{
		_wasUsed = false;
	}

	protected override void HandleTriggered()
	{
		base.HandleTriggered();
		StartCoroutine(ReviveSequence());
	}

	private IEnumerator ReviveSequence()
	{
		_wasUsed = true;
		if (WackAManager.IsActive)
		{
			MusicManager.Instance.PlayCurrentBossMusic();
		}
		else
		{
			MusicManager.Instance.PlayCurrentGameplayMusic();
		}
		ShipManager.instance.shipVisual.Revive();
		yield return new WaitForSeconds(0.5f);
		if (!LevelManager.Instance.IsTransitioning)
		{
			GameManager.ExplodeBombs();
			GameManager.KillAllEnemies();
		}
		GameManager.ExplodeProjectiles();
		GameManager.Instance.Continue();
		DifficultyManager.Instance.HealingElixirDifficultyDecrease();
		DestroyAndFinish(true);
	}
}
