using System.Collections.Generic;

public class RankData
{
	public const int MaxNormalLevel = 50;

	private const string BedrockStarsForNextRankAttribute = "starsForNextRank";

	private const string BedrockMaxGameplayGemsAttribute = "maxGameplayGems";

	public int RankNumber { get; private set; }

	public int StarsForNextRank { get; private set; }

	private string BedrockOverrideIdentifier
	{
		get
		{
			return "rank.override." + RankNumber;
		}
	}

	public RankData(List<string> csvData)
	{
		int num = 0;
		RankNumber = int.Parse(CsvUtilities.GetValueFromListOrNull(csvData, num++));
		StarsForNextRank = CsvUtilities.GetIntValueFromListOrDefault(csvData, num++, 3);
		Dictionary<string, string> resourceDictionary;
		if (Bedrock.GetRemoteUserResources(BedrockOverrideIdentifier, out resourceDictionary))
		{
			StarsForNextRank = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "starsForNextRank", StarsForNextRank);
		}
	}

	public override string ToString()
	{
		return string.Format("[RankData: RankNumber={0}, StarsForNextRank={1}]", RankNumber, StarsForNextRank);
	}
}
