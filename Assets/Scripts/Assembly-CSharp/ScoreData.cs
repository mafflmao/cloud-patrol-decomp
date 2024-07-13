using System.Collections.Generic;
using UnityEngine;

public class ScoreData : ScriptableObject
{
	public enum ScoreType
	{
		DEFAULT = 0,
		ENEMY_STANDARD = 5,
		ENEMY_SHOOTER = 10,
		ENEMY_SHOOTER_RED = 13,
		ENEMY_WIZARD = 15,
		PROJECTILE_STRAIGHT = 20,
		PROJECTILE_CORKSCREW = 25,
		OBJECT_GENERIC = 30,
		ROOMCLEARED = 35,
		ROOMCLEARED_UPGRADE = 36,
		BONUS_SMALL = 40,
		BONUS_MEDIUM = 45,
		BONUS_LARGE = 50,
		COMBO_1X = 55,
		COMBO_2X = 60,
		COMBO_3X = 65,
		COMBO_4X = 70,
		COMBO_5X = 75,
		COMBO_MAX = 80,
		BOMB = 85,
		SPIKESHIELD = 90,
		DESTRUCTIBLE_BONUS = 95,
		BOMBMALUS = 100,
		PROJECTILEHIT = 105
	}

	public List<ScoreKeyValue> ScoreSets;

	private Dictionary<ScoreType, int> ScoreDictionary = new Dictionary<ScoreType, int>();

	public void BuildDictionary()
	{
		ScoreDictionary.Clear();
		foreach (ScoreKeyValue scoreSet in ScoreSets)
		{
			ScoreDictionary.Add(scoreSet.type, scoreSet.scoreValue);
		}
	}

	public int GetScoreFromType(ScoreType type)
	{
		if (ScoreDictionary.ContainsKey(type))
		{
			switch (type)
			{
			case ScoreType.BOMBMALUS:
				return OperatorMenu.Instance.m_Pts_Removed_Per_Bombs;
			case ScoreType.PROJECTILEHIT:
				return OperatorMenu.Instance.m_Pts_Removed_Per_Projectile;
			default:
				return ScoreDictionary[type];
			}
		}
		Debug.LogError("Couldn't find score type " + type);
		return 0;
	}
}
