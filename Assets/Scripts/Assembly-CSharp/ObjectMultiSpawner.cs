using UnityEngine;

public class ObjectMultiSpawner : MonoBehaviour
{
	public float delayTime;

	public float waitTime;

	public int numToSpawn;

	public Transform[] enemyTypes;

	private float lastSpawnTime;

	private int count;

	private bool shouldSpawn = true;

	private void SpawnObject()
	{
		Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
		Object.Instantiate(enemyTypes[0], base.transform.position, rotation);
		GameManager.EnemySpawned();
		count++;
		if (count == numToSpawn)
		{
			shouldSpawn = false;
		}
	}

	private void Restart()
	{
		if (Time.time > lastSpawnTime + waitTime)
		{
			count = 0;
			shouldSpawn = true;
		}
	}

	private void Update()
	{
		if (GameManager.gameStarted && GameManager.gameState == GameManager.GameState.Playing)
		{
			if (Time.time > lastSpawnTime + delayTime && count < numToSpawn && shouldSpawn)
			{
				lastSpawnTime = Time.time;
				SpawnObject();
			}
			else if (!shouldSpawn)
			{
				Restart();
			}
		}
	}
}
