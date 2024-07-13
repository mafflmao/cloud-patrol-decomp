using UnityEngine;

public class ElementData : ScriptableObject
{
	public Elements.Type elementType;

	public Color gunTracerColor;

	public Color skylanderSelectColor;

	public GameObject LoadMuzzleFlashPrefab()
	{
		return LoadResource<GameObject>("MuzzleFlash");
	}

	public GameObject LoadTurretSpawnFxPrefab()
	{
		return LoadResource<GameObject>("TurretSpawnFx");
	}

	public SoundEventData LoadTurretSpawnSfx()
	{
		return LoadResource<SoundEventData>("Skylander_SpawnTurret_SFX");
	}

	public Texture2D LoadTurretTexture()
	{
		return LoadResource<Texture2D>("Turret_Diffuse");
	}

	public SoundEventData LoadPurchaseSfx()
	{
		return LoadResource<SoundEventData>("Skylander_Purchase_SFX");
	}

	public SoundEventData LoadLevelupSfx()
	{
		return LoadResource<SoundEventData>("Skylander_Touch_SFX");
	}

	private T LoadResource<T>(string resourceName) where T : Object
	{
		string resourcePath = string.Format("ElementData/{0}/{1}_{0}", elementType, resourceName);
		return ResourceUtils.LoadResource<T>(resourcePath);
	}
}
