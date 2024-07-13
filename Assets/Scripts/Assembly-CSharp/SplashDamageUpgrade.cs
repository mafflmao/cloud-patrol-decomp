using UnityEngine;

public class SplashDamageUpgrade : CharacterUpgrade
{
	public GameObject coinPrefab;

	public void TrySpawnCoin(Vector3 position)
	{
		if (coinPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(coinPrefab, position, Quaternion.identity) as GameObject;
			gameObject.transform.parent = LevelManager.Instance.currentScreenRoot.transform;
		}
	}
}
