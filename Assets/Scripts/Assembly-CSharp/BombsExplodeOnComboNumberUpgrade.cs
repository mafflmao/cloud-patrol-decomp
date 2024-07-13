using UnityEngine;

public class BombsExplodeOnComboNumberUpgrade : CharacterUpgrade
{
	public int comboToTrigger = 6;

	public GameObject bombImpactVFX;

	public SoundEventData sfxBombImpact;

	public GameObject coinToSpawn;

	public bool rocksToo;

	public void Explode()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Bomb");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (!(gameObject == null))
			{
				Hazard component = gameObject.GetComponent<Hazard>();
				if (component != null)
				{
					component.DropLoot();
					Object.Instantiate(bombImpactVFX, component.transform.position, component.transform.rotation);
					SoundEventManager.Instance.Play(sfxBombImpact, component.gameObject);
					component.DefusedHazard(true);
					Object.Instantiate(coinToSpawn, component.transform.position, Quaternion.identity);
					Object.Destroy(component.gameObject);
				}
			}
		}
		if (!rocksToo)
		{
			return;
		}
		GameObject[] array3 = GameObject.FindGameObjectsWithTag("Rock");
		GameObject[] array4 = array3;
		foreach (GameObject gameObject2 in array4)
		{
			if (!(gameObject2 == null))
			{
				Object.Instantiate(bombImpactVFX, gameObject2.transform.position, gameObject2.transform.rotation);
				SoundEventManager.Instance.Play(sfxBombImpact, gameObject2);
				Object.Instantiate(coinToSpawn, gameObject2.transform.position, Quaternion.identity);
				Object.Destroy(gameObject2);
			}
		}
	}
}
