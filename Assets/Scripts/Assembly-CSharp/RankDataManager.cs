using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RankDataManager
{
	private const string CsvResourcePath = "Ranks";

	private static RankDataManager _instance;

	private List<RankData> _rankData;

	public static RankDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new RankDataManager();
			}
			return _instance;
		}
	}

	public RankAndStars CurrentRank
	{
		get
		{
			return new RankAndStars(GetRankData(0), 0);
		}
	}

	public bool CanAwardGameplayGem
	{
		get
		{
			return true;
		}
	}

	public int NumGameplayGemsAvailable
	{
		get
		{
			return SwrveEconomy.GemCapForCurrentRank;
		}
	}

	private RankDataManager()
	{
		List<List<string>> source = CsvUtilities.LoadCsvDataFromResource("Ranks");
		_rankData = new List<RankData>();
		foreach (List<string> item2 in source.Skip(1))
		{
			RankData item = new RankData(item2);
			_rankData.Add(item);
		}
		Debug.Log("Loaded " + _rankData.Count + " ranks..");
	}

	public RankData GetRankData(int rankNumber)
	{
		if (rankNumber >= _rankData.Count)
		{
			Debug.LogWarning("Rank '" + rankNumber + "' is past end of array! (" + _rankData.Count + ")");
			return _rankData[_rankData.Count - 1];
		}
		return _rankData[rankNumber];
	}

	public bool IsFinalRank(RankData rankData)
	{
		return rankData.RankNumber >= _rankData.Count - 1;
	}

	public void IncreaseStars(int delta)
	{
		RankAndStars currentRank = CurrentRank;
		int totalNumberOfStars = GetTotalNumberOfStars();
		int num = 0;
		RankAndStars rankAndStars;
		if (IsFinalRank(currentRank.Rank))
		{
			int num2 = currentRank.Stars + delta;
			if (num2 >= currentRank.Rank.StarsForNextRank)
			{
				num = SwrveEconomy.RankGemsAwarded;
				num2 -= currentRank.Rank.StarsForNextRank;
			}
			rankAndStars = new RankAndStars(currentRank.Rank, num2);
		}
		else
		{
			rankAndStars = GetRankAndStars(totalNumberOfStars + delta);
			int rankNumber = currentRank.Rank.RankNumber;
			for (int i = rankNumber + 1; i <= rankAndStars.Rank.RankNumber; i++)
			{
				num += SwrveEconomy.RankGemsAwarded;
			}
		}
		if (num > 0)
		{
			Debug.Log("Awarding " + num + " gems.");
			SwrveEventsRewards.AwardGems(num, "RankReward");
		}
		if (currentRank.Rank.RankNumber != rankAndStars.Rank.RankNumber)
		{
			Debug.Log("Rank up!");
			SwrveEventsProgression.RankAwarded(num);
		}
	}

	public bool TryAwardGameplayGem()
	{
		if (CanAwardGameplayGem)
		{
			int num = 1;
			GameManager.gemsCollectedInVoyage += num;
			SwrveEventsRewards.AwardGems(num, "CollectedInFlight");
			return true;
		}
		return false;
	}

	public bool TryAwardGameplayGem(int numGems)
	{
		if (numGems <= NumGameplayGemsAvailable)
		{
			GameManager.gemsCollectedInVoyage += numGems;
			SwrveEventsRewards.AwardGems(numGems, "CollectedInFlight");
			return true;
		}
		return false;
	}

	public int GetTotalNumberOfStars()
	{
		RankAndStars currentRank = CurrentRank;
		int rankNumber = currentRank.Rank.RankNumber;
		int stars = currentRank.Stars;
		int starsRequiredForRank = GetStarsRequiredForRank(rankNumber);
		return starsRequiredForRank + stars;
	}

	public int GetStarsRequiredForRank(int rank)
	{
		int num = 0;
		for (int i = 0; i < rank; i++)
		{
			num += GetRankData(i).StarsForNextRank;
		}
		return num;
	}

	public RankAndStars GetRankAndStars(int totalNumberOfStars)
	{
		int num = totalNumberOfStars;
		RankData rankData = Instance.GetRankData(0);
		while (num >= rankData.StarsForNextRank)
		{
			num -= rankData.StarsForNextRank;
			rankData = Instance.GetRankData(rankData.RankNumber + 1);
		}
		return new RankAndStars(rankData, num);
	}
}
