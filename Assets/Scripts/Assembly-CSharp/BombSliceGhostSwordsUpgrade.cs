using UnityEngine;

public class BombSliceGhostSwordsUpgrade : CharacterUpgrade
{
	public GameObject bombSliceL2RPrefab;

	public GameObject bombSliceR2LPrefab;

	public GameObject shieldSliceL2RPrefab;

	public GameObject shieldSliceR2LPrefab;

	public void SpawnSlicedBombAtTarget(GameObject bomb)
	{
		if (!(bomb == null))
		{
			if (Random.value > 0.5f)
			{
				GameObject obj = Object.Instantiate(bombSliceL2RPrefab, bomb.transform.position, bomb.transform.rotation) as GameObject;
				Object.Destroy(obj, 2f);
			}
			else
			{
				GameObject obj2 = Object.Instantiate(bombSliceR2LPrefab, bomb.transform.position, bomb.transform.rotation) as GameObject;
				Object.Destroy(obj2, 2f);
			}
		}
	}

	public void SpawnSlicedShieldAtTarget(GameObject target)
	{
		if (!(target == null))
		{
			if (Random.value > 0.5f)
			{
				GameObject obj = Object.Instantiate(shieldSliceL2RPrefab, target.transform.position, target.transform.rotation) as GameObject;
				Object.Destroy(obj, 2f);
			}
			else
			{
				GameObject obj2 = Object.Instantiate(shieldSliceR2LPrefab, target.transform.position, target.transform.rotation) as GameObject;
				Object.Destroy(obj2, 2f);
			}
		}
	}
}
