using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnemyUtils
{
	public static IEnumerable<GameObject> GetEnemies(GameObject screenRootNode)
	{
		Type[] enemyTypes = new Type[5]
		{
			typeof(Troll),
			typeof(EnemyStationaryShooter),
			typeof(EnemyFireProjectile),
			typeof(TrollShield),
			typeof(EnemyShooterShielded)
		};
		if (screenRootNode == null)
		{
			yield break;
		}
		Type[] array = enemyTypes;
		foreach (Type enemyType in array)
		{
			foreach (MonoBehaviour enemyScript in screenRootNode.GetComponentsInChildren(enemyType).Cast<MonoBehaviour>())
			{
				if (enemyType == typeof(EnemyStationaryShooter))
				{
					EnemyStationaryShooter stationaryShooterScript = (EnemyStationaryShooter)enemyScript;
					if (stationaryShooterScript.hasJetpack)
					{
						continue;
					}
				}
				if (enemyType == typeof(Troll))
				{
					Troll trollScript = (Troll)enemyScript;
					if (trollScript.hasJetpack)
					{
						continue;
					}
				}
				yield return enemyScript.gameObject;
			}
		}
	}

	public static bool EnemyIsOfType(Health healthScript, EnemyModifierType type)
	{
		if (healthScript == null)
		{
			return false;
		}
		if (healthScript.isEnemy)
		{
			switch (type)
			{
			case EnemyModifierType.All:
				return true;
			case EnemyModifierType.Dodger:
				return healthScript.GetComponent<EnemyDodger>() != null || healthScript.GetComponent<EnemySuperDodger>() != null;
			case EnemyModifierType.Shield:
				return healthScript.GetComponent<TrollShield>() != null || healthScript.GetComponent<EnemyShooterShielded>() != null;
			case EnemyModifierType.Shooter:
				return healthScript.GetComponent<Shooter>() != null;
			case EnemyModifierType.Lobber:
				return healthScript.GetComponent<LobGrenade>() != null;
			case EnemyModifierType.Protector:
				return healthScript.GetComponent<EnemyProtector>() != null;
			case EnemyModifierType.Flier:
			{
				Troll component = healthScript.GetComponent<Troll>();
				if (component != null)
				{
					return component.hasJetpack;
				}
				EnemyStationaryShooter component2 = healthScript.GetComponent<EnemyStationaryShooter>();
				if (component2 != null)
				{
					return component2.hasJetpack;
				}
				return false;
			}
			default:
				return false;
			}
		}
		return false;
	}
}
