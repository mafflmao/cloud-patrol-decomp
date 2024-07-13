using UnityEngine;

public class ProjectileSpeedUpgrade : CharacterUpgrade
{
	public float projectileSpeedMultiplier = 0.5f;

	public GameObject windPrefab;

	public void SpawnWindOnProjectile(Transform projectile)
	{
		SpawnWindOnProjectile(projectile, Vector3.one);
	}

	public void SpawnWindOnProjectile(Transform projectile, Vector3 scale)
	{
		if (windPrefab != null && projectile != null)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = projectile;
			gameObject.transform.localScale = scale;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.AddComponent<Billboard>();
			GameObject gameObject2 = Object.Instantiate(windPrefab, projectile.position, Quaternion.identity) as GameObject;
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = Vector3.zero;
		}
	}
}
