using System.Linq;

public class ElementUserData
{
	public enum CollectionState
	{
		All = 0,
		Some = 1,
		None = 2
	}

	private ElementData _data;

	private CharacterUserData[] charUserDataList;

	public int numCharacters;

	public int numUnlockedCharacters;

	public int numClaimableCharacters;

	public CollectionState collectionState
	{
		get
		{
			if (numUnlockedCharacters == 0)
			{
				return CollectionState.None;
			}
			if (numUnlockedCharacters < numCharacters)
			{
				return CollectionState.Some;
			}
			return CollectionState.All;
		}
	}

	public ElementUserData(ElementData data)
	{
		_data = data;
		Update();
	}

	public void Update()
	{
		if (charUserDataList == null)
		{
			charUserDataList = (from characterData in ElementDataManager.Instance.characterDataList.GetCharacterListByElement(_data.elementType)
				select ElementDataManager.Instance.GetCharacterUserData(characterData)).ToArray();
		}
		numCharacters = charUserDataList.Length;
		numUnlockedCharacters = 0;
		CharacterUserData[] array = charUserDataList;
		foreach (CharacterUserData characterUserData in array)
		{
			if (characterUserData.IsUnlocked || characterUserData.IsToyLinked)
			{
				numUnlockedCharacters++;
			}
		}
		numClaimableCharacters = 0;
		CharacterUserData[] array2 = charUserDataList;
		foreach (CharacterUserData characterUserData2 in array2)
		{
			if (characterUserData2.IsToyClaimable)
			{
				numClaimableCharacters++;
			}
		}
	}
}
