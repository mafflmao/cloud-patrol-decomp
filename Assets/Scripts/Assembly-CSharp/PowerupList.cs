using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerupList : ScriptableObject
{
	public List<PowerupData> powerups;

	public PowerupData ChooseRandomCollectablePowerup()
	{
		PowerupData[] array = powerups.Where((PowerupData powerup) => powerup.isCollectable).ToArray();
		int num = Random.Range(0, array.Length);
		return array[num];
	}

	public List<string> GetMagicMomentSceneNames()
	{
		List<string> list = new List<string>();
		foreach (PowerupData powerup in powerups)
		{
			if (powerup.magicMomentScene != null && (!DebugSettingsUI.BuildWithGhostSwordsOnly || powerup.LocalizedName == "GHOSTSWORDS"))
			{
				if (string.IsNullOrEmpty(powerup.magicMomentScene))
				{
					Debug.LogError("PowerupData '" + powerup.name + "' has null or empty scene.");
				}
				list.Add(powerup.magicMomentScene);
			}
		}
		return list;
	}
}
