using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : ScriptableObject, ILinkable
{
	public enum FXOverlayType
	{
		Idle = 0,
		MuzzleFlash = 1,
		Tracer = 2,
		HitEffect = 3,
		KillEffect = 4
	}

	public enum GenderType
	{
		MALE = 0,
		FEMALE = 1
	}

	public enum ToyRelease
	{
		SPYROSADVENTURE = 0,
		GIANTS = 1,
		SERIES2 = 2,
		LIGHTCORE = 3
	}

	public const string StaticModelResourcePath = "CharacterStaticModels";

	public const string RiggedModelResourcePath = "CharacterRiggedModels";

	public const string UnlockEffectResourcePath = "UnlockEffectModels";

	public const string AudioResourcePath = "CharacterAudioResources";

	public const string FXOverlaysResourcePath = "FXOverlays";

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(CharacterData), LogLevel.Debug);

	public string charName;

	public ElementData elementData;

	public string magicMomentScene;

	public string staticModelResourceName;

	public string riggedModelResourceName;

	public string unlockEffectResourceName;

	public string audioResourceName;

	public CharacterUpgradeData passiveUpgrade;

	public Vector3 detailsPosition = Vector3.zero;

	public Vector3 detailsScale = Vector3.one;

	public Vector3 detailsRotation = Vector3.zero;

	public Vector3 loadoutPosition = Vector3.zero;

	public Vector3 loadoutScale = Vector3.one;

	public Vector3 loadoutRotation = Vector3.zero;

	public string movieIntro;

	public bool isSwapForce;

	[NonSerialized]
	private GameObject _staticModelPrefab;

	[NonSerialized]
	private GameObject _riggedModelPrefab;

	[NonSerialized]
	private GameObject _unlockEffectPrefab;

	[NonSerialized]
	public CharacterAudioResources _audioResources;

	public float scrollListPosition;

	public int ToyId = -1;

	public int SubType = -1;

	public bool isReleased = true;

	public bool FeatureSkylanders2011;

	public bool FeatureSkylanders2012;

	public bool FeatureSkylanders2012LightCore;

	public bool isGiant;

	public bool isLegendary;

	public bool isNew;

	[NonSerialized]
	private GameObject[] _fxOverlays = new GameObject[5];

	public string AmazonASIN;

	public GenderType Gender;

	public ToyRelease[] ToyTypes;

	public CharacterAudioResources AudioResources
	{
		get
		{
			if (_audioResources == null)
			{
				string path = "CharacterAudioResources/" + audioResourceName;
				_audioResources = (CharacterAudioResources)Resources.Load(path, typeof(CharacterAudioResources));
			}
			return _audioResources;
		}
	}

	public int GemCost
	{
		get
		{
			int num = ((!isGiant) ? SwrveEconomy.GlobalSkylanderGemCost : SwrveEconomy.GlobalSkylanderGiantGemCost);
			Dictionary<string, string> resourceDictionary;
			if (Bedrock.GetRemoteUserResources(charName, out resourceDictionary))
			{
				num = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "gemCost", num);
			}
			return num;
		}
	}

	public int UpgradeCost
	{
		get
		{
			if (passiveUpgrade == null)
			{
				return 0;
			}
			int num = passiveUpgrade.unlockCost;
			Dictionary<string, string> resourceDictionary;
			if (Bedrock.GetRemoteUserResources(charName, out resourceDictionary))
			{
				num = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "upgradecost", num);
			}
			return num;
		}
	}

	public bool IsUnlockOnSale
	{
		get
		{
			return UnlockSaleText != "0";
		}
	}

	public string UnlockSaleText
	{
		get
		{
			Dictionary<string, string> resourceDictionary;
			if (Bedrock.GetRemoteUserResources(charName, out resourceDictionary))
			{
				return Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "unlockSaleText", "0");
			}
			return "0";
		}
	}

	public bool IsUpgradeOnSale
	{
		get
		{
			return UpgradeSaleText != "0";
		}
	}

	public string UpgradeSaleText
	{
		get
		{
			Dictionary<string, string> resourceDictionary;
			if (Bedrock.GetRemoteUserResources(charName, out resourceDictionary))
			{
				return Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "upgradeSaleText", "0");
			}
			return "0";
		}
	}

	public bool IsSpellOnSale
	{
		get
		{
			Dictionary<string, string> resourceDictionary;
			if (Bedrock.GetRemoteUserResources(charName, out resourceDictionary))
			{
				return Bedrock.GetFromResourceDictionaryAsBool(resourceDictionary, "spellSaleText", false);
			}
			return false;
		}
	}

	public bool IsReleased
	{
		get
		{
			Dictionary<string, string> resourceDictionary;
			if (Bedrock.GetRemoteUserResources(charName, out resourceDictionary))
			{
				return Bedrock.GetFromResourceDictionaryAsBool(resourceDictionary, "isReleased", isReleased);
			}
			return isReleased;
		}
	}

	public bool IsNew
	{
		get
		{
			Dictionary<string, string> resourceDictionary;
			if (Bedrock.GetRemoteUserResources(charName, out resourceDictionary))
			{
				return Bedrock.GetFromResourceDictionaryAsBool(resourceDictionary, "isNew", isNew);
			}
			return isNew;
		}
	}

	public string ToyLinkDisplayName
	{
		get
		{
			return charName;
		}
	}

	public string ToyLinkCardInstructionText
	{
		get
		{
			return LocalizationManager.Instance.GetString("TOY_LINK_INSTRUCTION_SKYLANDER");
		}
	}

	public GameObject GetStaticModelPrefab()
	{
		if (_staticModelPrefab == null)
		{
			string text = "CharacterStaticModels/" + staticModelResourceName;
			_log.Log("Loading static model resource: '{0}' for '{1}'", text, staticModelResourceName);
			_staticModelPrefab = (GameObject)Resources.Load(text, typeof(GameObject));
		}
		return _staticModelPrefab;
	}

	public GameObject GetRiggedModelPrefab()
	{
		if (_riggedModelPrefab == null)
		{
			string text = "CharacterRiggedModels/" + riggedModelResourceName;
			_log.Log("Loading rigged model resource: {0}", text);
			_riggedModelPrefab = (GameObject)Resources.Load(text, typeof(GameObject));
		}
		return _riggedModelPrefab;
	}

	public GameObject GetUnlockEffectPrefab()
	{
		if (_unlockEffectPrefab == null)
		{
			string path = "UnlockEffectModels/" + unlockEffectResourceName;
			_unlockEffectPrefab = (GameObject)Resources.Load(path, typeof(GameObject));
		}
		return _unlockEffectPrefab;
	}

	public void LoadFXOverlayPrefabs()
	{
		string text = "FXOverlays/" + riggedModelResourceName + "_FXOverlay";
		if (_fxOverlays[0] == null)
		{
			_fxOverlays[0] = (GameObject)Resources.Load(text + "Idle", typeof(GameObject));
		}
		if (_fxOverlays[1] == null)
		{
			_fxOverlays[1] = (GameObject)Resources.Load(text + "MuzzleFlash", typeof(GameObject));
		}
		if (_fxOverlays[2] == null)
		{
			_fxOverlays[2] = (GameObject)Resources.Load(text + "Tracer", typeof(GameObject));
		}
		if (_fxOverlays[3] == null)
		{
			_fxOverlays[3] = (GameObject)Resources.Load(text + "HitEffect", typeof(GameObject));
		}
		if (_fxOverlays[4] == null)
		{
			_fxOverlays[4] = (GameObject)Resources.Load(text + "KillEffect", typeof(GameObject));
		}
	}

	public GameObject GetFXOverlayPrefab(FXOverlayType overlayType)
	{
		return _fxOverlays[(int)overlayType];
	}

	public void ReleaseReferencesToResources()
	{
		_audioResources = null;
		_unlockEffectPrefab = null;
		_riggedModelPrefab = null;
		_staticModelPrefab = null;
		passiveUpgrade.ReleaseIconResources();
	}

	public IEnumerator PlayFXOverlay(FXOverlayType overlayType, Vector3 position, Quaternion rotation)
	{
		GameObject fxOverlay = _fxOverlays[(int)overlayType];
		if (fxOverlay != null)
		{
			GameObject instance = UnityEngine.Object.Instantiate(fxOverlay, position, rotation) as GameObject;
			ParticleSystem particleSystem = instance.GetComponent<ParticleSystem>();
			if (particleSystem != null)
			{
				particleSystem.Play(true);
			}
			UnityEngine.Object.Destroy(instance, 2f);
		}
		yield return null;
	}

	public void InitCharacterUpgradeData()
	{
		_log.Log("Initialize upgrade data for '{0}'.", charName);
		passiveUpgrade.Initialize(this);
	}

	public bool MatchesToyAndSubtype(uint linkedToyId, uint linkedToySubtypeId)
	{
		if (linkedToyId != ToyId)
		{
			return false;
		}
		if (SubType < 0)
		{
			return true;
		}
		if (linkedToySubtypeId == 0 && !FeatureSkylanders2011)
		{
			return false;
		}
		if (isLegendary)
		{
			return SubType == linkedToySubtypeId;
		}
		return (uint)(SubType | (int)linkedToySubtypeId) == SubType;
	}

	public void UnlockFromToy(uint linkToySubtypeId)
	{
		_log.LogDebug("UnlockFromToy({0})", linkToySubtypeId);
		CharacterUserData characterUserData = new CharacterUserData(this);
		CharacterUserData.ToyLink toyLink = CharacterUserData.RawSubTypeToToyLinkFlag(linkToySubtypeId);
		if (Application.isEditor)
		{
			toyLink = (CharacterUserData.ToyLink)linkToySubtypeId;
		}
		if ((toyLink != CharacterUserData.ToyLink.Skylanders2011 || !FeatureSkylanders2011) && (toyLink != CharacterUserData.ToyLink.Skylanders2012 || !FeatureSkylanders2012) && (toyLink != CharacterUserData.ToyLink.Skylanders2012LightCore || !FeatureSkylanders2012LightCore))
		{
			_log.LogError("This character is being linked with a toy of type '{0}', but the data says it doesn't support that type!", toyLink.ToString());
		}
		int? amountUserSpentToUnlock = characterUserData.AmountUserSpentToUnlock;
		_log.LogDebug("Original Unlock Price = '{0}'", amountUserSpentToUnlock);
		if (amountUserSpentToUnlock != -1)
		{
			_log.LogDebug("Skylander was not previously linked...");
			if (amountUserSpentToUnlock.HasValue && (amountUserSpentToUnlock.GetValueOrDefault() != 0 || !amountUserSpentToUnlock.HasValue))
			{
				_log.LogDebug("Toy was previosuly unlocked, but with non-0, non-link value. Giving gem refund.");
				SwrveEventsProgression.ToyRegistration(amountUserSpentToUnlock.Value, charName, true);
				SwrveEventsRewards.AwardGems(amountUserSpentToUnlock.Value, "SkylanderRefund");
			}
			else
			{
				_log.LogDebug("Toy was not previously or originally unlocked for 0 gems. No refund awarded.");
				SwrveEventsProgression.ToyRegistration(0, charName, true);
			}
		}
		else
		{
			_log.LogDebug("Toy was previously linked. Not giving a refund.");
		}
		_log.Log("Unlocking character '{0}' with toyLinkFlag '{1}'", charName, toyLink);
		characterUserData.UnlockCharacter(-1, toyLink);
	}
}
