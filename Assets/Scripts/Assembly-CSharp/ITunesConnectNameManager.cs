public class ITunesConnectNameManager
{
	private const bool _useReleaseIdentifiers = true;

	private static string[] BaseLeaderboardIds = new string[3] { "highScore", "maxRooms", "coinsCollected" };

	private string[] _inAppPurcahseIdentifiers = new string[4] { "MarmosetGemsLevel1", "MarmosetGemsLevel2", "MarmosetGemsLevel3", "MarmosetGemsLevel4" };

	private Bedrock.brIAPProductCategory[] _inAppPurchaseCategories = new Bedrock.brIAPProductCategory[4];

	private string _highScoreLeaderbaordId;

	private string _maxRoomsLeaderboardId;

	private string _coinsCollectedLeaderboardId;

	private static ITunesConnectNameManager _instance;

	public static ITunesConnectNameManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ITunesConnectNameManager();
			}
			return _instance;
		}
	}

	public string[] IapProductIdentifiers
	{
		get
		{
			return _inAppPurcahseIdentifiers;
		}
	}

	public Bedrock.brIAPProductCategory[] IapProductCategories
	{
		get
		{
			return _inAppPurchaseCategories;
		}
	}

	public string HighScoreLeaderboardId
	{
		get
		{
			return _highScoreLeaderbaordId;
		}
	}

	public string MaxRoomsLeaderboardId
	{
		get
		{
			return _maxRoomsLeaderboardId;
		}
	}

	public string CoinsCollectedLeaderboardId
	{
		get
		{
			return _coinsCollectedLeaderboardId;
		}
	}

	private ITunesConnectNameManager()
	{
		_highScoreLeaderbaordId = GetIdentifierForAchievement(BaseLeaderboardIds, 0);
		_maxRoomsLeaderboardId = GetIdentifierForAchievement(BaseLeaderboardIds, 1);
		_coinsCollectedLeaderboardId = GetIdentifierForAchievement(BaseLeaderboardIds, 2);
	}

	public SwrveEconomy.GemPack GetGemPackFromId(string productIdentifier)
	{
		if (productIdentifier == _inAppPurcahseIdentifiers[0])
		{
			return SwrveEconomy.GemPack.Pack1;
		}
		if (productIdentifier == _inAppPurcahseIdentifiers[1])
		{
			return SwrveEconomy.GemPack.Pack2;
		}
		if (productIdentifier == _inAppPurcahseIdentifiers[2])
		{
			return SwrveEconomy.GemPack.Pack3;
		}
		if (productIdentifier == _inAppPurcahseIdentifiers[3])
		{
			return SwrveEconomy.GemPack.Pack4;
		}
		return SwrveEconomy.GemPack.None;
	}

	public string GetIdentifierForGemPack(SwrveEconomy.GemPack gemPack)
	{
		switch (gemPack)
		{
		case SwrveEconomy.GemPack.Pack1:
			return _inAppPurcahseIdentifiers[0];
		case SwrveEconomy.GemPack.Pack2:
			return _inAppPurcahseIdentifiers[1];
		case SwrveEconomy.GemPack.Pack3:
			return _inAppPurcahseIdentifiers[2];
		case SwrveEconomy.GemPack.Pack4:
			return _inAppPurcahseIdentifiers[3];
		default:
			return string.Empty;
		}
	}

	private static string[] GetIdentifiers(string[] sourceIdentifiers)
	{
		string[] array = new string[sourceIdentifiers.Length];
		for (int i = 0; i < sourceIdentifiers.Length; i++)
		{
			array[i] = GetIdentifierForAchievement(sourceIdentifiers, i);
		}
		return array;
	}

	private static string GetIdentifierForAchievement(string[] sourceIdentifiers, int index)
	{
		return sourceIdentifiers[index];
	}

	public static string GetIdentifierForAchievement(string identifier)
	{
		return identifier;
	}
}
