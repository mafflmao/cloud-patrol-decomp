using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : ScriptableObject
{
	public string sceneName;

	public List<EnemyTypes> requiredTutorials;

	[NonSerialized]
	public bool isExpanded;
}
