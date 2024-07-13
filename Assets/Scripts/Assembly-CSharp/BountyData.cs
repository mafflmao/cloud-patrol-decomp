using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BountyData
{
	public const string CsvFilePath = "Bounties";

	public const string BountyIconTexturePrefix = "BountyIcon_";

	public const int MinRankSettingForLevelCapBounty = 999;

	private const int HighLevelGoalIdStart = 100000;

	private const int IdColumn = 0;

	private const int ScriptNameColumn = 1;

	private const int GoalColumn = 7;

	private const int HighLevelGoalColumn = 8;

	private const int MinRankColumn = 5;

	private const int MaxRankColumn = 6;

	public const string BedrockBountyVersionKey = "override.bounty.version";

	private const string BedrockBountyOverrideIdPrefix = "override.bounty.";

	private const string BedrockDescriptionKey = "bountyDescription";

	private const string BedrockUniquenessGroupKey = "uniquenessGroup";

	private const string BedrockIconSuffixKey = "iconSuffix";

	private const string BedrockMinRankKey = "minRank";

	private const string BedrockMaxRankKey = "maxRank";

	private const string BedrockGoalKey = "goal";

	private const string BedrockHighLevelGoal = "highLevelGoal";

	private const string BedrockRewardKey = "reward";

	private const string BedrockScriptParamsKey = "scriptParams";

	public int Id { get; private set; }

	public string ScriptName { get; private set; }

	public string Description { get; private set; }

	public string UniquenessGroup { get; private set; }

	public string IconSuffix { get; private set; }

	public int Goal { get; private set; }

	public int HighLevelGoal { get; private set; }

	public int Reward { get; private set; }

	public string ScriptParams { get; private set; }

	public int MinRank { get; private set; }

	public int MaxRank { get; private set; }

	public bool SingleRunModifier { get; private set; }

	public string ElementModifierData { get; private set; }

	public string SkylanderModifierData { get; private set; }

	public bool IsUnlockedSkylanderModifier { get; private set; }

	public bool HasUnlockedMagicItemModifier { get; private set; }

	public int? HasMagicItemWithLevelModifierData { get; private set; }

	public bool? IsUsingBonusElementModifierData { get; private set; }

	public string SpecificMagicItemModifierData { get; private set; }

	public int? IsMagicItemOfLevelModidiferData { get; private set; }

	public string ShootEnemyModifierData { get; private set; }

	public bool ShootHatModifier { get; private set; }

	public bool ShootBalloonModifier { get; private set; }

	public string ShootDestructibleModifierData { get; private set; }

	public bool ShootProjectileModifier { get; private set; }

	public bool ShootSheepModifier { get; private set; }

	public bool ShootPresentModifier { get; private set; }

	public bool IsBossRoomModifier { get; private set; }

	public string ElementalComboCoinModifierData { get; private set; }

	public string BountyIconTextureName
	{
		get
		{
			return "BountyIcon_" + IconSuffix;
		}
	}

	public string LocalizedDescription
	{
		get
		{
			return LocalizationManager.Instance.GetString(Description);
		}
	}

	public BountyData(List<string> csvData)
	{
		int num = 0;
		Id = int.Parse(CsvUtilities.GetValueFromListOrNull(csvData, num++));
		ScriptName = CsvUtilities.GetValueFromListOrNull(csvData, num++);
		Description = CsvUtilities.GetValueFromListOrNull(csvData, num++);
		UniquenessGroup = CsvUtilities.GetValueFromListOrNull(csvData, num++);
		IconSuffix = CsvUtilities.GetValueFromListOrNull(csvData, num++);
		MinRank = int.Parse(CsvUtilities.GetValueFromListOrNull(csvData, num++));
		MaxRank = int.Parse(CsvUtilities.GetValueFromListOrNull(csvData, num++));
		Goal = int.Parse(CsvUtilities.GetValueFromListOrNull(csvData, num++));
		HighLevelGoal = int.Parse(CsvUtilities.GetValueFromListOrNull(csvData, num++));
		Reward = int.Parse(CsvUtilities.GetValueFromListOrNull(csvData, num++));
		ScriptParams = CsvUtilities.GetValueFromListOrNull(csvData, num++);
		SingleRunModifier = !CsvUtilities.IsValueEmptyOrMissing(csvData, num++);
		ElementModifierData = CsvUtilities.GetValueFromListOrNull(csvData, num++);
		SkylanderModifierData = CsvUtilities.GetValueFromListOrNull(csvData, num++);
		IsUnlockedSkylanderModifier = !CsvUtilities.IsValueEmptyOrMissing(csvData, num++);
		HasUnlockedMagicItemModifier = !CsvUtilities.IsValueEmptyOrMissing(csvData, num++);
		HasMagicItemWithLevelModifierData = CsvUtilities.GetIntValueFromListOrNull(csvData, num++);
		IsUsingBonusElementModifierData = CsvUtilities.GetBoolValueFromListOrNull(csvData, num++);
		SpecificMagicItemModifierData = CsvUtilities.GetValueFromListOrNull(csvData, num++);
		IsMagicItemOfLevelModidiferData = CsvUtilities.GetIntValueFromListOrNull(csvData, num++);
		ShootEnemyModifierData = CsvUtilities.GetValueFromListOrNull(csvData, num++);
		ShootHatModifier = !CsvUtilities.IsValueEmptyOrMissing(csvData, num++);
		ShootBalloonModifier = !CsvUtilities.IsValueEmptyOrMissing(csvData, num++);
		ShootDestructibleModifierData = CsvUtilities.GetValueFromListOrNull(csvData, num++);
		ShootProjectileModifier = !CsvUtilities.IsValueEmptyOrMissing(csvData, num++);
		ShootSheepModifier = !CsvUtilities.IsValueEmptyOrMissing(csvData, num++);
		ShootPresentModifier = !CsvUtilities.IsValueEmptyOrMissing(csvData, num++);
		IsBossRoomModifier = !CsvUtilities.IsValueEmptyOrMissing(csvData, num++);
		ElementalComboCoinModifierData = CsvUtilities.GetValueFromListOrNull(csvData, num++);
	}

	public static BountyData[] LoadAllBountiesFromResources()
	{
		List<List<string>> source = CsvUtilities.LoadCsvDataFromResource("Bounties");
		return (from data in (from csvLine in source.Skip(1)
				where 1 < csvLine.Count && !string.IsNullOrEmpty(csvLine[1])
				select csvLine).SelectMany((List<string> csvLine) => GenerateBountyDataFromLine(csvLine))
			orderby data.Id
			select data).ToArray();
	}

	private static IEnumerable<BountyData> GenerateBountyDataFromLine(List<string> csvLine)
	{
		BountyData baseBountyData = new BountyData(csvLine);
		yield return baseBountyData;
		if (baseBountyData.HighLevelGoal > 0 && baseBountyData.MinRank != 999)
		{
			csvLine[0] = (100000 + baseBountyData.Id).ToString();
			csvLine[7] = csvLine[8];
			csvLine[8] = (-1).ToString();
			csvLine[5] = (baseBountyData.MinRank + 50).ToString();
			csvLine[6] = (baseBountyData.MaxRank + 50).ToString();
			yield return new BountyData(csvLine);
		}
	}

	public void LoadOverrideDataFromSwrve()
	{
		string itemId = "override.bounty." + Id;
		Dictionary<string, string> resourceDictionary;
		if (Bedrock.GetRemoteUserResources(itemId, out resourceDictionary))
		{
			Description = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "bountyDescription", Description);
			UniquenessGroup = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "uniquenessGroup", UniquenessGroup);
			IconSuffix = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "iconSuffix", IconSuffix);
			MinRank = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "minRank", MinRank);
			MaxRank = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "maxRank", MaxRank);
			Goal = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "goal", Goal);
			HighLevelGoal = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "goal", HighLevelGoal);
			Reward = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "reward", Reward);
			ScriptParams = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "scriptParams", ScriptParams);
			Debug.Log("Using SWRVE override data for bounty " + this);
		}
	}

	public override string ToString()
	{
		return string.Format("[BountyData: Id={0}, ScriptName={1}, Description={2}, UniquenessGroup={3}, IconSuffix={4}, Goal={5}, Reward={6}, ScriptParams={7}, MinRank={8}, MaxRank={9}, SingleRunModifier={10}, ElementModifierData={11}, SkylanderModifierData={12}, IsUnlockedSkylanderModifier={13}, HasUnlockedMagicItemModifier={14}, HasMagicItemWithLevelModifierData={15}, IsUsingBonusElementModifierData={16}, SpecificMagicItemModifierData={17}, IsMagicItemOfLevelModidiferData={18}, ShootEnemyModifierData={19}, ShootHatModifier={20}, ShootBalloonModifier={21}, ShootDestructibleModifierData={22}, ShootProjectileModifier={23}, ShootSheepModifier={24}, ShootPresentModifier={25}, IsBossRoomModifier={26}, BountyIconTextureName={27}]", Id, ScriptName, Description, UniquenessGroup, IconSuffix, Goal, Reward, ScriptParams, MinRank, MaxRank, SingleRunModifier, ElementModifierData, SkylanderModifierData, IsUnlockedSkylanderModifier, HasUnlockedMagicItemModifier, HasMagicItemWithLevelModifierData, IsUsingBonusElementModifierData, SpecificMagicItemModifierData, IsMagicItemOfLevelModidiferData, ShootEnemyModifierData, ShootHatModifier, ShootBalloonModifier, ShootDestructibleModifierData, ShootProjectileModifier, ShootSheepModifier, ShootPresentModifier, IsBossRoomModifier, BountyIconTextureName);
	}
}
