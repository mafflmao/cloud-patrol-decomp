public class CharacterUserData
{
	public enum ToyLink
	{
		None = 0,
		Skylanders2011 = 1,
		Skylanders2012 = 2,
		Skylanders2012LightCore = 4
	}

	public const int ToyLinkedGemCostRepresentation = -1;

	public const int AmazonStorePurchasedGemCostRepresentation = -2;

	public const int ToyClaimGemCostRepresentation = -3;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(CharacterUserData), LogLevel.Debug);

	private UnlockedCharacterCache _unlockedCharacterCache;

	private CharacterData _characterData;

	private UnlockedCharacterCache CharacterCache
	{
		get
		{
			if (_unlockedCharacterCache == null)
			{
				_unlockedCharacterCache = UnlockedCharacterCache.Instance;
			}
			return _unlockedCharacterCache;
		}
	}

	public CharacterData characterData
	{
		get
		{
			return _characterData;
		}
	}

	public bool IsUnlocked
	{
		get
		{
			return AmountUserSpentToUnlock.HasValue && AmountUserSpentToUnlock.Value >= -1;
		}
	}

	public ToyLink ToyLinkFlags
	{
		get
		{
			return CharacterCache.GetToyLinkFlags(_characterData);
		}
	}

	public bool IsToyClaimable
	{
		get
		{
			Bedrock.brContentUnlockInfo[] array = Bedrock.ListUnlockedContent(Bedrock.brLobbyServerTier.BR_LOBBY_SERVER_FRANCHISE, 256u);
			Bedrock.brContentUnlockInfo[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Bedrock.brContentUnlockInfo brContentUnlockInfo = array2[i];
				if (_characterData.MatchesToyAndSubtype(brContentUnlockInfo.contentKey, brContentUnlockInfo.subType) && !IsToyLinked)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool IsToyLinked
	{
		get
		{
			return ToyLinkFlags != 0 || (AmountUserSpentToUnlock.HasValue && AmountUserSpentToUnlock.Value == -1);
		}
	}

	public bool HasToyLinkSkylanders2011
	{
		get
		{
			return (ToyLinkFlags & ToyLink.Skylanders2011) != 0;
		}
	}

	public bool HasToyLinkSkylanders2012
	{
		get
		{
			return (ToyLinkFlags & ToyLink.Skylanders2012) != 0;
		}
	}

	public bool HasToyLinkSkylanders2012LightCore
	{
		get
		{
			return (ToyLinkFlags & ToyLink.Skylanders2012LightCore) != 0;
		}
	}

	public int? AmountUserSpentToUnlock
	{
		get
		{
			return CharacterCache.GetAmountUserSpentToUnlock(_characterData);
		}
	}

	public CharacterUserData(CharacterData cd)
	{
		_characterData = cd;
	}

	public void UnlockCharacter(int gemsSpentOnUnlock, ToyLink unlockedToyLinkFlags)
	{
		_log.LogDebug("UnlockCharacter({0}, {1})", gemsSpentOnUnlock, unlockedToyLinkFlags);
		ToyLink toyLinkFlags = ToyLinkFlags | unlockedToyLinkFlags;
		CharacterCache.Unlock(_characterData, gemsSpentOnUnlock, toyLinkFlags);
		ElementDataManager.Instance.GetElementUserData(_characterData.elementData.elementType).Update();
	}

	public void ResetUnlockState()
	{
		CharacterCache.ResetToyLinkFlags(_characterData);
		UnlockCharacter(0, ToyLink.None);
	}

	public static ToyLink RawSubTypeToToyLinkFlag(uint rawSubType)
	{
		CharacterSubType characterSubType = new CharacterSubType(rawSubType);
		if (characterSubType.Product == CharacterSubType.CharacterProduct.Skylanders2011)
		{
			return ToyLink.Skylanders2011;
		}
		if (characterSubType.Product == CharacterSubType.CharacterProduct.Skylanders2012 && characterSubType.Feature == CharacterSubType.CharacterFeature.LightCore)
		{
			return ToyLink.Skylanders2012LightCore;
		}
		if (characterSubType.Product == CharacterSubType.CharacterProduct.Skylanders2012)
		{
			return ToyLink.Skylanders2012;
		}
		return ToyLink.None;
	}
}
