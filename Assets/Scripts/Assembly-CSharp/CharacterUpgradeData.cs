using System;
using UnityEngine;

[Serializable]
public class CharacterUpgradeData : CharacterUpgradeBase
{
	public CharacterUpgrade UpgradeInstance { get; private set; }

	protected override string PrefabResourceFolder
	{
		get
		{
			return "CharacterUpgrades";
		}
	}

	protected override string IconResourceFolder
	{
		get
		{
			return "CharacterUpgradeIcons";
		}
	}

	protected override string IconResourcePrefix
	{
		get
		{
			return "UpgradeIcon_";
		}
	}

	public CharacterUpgrade CreateUpgradeInstance()
	{
		if (UpgradeInstance != null)
		{
			Debug.LogError("Double-creating upgrade instance! Ruh-Roh!");
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(GetResourcePrefab());
		gameObject.transform.parent = CharacterUpgradeManager.Instance.transform;
		UpgradeInstance = gameObject.GetComponent<CharacterUpgrade>();
		if (UpgradeInstance == null)
		{
			Debug.LogError("Unable to find CharacterUpgrade script on upgrade instance for '" + readableName + ".");
		}
		return UpgradeInstance;
	}

	public void DestroyAndReleaseUpgradeInstance()
	{
		if (UpgradeInstance != null)
		{
			UnityEngine.Object.Destroy(UpgradeInstance.gameObject);
			UpgradeInstance = null;
		}
	}

	public PowerupData GetRequiredMagicItem()
	{
		if (requirement == null)
		{
			return null;
		}
		MagicItemUnlockedRequirement component = requirement.GetComponent<MagicItemUnlockedRequirement>();
		if (component != null)
		{
			return component.magicItem;
		}
		return null;
	}
}
