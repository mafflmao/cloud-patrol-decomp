using UnityEngine;

public class WizardExplosionUpgrade : CharacterUpgrade
{
	public bool onlyRedWizards = true;

	public GameObject explosionPrefab;

	public void SpawnExplosion(Vector3 position)
	{
		if (explosionPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(explosionPrefab, position, Quaternion.identity) as GameObject;
			gameObject.transform.parent = LevelManager.Instance.currentScreenRoot.transform;
		}
	}
}
