using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
	public float startTime = 1f;

	public float repeatTime = 1f;

	public Transform[] enemyTypes;

	public bool randomSpawn;

	public bool shouldSpawn = true;

	public bool spawnInfinite = true;

	public int totalNumToSpawn = 3;

	public bool addToObjsToDestroy;

	public bool destroyWhenDone;

	public bool switchDirections;

	public Vector3 spawnedMoveOverride = Vector3.zero;

	public float spawnedSpeedOverride;

	public bool isStationary = true;

	public List<Transform> dodgerLocs;

	public bool sheepLauncher;

	public int rotation;

	public float PercentChanceToSpawn = 1f;

	private int _spawnIndex;

	private float _lastFrameTime;

	private float _startUpTimer;

	private int _totalNumSpawned;

	private SpawnerChangeUpgrade _upgrade;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(ObjectSpawner), LogLevel.Log);

	private void Start()
	{
		if (PercentChanceToSpawn != 1f)
		{
			float num = Random.Range(0f, 1f);
			if (num <= PercentChanceToSpawn)
			{
				shouldSpawn = true;
			}
			else
			{
				shouldSpawn = false;
			}
		}
		_startUpTimer = startTime + Time.time;
		_upgrade = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<SpawnerChangeUpgrade>();
	}

	private GameObject SpawnObject()
	{
		return SpawnObject(1f, false);
	}

	public static bool IsShootable(GameObject spawnObject)
	{
		return !spawnObject.GetComponent<Hazard>() && !spawnObject.name.StartsWith("EmptyObj");
	}

	public GameObject SpawnObject(float trollWaitTimeFactor, bool forceShootableIfPossible)
	{
		_log.Log("Spawning from - {0}", base.name);
		if (randomSpawn)
		{
			_spawnIndex = Random.Range(0, enemyTypes.Length);
		}
		else
		{
			_spawnIndex++;
			_spawnIndex %= enemyTypes.Length;
		}
		Transform transform = enemyTypes[_spawnIndex];
		if (_upgrade != null)
		{
			transform = _upgrade.ReplaceIfNecessary(transform);
		}
		if (forceShootableIfPossible)
		{
			int num = 0;
			while (num < enemyTypes.Length && !IsShootable(transform.gameObject))
			{
				num++;
				_spawnIndex++;
				_spawnIndex %= enemyTypes.Length;
				transform = enemyTypes[_spawnIndex];
			}
		}
		Quaternion quaternion = Quaternion.Euler(0f, rotation, 0f);
		Transform transform2 = (Transform)Object.Instantiate(transform, base.transform.position, quaternion);
		Health component = transform2.GetComponent<Health>();
		if (component != null)
		{
			component.destroyTimeAfterHit = 0.5f;
		}
		if (addToObjsToDestroy)
		{
			transform2.parent = LevelManager.Instance.CurrentScreenManager.transform;
		}
		EnemyStationaryShooter component2 = transform2.GetComponent<EnemyStationaryShooter>();
		if (component2 != null && isStationary)
		{
			component2.gameObject.layer = Layers.EnemiesDontTarget;
			component2.initWaitTime = Random.Range(0.2f, 1f) * trollWaitTimeFactor;
			component2.peekABoo = true;
		}
		EnemyFireProjectile component3 = transform2.GetComponent<EnemyFireProjectile>();
		if (component3 != null)
		{
			component3.gameObject.layer = Layers.EnemiesDontTarget;
			component3.initWaitTime = Random.Range(0.5f, 1.5f) * trollWaitTimeFactor;
			component3.peekABoo = true;
		}
		Troll component4 = transform2.GetComponent<Troll>();
		if (component4 != null)
		{
			component4.gameObject.layer = Layers.EnemiesDontTarget;
			component4.peekABoo = true;
			component4.initialWaitTime *= trollWaitTimeFactor;
		}
		EnemyDodger component5 = transform2.GetComponent<EnemyDodger>();
		if ((bool)component5)
		{
			Health component6 = transform2.GetComponent<Health>();
			if ((bool)component6.poofEffect)
			{
				Object.Instantiate(component6.poofEffect, transform2.transform.position, Quaternion.identity);
			}
			SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.EnemyPoof, base.gameObject);
			if (dodgerLocs.Count > 0)
			{
				int index = Random.Range(0, dodgerLocs.Count);
				component5.teleportLocation = dodgerLocs[index];
			}
		}
		MoverBounce component7 = transform2.GetComponent<MoverBounce>();
		if ((bool)component7 && sheepLauncher)
		{
			component7.jumpOnStart = true;
			component7.forceWhenAtRest.y = Random.Range(3, 6);
			Rotator component8 = transform2.GetComponent<Rotator>();
			if (component8 != null)
			{
				component8.randomRotationX = 1;
			}
		}
		else if ((bool)component7 && component7.forceWhenAtRest.y == 3f)
		{
			component7.forceWhenAtRest.y = Random.Range(3f, 3.5f);
		}
		TrollBase component9 = transform2.GetComponent<TrollBase>();
		if (component9 != null)
		{
			component9.StartTrollBehaviour();
		}
		if (switchDirections)
		{
			Mover component10 = transform2.GetComponent<Mover>();
			if ((bool)component10)
			{
				component10.direction = new Vector3(1f, 0f, 0f);
			}
		}
		else if (spawnedMoveOverride != Vector3.zero)
		{
			Mover component11 = transform2.GetComponent<Mover>();
			if ((bool)component11)
			{
				component11.direction = spawnedMoveOverride;
			}
		}
		if (spawnedSpeedOverride != 0f)
		{
			Mover component12 = transform2.GetComponent<Mover>();
			if ((bool)component12)
			{
				component12.speed = spawnedSpeedOverride;
			}
		}
		GameManager.EnemySpawned();
		_totalNumSpawned++;
		return transform2.gameObject;
	}

	public static void DestroySpawnedInstance(GameObject spawnedObject)
	{
		if (spawnedObject == null)
		{
			return;
		}
		Health component = spawnedObject.GetComponent<Health>();
		if (component != null)
		{
			component.SpawnPoof();
			SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.EnemyPoof, spawnedObject);
		}
		else
		{
			Hazard component2 = spawnedObject.GetComponent<Hazard>();
			if (component2 != null)
			{
				component2.SpawnPoof();
				SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.EnemyPoof, spawnedObject);
			}
		}
		Object.Destroy(spawnedObject, 0.1f);
	}

	private void Update()
	{
		if (shouldSpawn)
		{
			if (Time.time > _lastFrameTime && Time.time > _startUpTimer && (spawnInfinite || _totalNumSpawned < totalNumToSpawn))
			{
				SpawnObject();
				_lastFrameTime = Time.time + repeatTime;
			}
			else if (destroyWhenDone && _totalNumSpawned >= totalNumToSpawn)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void StopSpawn()
	{
		shouldSpawn = false;
	}

	public void StartSpawn()
	{
		shouldSpawn = true;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawCube(base.transform.position, new Vector3(0.25f, 0.3333333f, 0.001f));
	}
}
