using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BountyChooser : SingletonMonoBehaviour
{
	public const int NumberOfActiveBounties = 3;

	public const string ShootBountyTypeName = "Shoot";

	public const string CollectCoinsBountyTypeName = "CollectCoins";

	public const string UseMagicItemsBountyTypeName = "UseMagicItem";

	public const string SurviveBountyTypeName = "Survive";

	public const string ComboBountyTypeName = "Combo";

	public const string AvoidDestructiblesBountyTypeName = "AvoidDestructibles";

	public const string StartGameBountyTypeName = "StartGame";

	public const string ClearInOneShotBountyTypeName = "ClearInOneShot";

	public const string ManualTriggerBountyTypeName = "Manual";

	public const string SplashDamageBountyTypeName = "SplashDamage";

	public const string CollectComboCoinsTypeName = "CollectComboCoins";

	private const int FixedBountyCountNumber = 3;

	public Texture2D[] bountyIcons;

	public PowerupList allPowerups;

	public PowerupData startingPowerup;

	public CharacterDataList allCharacters;

	public GameObject hatPrefab;

	private Bounty[] _activeBounties;

	private HashSet<int> _completedBountyIds = new HashSet<int>();

	private static BountyData[] _bountyDefinitions;

	private static int _bountyDataVersion;

	public static BountyChooser Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<BountyChooser>();
		}
	}

	public Bounty[] ActiveBounties
	{
		get
		{
			return _activeBounties;
		}
	}

	public static event EventHandler<BountyChangeEventArgs> BountyChanged;

	protected override void AwakeOnce()
	{
		base.AwakeOnce();
		_activeBounties = new Bounty[3];
		if (_bountyDefinitions == null)
		{
			_bountyDefinitions = BountyData.LoadAllBountiesFromResources();
		}
		UpdateDefinitionsFromSwrveIfNewer();
	}

	private void HandleStorageChanged(object sender, EventArgs e)
	{
		_activeBounties = new Bounty[3];
		LoadBountiesFromStorage();
	}

	private void UpdateDefinitionsFromSwrveIfNewer()
	{
		int remoteVariableAsInt = Bedrock.GetRemoteVariableAsInt("override.bounty.version", 0);
		Debug.Log("Swrve bounty data version - " + remoteVariableAsInt);
		if (remoteVariableAsInt != _bountyDataVersion)
		{
			Debug.Log("Updating all bounties from swrve.");
			BountyData[] bountyDefinitions = _bountyDefinitions;
			foreach (BountyData bountyData in bountyDefinitions)
			{
				bountyData.LoadOverrideDataFromSwrve();
			}
			_bountyDataVersion = remoteVariableAsInt;
		}
	}

	private Bounty InstantiateBounty(BountyData bountyData)
	{
		Bounty bounty = GenerateBaseBounty(bountyData);
		if (bountyData.SingleRunModifier)
		{
			bounty.gameObject.AddComponent<InSingleRunModifier>();
		}
		if (!string.IsNullOrEmpty(bountyData.ElementModifierData))
		{
			CorrectElementModifier correctElementModifier = bounty.gameObject.AddComponent<CorrectElementModifier>();
			Elements.Type value;
			if (EnumUtils.TryParse<Elements.Type>(bountyData.ElementModifierData, out value))
			{
				correctElementModifier.SetElement(value);
			}
		}
		if (!string.IsNullOrEmpty(bountyData.SkylanderModifierData))
		{
			CorrectSkylanderModifier correctSkylanderModifier = bounty.gameObject.AddComponent<CorrectSkylanderModifier>();
			correctSkylanderModifier.SetSkylanderFromData(bountyData.SkylanderModifierData);
		}
		if (bountyData.IsUnlockedSkylanderModifier)
		{
			bounty.gameObject.AddComponent<IsUnlockedSkylanderModifier>();
		}
		if (bountyData.HasUnlockedMagicItemModifier)
		{
			bounty.gameObject.AddComponent<HasUnlockedMagicItemModifier>();
		}
		if (bountyData.HasMagicItemWithLevelModifierData.HasValue)
		{
			HasMagicItemWithLevelModifier hasMagicItemWithLevelModifier = bounty.gameObject.AddComponent<HasMagicItemWithLevelModifier>();
			hasMagicItemWithLevelModifier.MinimumLevel = bountyData.HasMagicItemWithLevelModifierData.Value;
		}
		if (bountyData.IsUsingBonusElementModifierData.HasValue)
		{
			UsingBonusElementModifier usingBonusElementModifier = bounty.gameObject.AddComponent<UsingBonusElementModifier>();
			usingBonusElementModifier.RequireCorrectElement = bountyData.IsUsingBonusElementModifierData.Value;
		}
		if (!string.IsNullOrEmpty(bountyData.SpecificMagicItemModifierData))
		{
			bounty.gameObject.AddComponent<CorrectMagicItemModifier>();
		}
		if (bountyData.IsMagicItemOfLevelModidiferData.HasValue)
		{
			IsMagicItemAtLeastLevelModifier isMagicItemAtLeastLevelModifier = bounty.gameObject.AddComponent<IsMagicItemAtLeastLevelModifier>();
			isMagicItemAtLeastLevelModifier.MinimumItemLevel = bountyData.IsMagicItemOfLevelModidiferData.Value;
		}
		if (!string.IsNullOrEmpty(bountyData.ShootEnemyModifierData))
		{
			IsEnemyModifier isEnemyModifier = bounty.gameObject.AddComponent<IsEnemyModifier>();
			isEnemyModifier.SetEnemyType(bountyData.ShootEnemyModifierData);
		}
		if (bountyData.ShootHatModifier)
		{
			bounty.gameObject.AddComponent<HasHatModifier>();
		}
		if (bountyData.ShootBalloonModifier)
		{
			bounty.gameObject.AddComponent<IsBalloonModifier>();
		}
		if (!string.IsNullOrEmpty(bountyData.ShootDestructibleModifierData))
		{
			IsDestructibleModifier isDestructibleModifier = bounty.gameObject.AddComponent<IsDestructibleModifier>();
			Destructible.Type value2;
			if (EnumUtils.TryParse<Destructible.Type>(bountyData.ShootDestructibleModifierData, out value2))
			{
				isDestructibleModifier.RequiredType = value2;
			}
			else
			{
				isDestructibleModifier.RequiredType = null;
			}
		}
		if (bountyData.ShootProjectileModifier)
		{
			bounty.gameObject.AddComponent<IsProjectileModifier>();
		}
		if (bountyData.ShootSheepModifier)
		{
			bounty.gameObject.AddComponent<IsSheepModifier>();
		}
		if (bountyData.ShootPresentModifier)
		{
			bounty.gameObject.AddComponent<IsPresentModifier>();
		}
		if (bountyData.IsBossRoomModifier)
		{
			bounty.gameObject.AddComponent<IsBossRoomModifier>();
		}
		if (!string.IsNullOrEmpty(bountyData.ElementalComboCoinModifierData))
		{
			IsElementalComboCoinModifier isElementalComboCoinModifier = bounty.gameObject.AddComponent<IsElementalComboCoinModifier>();
			Elements.Type value3;
			if (EnumUtils.TryParse<Elements.Type>(bountyData.ElementalComboCoinModifierData, out value3))
			{
				isElementalComboCoinModifier.RequiredElement = value3;
			}
			else
			{
				isElementalComboCoinModifier.RequiredElement = null;
			}
		}
		bounty.Initialize();
		return bounty;
	}

	private Bounty GenerateBaseBounty(BountyData bountyData)
	{
		GameObject gameObject = new GameObject("Bounty");
		switch (bountyData.ScriptName)
		{
		case "Shoot":
			gameObject.AddComponent<HealthKilledBounty>();
			break;
		case "CollectCoins":
			gameObject.AddComponent<CollectCoinsBounty>();
			break;
		case "UseMagicItem":
			gameObject.AddComponent<UseMagicItemBounty>();
			break;
		case "Survive":
			gameObject.AddComponent<SurviveBounty>();
			break;
		case "Combo":
		{
			ComboBounty comboBounty = gameObject.AddComponent<ComboBounty>();
			comboBounty.ParseFromBountyParams(bountyData.ScriptParams);
			break;
		}
		case "AvoidDestructibles":
			gameObject.AddComponent<AvoidDestructiblesBounty>();
			break;
		case "StartGame":
			gameObject.AddComponent<StartGameBounty>();
			break;
		case "ClearInOneShot":
			gameObject.AddComponent<ClearInOneShotBounty>();
			break;
		case "Manual":
		{
			ManualTriggerBounty manualTriggerBounty = gameObject.AddComponent<ManualTriggerBounty>();
			manualTriggerBounty.TriggerId = bountyData.ScriptParams;
			break;
		}
		case "SplashDamage":
			gameObject.AddComponent<SplashDamageKilledBounty>();
			break;
		case "CollectComboCoins":
			gameObject.AddComponent<CollectComboCoinsBounty>();
			break;
		default:
			UnityEngine.Object.Destroy(gameObject);
			throw new Exception("Can't determine script type from bounty type '" + bountyData.ScriptName + "'. Did it get removed from the list of valid types? Why is it in there?");
		}
		gameObject.name = string.Format("{0}_{1}_{2}_{3}", bountyData.Id, bountyData.ScriptName, bountyData.Goal, bountyData.Reward);
		Bounty component = gameObject.GetComponent<Bounty>();
		gameObject.transform.parent = base.transform;
		component.bountyData = bountyData;
		component.iconTexture = GetTextureFor(bountyData);
		component.LoadFromSaveState(bountyData.ScriptParams);
		return component;
	}

	private Texture2D GetTextureFor(BountyData data)
	{
		string bountyIconTextureName = data.BountyIconTextureName;
		Texture2D[] array = bountyIcons;
		foreach (Texture2D texture2D in array)
		{
			if (texture2D.name.Equals(bountyIconTextureName))
			{
				return texture2D;
			}
		}
		Debug.LogError("Unable to find icon '" + bountyIconTextureName + "' associated with short name '" + data.IconSuffix + "'");
		return null;
	}

	public Bounty SetBounty(int bountySlot, BountyData bountyData)
	{
		Bounty bounty = _activeBounties[bountySlot];
		if (bounty != null)
		{
			Debug.Log("Destroying old bounty - " + bounty.bountyData);
			UnityEngine.Object.Destroy(bounty.gameObject);
		}
		Bounty bounty2 = InstantiateBounty(bountyData);
		if (bounty2 == null)
		{
			Debug.LogError("Unable to instantiate new bounty!... Ruh Roh!");
			return null;
		}
		_activeBounties[bountySlot] = bounty2;
		OnBountyChanged(bountySlot, bounty, bounty2);
		return bounty2;
	}

	public Bounty ChooseNewBounty(int bountySlot)
	{
		int num = 0;
		BountyData bountyData = ((num >= 3) ? ChooseRandomBounty() : _bountyDefinitions[num]);
		SwrveEventsProgression.GoalNew(bountyData.Id);
		return SetBounty(bountySlot, bountyData);
	}

	private BountyData ChooseRandomBounty()
	{
		HashSet<string> forbiddenUniquenessKeys = new HashSet<string>();
		HashSet<int> forbiddenIds = new HashSet<int>();
		RankData currentRank = RankDataManager.Instance.CurrentRank.Rank;
		bool playerIsAtMaxRank = RankDataManager.Instance.IsFinalRank(currentRank);
		for (int i = 0; i < 3; i++)
		{
			forbiddenIds.Add(_bountyDefinitions[i].Id);
		}
		if (!playerIsAtMaxRank)
		{
			forbiddenIds.UnionWith(_completedBountyIds);
		}
		Bounty[] activeBounties = _activeBounties;
		foreach (Bounty bounty in activeBounties)
		{
			if (bounty != null)
			{
				forbiddenUniquenessKeys.Add(bounty.bountyData.UniquenessGroup);
				forbiddenIds.Add(bounty.bountyData.Id);
			}
		}
		BountyData[] array = (from data in _bountyDefinitions
			where IsAllowedRank(data, currentRank.RankNumber, playerIsAtMaxRank)
			where !forbiddenIds.Contains(data.Id)
			where !forbiddenUniquenessKeys.Contains(data.UniquenessGroup)
			select data).ToArray();
		if (!array.Any())
		{
			Debug.LogError("No valid bounty candidates exist after filtering... Choosing randomly from ALL bounties.");
			array = _bountyDefinitions.ToArray();
		}
		int num = UnityEngine.Random.Range(0, array.Length);
		return array[num];
	}

	private static bool IsAllowedRank(BountyData candidate, int currentRankNumber, bool playerIsAtMaxRank)
	{
		return (currentRankNumber >= candidate.MinRank && currentRankNumber <= candidate.MaxRank) || (playerIsAtMaxRank && candidate.MinRank == 999);
	}

	public void LoadBountiesFromStorage()
	{
	}

	public BountyData GetBountyDataForId(int bountyId)
	{
		BountyData[] bountyDefinitions = _bountyDefinitions;
		foreach (BountyData bountyData in bountyDefinitions)
		{
			if (bountyData.Id == bountyId)
			{
				return bountyData;
			}
		}
		return null;
	}

	public void SaveBountyProgress(int index)
	{
	}

	public Bounty[] GetCurrentBounties()
	{
		return _activeBounties;
	}

	public void RememberBountyComplete(BountyData bountyData)
	{
		_completedBountyIds.Add(bountyData.Id);
	}

	private void OnBountyChanged(int bountyIndex, Bounty oldBounty, Bounty newBounty)
	{
		if (BountyChooser.BountyChanged != null)
		{
			BountyChooser.BountyChanged(this, new BountyChangeEventArgs(bountyIndex, oldBounty, newBounty));
		}
	}
}