using System;
using UnityEngine;

public class ObjectDeathDropsObjectUpgrade : CharacterUpgrade
{
	public EnemyModifierType _enemyOfType;

	public bool _sheep;

	public bool _balloon;

	public bool _destructible;

	public Destructible.Type _destructibleType = Destructible.Type.Other;

	public GameObject _objectPrefab;

	private void OnEnable()
	{
		Health.Killed += HandleHealthKilled;
	}

	private void OnDisable()
	{
		Health.Killed -= HandleHealthKilled;
	}

	private void HandleHealthKilled(object sender, EventArgs e)
	{
		Health health = (Health)sender;
		if (EnemyUtils.EnemyIsOfType(health, _enemyOfType) || (_sheep && IsSheepModifier.IsHealthAttachedToSheep(health)) || (_balloon && IsBalloonModifier.IsHealthAttachedToBalloon(health)) || (_destructible && IsDestructibleModifier.IsHealthAttachedToDestructible(health, _destructibleType)))
		{
			UnityEngine.Object.Instantiate(_objectPrefab, health.transform.position, Quaternion.identity);
		}
	}
}
