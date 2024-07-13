using UnityEngine;

public class SparxUpgrade : CharacterUpgrade
{
	public GameObject overrideSparxPrefab;

	public GameObject overrideLineRenderer;

	public GameObject overrideMuzzleFlash;

	public GameObject coinPrefab;

	public Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);

	public bool affectSpikeShields;

	public bool affectProjectiles;

	public float rateOfFire = 0.25f;

	public GameObject GetSparxOverride()
	{
		if (overrideSparxPrefab != null)
		{
			return Object.Instantiate(overrideSparxPrefab) as GameObject;
		}
		return null;
	}

	public GameObject GetElementalCoin()
	{
		if (coinPrefab != null)
		{
			return Object.Instantiate(coinPrefab) as GameObject;
		}
		return null;
	}

	public GameObject GetLineRenderer()
	{
		if (overrideLineRenderer != null)
		{
			return Object.Instantiate(overrideLineRenderer) as GameObject;
		}
		return null;
	}
}
