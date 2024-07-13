using System.Linq;
using UnityEngine;

public class Payload : MonoBehaviour
{
	public static string LastEvent;

	public static string CurrentRank
	{
		get
		{
			return "1";
		}
	}

	public static string CurrentArea
	{
		get
		{
			if ((bool)LevelManager.Instance.currentScreenRoot)
			{
				return LevelManager.Instance.currentScreenRoot.name;
			}
			return "None";
		}
	}

	public static string AreasCleared
	{
		get
		{
			return LevelManager.Instance.RoomsCleared.ToString();
		}
	}

	public static string ActiveSkylander
	{
		get
		{
			return StartGameSettings.Instance.activeSkylander.charName;
		}
	}

	public static string ElementOfTheDay
	{
		get
		{
			return Elements.Names[0];
		}
	}

	public static string ActiveGoal1
	{
		get
		{
			return "None";
		}
	}

	public static string ActiveGoal2
	{
		get
		{
			return "None";
		}
	}

	public static string ActiveGoal3
	{
		get
		{
			return "None";
		}
	}

	public static string CurrentCoinTotal
	{
		get
		{
			return "0";
		}
	}

	public static string CurrentGemTotal
	{
		get
		{
			return "0";
		}
	}

	public static string CurrentFlightCoinTotal
	{
		get
		{
			return GameManager.moneyCollectedInVoyage.ToString();
		}
	}

	public static string EquippedMagicItems
	{
		get
		{
			return string.Join(",", ShipManager.instance.UnlockedPowerupHolders.Select((PowerupHolder holder) => holder.EquippedPowerupName).ToArray());
		}
	}

	public static string DeathType
	{
		get
		{
			return GameManager.sessionStats.deathType;
		}
	}

	public static string DeathAI
	{
		get
		{
			return GameManager.sessionStats.deathAI.Replace("(Clone)", string.Empty);
		}
	}

	public static string DeathScreenLocation
	{
		get
		{
			return GameManager.sessionStats.deathScreenLocation.ToString();
		}
	}

	public static string SingleCoinsSpawned
	{
		get
		{
			return GameManager.sessionStats.singleCoinsSpawned.ToString();
		}
	}

	public static string SingleCoinsCollected
	{
		get
		{
			return GameManager.sessionStats.singleCoinsCollected.ToString();
		}
	}

	public static string Combo2CoinsSpawned
	{
		get
		{
			return GameManager.sessionStats.combo2CoinsSpawned.ToString();
		}
	}

	public static string Combo3CoinsSpawned
	{
		get
		{
			return GameManager.sessionStats.combo3CoinsSpawned.ToString();
		}
	}

	public static string Combo4CoinsSpawned
	{
		get
		{
			return GameManager.sessionStats.combo4CoinsSpawned.ToString();
		}
	}

	public static string Combo5CoinsSpawned
	{
		get
		{
			return GameManager.sessionStats.combo5CoinsSpawned.ToString();
		}
	}

	public static string Combo6CoinsSpawned
	{
		get
		{
			return GameManager.sessionStats.combo6CoinsSpawned.ToString();
		}
	}

	public static string Combo2CoinsCollected
	{
		get
		{
			return GameManager.sessionStats.combo2CoinsCollected.ToString();
		}
	}

	public static string Combo3CoinsCollected
	{
		get
		{
			return GameManager.sessionStats.combo3CoinsCollected.ToString();
		}
	}

	public static string Combo4CoinsCollected
	{
		get
		{
			return GameManager.sessionStats.combo4CoinsCollected.ToString();
		}
	}

	public static string Combo5CoinsCollected
	{
		get
		{
			return GameManager.sessionStats.combo5CoinsCollected.ToString();
		}
	}

	public static string Combo6CoinsCollected
	{
		get
		{
			return GameManager.sessionStats.combo6CoinsCollected.ToString();
		}
	}

	public static string TotalElixirUsed
	{
		get
		{
			return GameManager.sessionStats.totalElixirsUsed.ToString();
		}
	}

	public static string TotalAreaSkipsUsed
	{
		get
		{
			return GameManager.sessionStats.totalAreaSkipsUsed.ToString();
		}
	}

	public static string GlobalDifficulty
	{
		get
		{
			return DifficultyManager.Instance.currentGlobalDifficulty.ToString();
		}
	}

	public static string Device
	{
		get
		{
			return Application.platform.ToString();
		}
	}
}
