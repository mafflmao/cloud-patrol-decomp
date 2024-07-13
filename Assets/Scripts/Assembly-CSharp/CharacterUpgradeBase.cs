using System;
using UnityEngine;

[Serializable]
public abstract class CharacterUpgradeBase
{
	public string readableName = "Readble Name";

	public string description = "Description";

	public int unlockCost = 10000;

	public UpgradeRequirement requirement;

	public string iconName;

	[NonSerialized]
	private string _prefabResourcePath;

	[NonSerialized]
	private Texture2D _icon;

	[NonSerialized]
	private UpgradeRequirement _requirementInstance;

	public string LocalizedName
	{
		get
		{
			return LocalizationManager.Instance.GetString(readableName);
		}
	}

	public string LocalizedDescription
	{
		get
		{
			string text = LocalizationManager.Instance.GetString(description);
			if (requirement is MagicItemUnlockedRequirement)
			{
				MagicItemUnlockedRequirement magicItemUnlockedRequirement = (MagicItemUnlockedRequirement)requirement;
				string localizedName = magicItemUnlockedRequirement.magicItem.LocalizedName;
				text = text.Replace("{Magic Item}", localizedName);
			}
			return text;
		}
	}

	protected abstract string PrefabResourceFolder { get; }

	protected abstract string IconResourceFolder { get; }

	protected abstract string IconResourcePrefix { get; }

	public bool AllRequirementsMet
	{
		get
		{
			return requirement == null || requirement.CheckRequirement();
		}
	}

	public Texture2D Icon
	{
		get
		{
			if (_icon == null)
			{
				string text = IconResourceFolder + "/" + IconResourcePrefix + iconName;
				_icon = (Texture2D)Resources.Load(text, typeof(Texture2D));
				if (_icon == null)
				{
					Debug.LogError("Unable to load icon '" + iconName + "' from '" + text + "'.");
				}
			}
			return _icon;
		}
	}

	public void Initialize(CharacterData character)
	{
		if (_prefabResourcePath == null)
		{
			if (character.isSwapForce)
			{
				_prefabResourcePath = string.Format("{0}/{1}", PrefabResourceFolder, "Swapforce");
			}
			else
			{
				_prefabResourcePath = string.Format("{0}/{1}", PrefabResourceFolder, character.elementData.elementType.ToString());
			}
		}
	}

	public void CreateRequirementInstance()
	{
		DestroyRequirementInstance();
		if (!(requirement == null))
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(requirement.gameObject);
			gameObject.transform.parent = CharacterUpgradeManager.Instance.transform;
			_requirementInstance = gameObject.GetComponent<UpgradeRequirement>();
		}
	}

	public void DestroyRequirementInstance()
	{
		if (_requirementInstance != null)
		{
			UnityEngine.Object.Destroy(_requirementInstance.gameObject);
			_requirementInstance = null;
		}
	}

	public void ReleaseIconResources()
	{
		_icon = null;
	}

	protected GameObject GetResourcePrefab()
	{
		if (string.IsNullOrEmpty(_prefabResourcePath))
		{
			Debug.LogError("Unable to find resource path for '" + readableName + " (at path '" + _prefabResourcePath + "') '... Has it been initialized?");
			return null;
		}
		GameObject gameObject = (GameObject)Resources.Load(_prefabResourcePath, typeof(GameObject));
		if (gameObject == null)
		{
			Debug.LogError("Unable to load resource from '" + _prefabResourcePath + "' for '" + readableName + "'.");
			return null;
		}
		return gameObject;
	}
}
