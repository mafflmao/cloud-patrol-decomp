using System.Collections.Generic;
using UnityEngine;

public class SpawnerChangeUpgrade : CharacterUpgrade
{
	public SpawnerChangeData[] replacements;

	public bool doubledSpawnChance;

	public float replaceChance = 1f;

	private Dictionary<Transform, Transform> _replacementLookup = new Dictionary<Transform, Transform>();

	private void Awake()
	{
		SpawnerChangeData[] array = replacements;
		foreach (SpawnerChangeData spawnerChangeData in array)
		{
			_replacementLookup.Add(spawnerChangeData.originalObject, spawnerChangeData.replacement);
		}
	}

	public Transform ReplaceIfNecessary(Transform candidate)
	{
		Transform value;
		if (!_replacementLookup.TryGetValue(candidate, out value) || !LevelManager.Instance.FinishedTutorials || Random.value > replaceChance)
		{
			return candidate;
		}
		return value;
	}
}
