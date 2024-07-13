using System.Collections.Generic;
using UnityEngine;

public class PowerupData : ScriptableObject, ILinkable
{
	public enum ItemType
	{
		Powerup = 0,
		Consumable = 1
	}

	public enum CostType
	{
		Coins = 0,
		Gems = 1
	}

	public const int MaxLevel = 5;

	private const string LocalizedNameFormatString = "{0}_NAME";

	private const string LocalizedLockedFormatString = "{0}_LOCKEDDESCRIPTION";

	private const string LocalizedDescriptionFormatString = "{0}_DESCRIPTION_LEVEL_{1}";

	private const string LocalizedUpgradeFormatString = "{0}_UPGRADEDESCRIPTION_LEVEL_{1}";

	public static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(PowerupData), LogLevel.Log);

	public string localizationName;

	public string readableName;

	public string storageKey;

	public bool isCollectable = true;

	public int cost;

	public string storeSpriteName;

	public int storeSpriteGraphicIndex;

	public bool isUnlockOnSale;

	public string unlockSaleText = "0";

	public bool isUpgradeOnSale;

	public string upgradeSaleText = "0";

	public Texture inGameButtonTexture;

	public string magicMomentScene;

	public int toyId = -1;

	public bool canLinkWithToy = true;

	public bool canUpgrade = true;

	public ItemType Type;

	public bool affectsScoreMultiplier;

	public float level1_value;

	public int level1_upgradeCost;

	public int level1_scoreModifier = 2;

	public float level2_value;

	public int level2_upgradeCost;

	public int level2_scoreModifier = 3;

	public float level3_value;

	public int level3_upgradeCost;

	public int level3_scoreModifier = 4;

	public float level4_value;

	public int level4_upgradeCost;

	public int level4_scoreModifier = 5;

	public float level5_value;

	public int level5_upgradeCost;

	public int level5_scoreModifier = 6;

	public int maxConsumablesHeld = 99;

	public int purchaseConsumablePackCount = 1;

	public int consumableCost;

	public CostType consumableCostType;

	private int mConsumableHeld;

	public string LocalizedName
	{
		get
		{
			string key = string.Format("{0}_NAME", localizationName);
			return LocalizationManager.Instance.GetString(key);
		}
	}

	public int consumablesHeld
	{
		get
		{
			return mConsumableHeld;
		}
		set
		{
			mConsumableHeld = value;
		}
	}

	public bool IsLinkable
	{
		get
		{
			return canLinkWithToy && !IsLinked && !IsToyClaimable;
		}
	}

	public bool IsLinked
	{
		get
		{
			return false;
		}
	}

	public GameObject PowerupPrefab { get; private set; }

	public bool IsPurchasable
	{
		get
		{
			if (Type == ItemType.Consumable)
			{
				return !IsLocked;
			}
			if (Type == ItemType.Powerup)
			{
				return !IsLocked && canUpgrade;
			}
			return false;
		}
	}

	public bool IsPurchaseAvailable
	{
		get
		{
			if (Type == ItemType.Consumable)
			{
				return IsPurchasable && consumablesHeld + purchaseConsumablePackCount <= maxConsumablesHeld;
			}
			if (Type == ItemType.Powerup)
			{
				return IsPurchasable && !IsAtMaxLevel;
			}
			return false;
		}
	}

	public int PurchaseCost
	{
		get
		{
			if (Type == ItemType.Consumable)
			{
				return consumableCost;
			}
			if (Type == ItemType.Powerup)
			{
				return GetUpgradeCost(Level);
			}
			return 0;
		}
	}

	public int AltPurchaseCost
	{
		get
		{
			if (Type == ItemType.Consumable)
			{
				return level1_upgradeCost;
			}
			_log.LogWarning("You shouldnt' be accessing this with a non-consumable!");
			return 0;
		}
	}

	public CostType PurchaseCostType
	{
		get
		{
			if (Type == ItemType.Consumable)
			{
				return consumableCostType;
			}
			return CostType.Coins;
		}
	}

	public int Level
	{
		get
		{
			return 1;
		}
	}

	public string Description
	{
		get
		{
			string key = ((Type != 0) ? string.Format("{0}_LOCKEDDESCRIPTION", localizationName) : ((Level != 0) ? string.Format("{0}_DESCRIPTION_LEVEL_{1}", localizationName, Level) : string.Format("{0}_LOCKEDDESCRIPTION", localizationName, Level)));
			return LocalizationManager.Instance.GetString(key);
		}
	}

	public string UpgradeDescription
	{
		get
		{
			string key;
			if (Type == ItemType.Powerup)
			{
				key = string.Format("{0}_UPGRADEDESCRIPTION_LEVEL_{1}", localizationName, Level);
				return LocalizationManager.Instance.GetString(key);
			}
			key = string.Format("{0}_UPGRADEDESCRIPTION_LEVEL_{1}", localizationName, 1);
			return LocalizationManager.Instance.GetString(key);
		}
	}

	public bool IsAtMaxLevel
	{
		get
		{
			if (!canUpgrade)
			{
				return true;
			}
			return GetLevel() >= 5;
		}
	}

	public bool IsLocked
	{
		get
		{
			return false;
		}
	}

	public bool IsToyLinked
	{
		get
		{
			return false;
		}
	}

	public bool IsToyClaimable
	{
		get
		{
			if (canLinkWithToy)
			{
				Bedrock.brContentUnlockInfo[] array = Bedrock.ListUnlockedContent(Bedrock.brLobbyServerTier.BR_LOBBY_SERVER_FRANCHISE, 256u);
				Bedrock.brContentUnlockInfo[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					Bedrock.brContentUnlockInfo brContentUnlockInfo = array2[i];
					if (MatchesToyAndSubtype(brContentUnlockInfo.contentKey, brContentUnlockInfo.subType) && !IsToyLinked)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public string ToyLinkDisplayName
	{
		get
		{
			return LocalizedName;
		}
	}

	public string ToyLinkCardInstructionText
	{
		get
		{
			return LocalizationManager.Instance.GetString("TOY_LINK_INSTRUCTION_MAGIC_ITEM");
		}
	}

	public void LoadPowerupPrefabFromResources()
	{
		string resourcePath = string.Format("PowerupPrefabs/{0}", storageKey);
		PowerupPrefab = ResourceUtils.LoadResource<GameObject>(resourcePath);
	}

	public void ReleasePowerupPrefab()
	{
		if (PowerupPrefab != null)
		{
			PowerupPrefab = null;
		}
	}

	public float GetValueForLevel(int level)
	{
		switch (level)
		{
		case 1:
			return level1_value;
		case 2:
			return level2_value;
		case 3:
			return level3_value;
		case 4:
			return level4_value;
		case 5:
			return level5_value;
		default:
			return level1_value;
		}
	}

	public int GetLevel()
	{
		return 1;
	}

	public int GetUpgradeCost(int level)
	{
		if (canUpgrade)
		{
			switch (level)
			{
			case 1:
				return level1_upgradeCost;
			case 2:
				return level2_upgradeCost;
			case 3:
				return level3_upgradeCost;
			case 4:
				return level4_upgradeCost;
			}
		}
		return 0;
	}

	public int GetScoreModifier(int level)
	{
		if (canUpgrade)
		{
			switch (level)
			{
			case 1:
				return level1_scoreModifier;
			case 2:
				return level2_scoreModifier;
			case 3:
				return level3_scoreModifier;
			case 4:
				return level4_scoreModifier;
			case 5:
				return level5_scoreModifier;
			}
		}
		return level1_scoreModifier;
	}

	public void UpdateDataFromSwrve()
	{
		Dictionary<string, string> resourceDictionary;
		if (Bedrock.GetRemoteUserResources(storageKey, out resourceDictionary))
		{
			cost = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "Cost", cost);
			consumableCost = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "consumableCost", consumableCost);
			level1_value = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "level1_value", level1_value);
			level1_upgradeCost = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "level1_upgradeCost", level1_upgradeCost);
			level1_scoreModifier = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "level1_scoreModifier", level1_scoreModifier);
			level2_value = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "level2_value", level2_value);
			level2_upgradeCost = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "level2_upgradeCost", level2_upgradeCost);
			level2_scoreModifier = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "level2_scoreModifier", level2_scoreModifier);
			level3_value = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "level3_value", level3_value);
			level3_upgradeCost = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "level3_upgradeCost", level3_upgradeCost);
			level3_scoreModifier = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "level3_scoreModifier", level3_scoreModifier);
			level4_value = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "level4_value", level4_value);
			level4_upgradeCost = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "level4_upgradeCost", level4_upgradeCost);
			level4_scoreModifier = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "level4_scoreModifier", level4_scoreModifier);
			level5_value = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "level5_value", level5_value);
			level5_upgradeCost = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "level5_upgradeCost", level5_upgradeCost);
			level5_scoreModifier = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "level5_scoreModifier", level5_scoreModifier);
			unlockSaleText = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "unlockSaleText", unlockSaleText);
			isUnlockOnSale = ((!(unlockSaleText == "0")) ? true : false);
			_log.LogDebug("unlockOnSaleText for {0} is {1}", readableName, unlockSaleText);
			_log.LogDebug("isUnlockOnSale for {0} is {1}", readableName, isUnlockOnSale);
			upgradeSaleText = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "upgradeSaleText", upgradeSaleText);
			isUpgradeOnSale = ((!(upgradeSaleText == "0")) ? true : false);
			_log.LogDebug("upgradeOnSaleText for {0} is {1}", readableName, upgradeSaleText);
			_log.LogDebug("isUpgradeOnSale for {0} is {1}", readableName, isUpgradeOnSale);
		}
	}

	public int GetAttribute(string key, int inValue)
	{
		Dictionary<string, string> resourceDictionary;
		if (Bedrock.GetRemoteUserResources(storageKey, out resourceDictionary))
		{
			return Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, key, inValue);
		}
		return inValue;
	}

	public string GetAttribute(string key, string inValue)
	{
		Dictionary<string, string> resourceDictionary;
		if (Bedrock.GetRemoteUserResources(storageKey, out resourceDictionary))
		{
			return Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, key, inValue);
		}
		return inValue;
	}

	public float GetAttribute(string key, float inValue)
	{
		Dictionary<string, string> resourceDictionary;
		if (Bedrock.GetRemoteUserResources(storageKey, out resourceDictionary))
		{
			return Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, key, inValue);
		}
		return inValue;
	}

	public List<Powerup> Trigger(PowerupHolder powerupHolder, bool isBonus)
	{
		_log.LogDebug("Trigger({0}, {1})", (!(powerupHolder == null)) ? powerupHolder.name : "(null)", isBonus);
		int num = 1;
		if (storageKey == "ghostswords" && PqmtScreenGestures.m_instancePQMT != null)
		{
			num = PqmtScreenGestures.m_instancePQMT.mMaxFingers;
		}
		List<Powerup> list = new List<Powerup>();
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(PowerupPrefab);
			Powerup component = gameObject.GetComponent<Powerup>();
			component.Holder = powerupHolder;
			component.PowerupData = this;
			component.IsBonus = isBonus;
			component.SetLevel(1, GetValueForLevel(1));
			component.DragMultiTargetIndex = i;
			component.OnPowerupTriggered();
			list.Add(component);
		}
		return list;
	}

	public bool MatchesToyAndSubtype(uint linkToyId, uint linkToySubtypeId)
	{
		switch (linkToyId)
		{
		case 206u:
			linkToyId = 202u;
			break;
		case 202u:
			linkToyId = 206u;
			break;
		}
		return toyId == linkToyId;
	}

	public void UnlockFromToy(uint linkToySubtypeId)
	{
		_log.LogDebug("Checking for refund for '{0}'", LocalizedName);
	}
}
