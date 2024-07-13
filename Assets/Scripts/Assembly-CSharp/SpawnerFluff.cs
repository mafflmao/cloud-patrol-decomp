using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerFluff : MonoBehaviour
{
	public Transform[] spawnTypes;

	public float PercentChanceToSpawn = 1f;

	public int numToSpawn = 1;

	private int numSpawned;

	public float timeBetweenSpawns = 2f;

	public int tempLevelObjHack = 270;

	public int delayFirstSpawn;

	private float timer;

	private static bool HasSetCollisionFlags;

	private static void TrySetLayerCollisionFlags()
	{
		if (!HasSetCollisionFlags)
		{
			Physics.IgnoreLayerCollision(Layers.Enemies, Layers.Props);
			HasSetCollisionFlags = true;
		}
	}

	private void Start()
	{
		TrySetLayerCollisionFlags();
		StartCoroutine(SpawnCoroutine());
	}

	private IEnumerator SpawnCoroutine()
	{
		if (delayFirstSpawn != 0)
		{
			yield return new WaitForSeconds(delayFirstSpawn);
		}
		SpawnerChangeUpgrade upgrade = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<SpawnerChangeUpgrade>();
		while (numSpawned < numToSpawn)
		{
			SpawnSingleObject(upgrade);
			numSpawned++;
			yield return new WaitForSeconds(timeBetweenSpawns);
		}
	}

	private void SpawnSingleObject(SpawnerChangeUpgrade upgrade)
	{
		if (Random.value > PercentChanceToSpawn)
		{
			return;
		}
		int num = 0;
		if (upgrade != null && upgrade.doubledSpawnChance)
		{
			List<int> list = new List<int>();
			SpawnerChangeData[] replacements = upgrade.replacements;
			foreach (SpawnerChangeData spawnerChangeData in replacements)
			{
				for (int j = 0; j < spawnTypes.Length; j++)
				{
					if (spawnTypes[j] == spawnerChangeData.originalObject)
					{
						list.Add(j);
						break;
					}
				}
			}
			num = Random.Range(0, spawnTypes.Length + list.Count - 1);
			if (num >= spawnTypes.Length)
			{
				num = list[num - spawnTypes.Length];
			}
		}
		else
		{
			num = Random.Range(0, spawnTypes.Length);
		}
		Transform transform = spawnTypes[num];
		if (upgrade != null)
		{
			transform = upgrade.ReplaceIfNecessary(transform);
		}
		Quaternion rotation = Quaternion.Euler(tempLevelObjHack, 0f, 0f);
		Transform transform2 = (Transform)Object.Instantiate(transform, base.transform.position, rotation);
		if (transform2.name.ToLower().StartsWith("rock_"))
		{
			transform2.gameObject.AddComponent<MeshCollider>();
			if (transform2.gameObject.layer != Layers.Enemies)
			{
				transform2.gameObject.layer = Layers.Props;
			}
		}
		base.transform.localScale = Vector3.one;
		transform2.transform.parent = base.transform;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawCube(base.transform.position, new Vector3(0.25f, 0.3333333f, 0.001f));
	}
}
