using UnityEngine;

public class GemItem : Powerup
{
	public GameObject oneGemSpawn;

	public GameObject twoGemSpawn;

	public GameObject threeGemSpawn;

	public SoundEventData sfxGemSpawn;

	protected void Start()
	{
	}

	protected override void HandleTriggered()
	{
		SoundEventManager.Instance.Play(sfxGemSpawn, base.gameObject);
		SpawnGemFromPrefab(oneGemSpawn);
		base.HandleTriggered();
	}

	protected override void HandleCutsceneComplete()
	{
		base.HandleCutsceneComplete();
		DestroyAndFinish(false);
	}

	private PowerupHolder GetTriggeredPowerupHolder()
	{
		PowerupHolder result = null;
		PowerupHolder[] powerupHolders = ShipManager.instance.powerupHolders;
		for (int i = 0; i < powerupHolders.Length; i++)
		{
			if (powerupHolders[i].State == PowerupStates.active)
			{
				result = powerupHolders[i];
				break;
			}
		}
		return result;
	}

	private void SpawnGemFromPrefab(GameObject prefab)
	{
		PowerupHolder triggeredPowerupHolder = GetTriggeredPowerupHolder();
		if (triggeredPowerupHolder != null)
		{
			Vector3 position = triggeredPowerupHolder.transform.position;
			GameObject gameObject = Object.Instantiate(prefab, position, Quaternion.identity) as GameObject;
			gameObject.transform.Translate(new Vector3(0f, 0f, 0.01f));
			gameObject.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
			GameObjectUtils.SetLayerRecursive(gameObject, LayerMask.NameToLayer("LitHUD"));
			iTween.PunchScale(gameObject, iTween.Hash("amount", new Vector3(0.37f, 0.37f, 0.37f), "time", 1f));
		}
	}
}
