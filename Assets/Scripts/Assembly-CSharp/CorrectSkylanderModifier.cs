using System.Text;
using UnityEngine;

public class CorrectSkylanderModifier : BountyModifier
{
	public const string SkylanderPlaceholderString = "{skylander}";

	private CharacterData _skylander;

	private void Awake()
	{
		ChooseRandomSkylander();
	}

	public override bool AllowIncrement()
	{
		return StartGameSettings.Instance.activeSkylander == _skylander;
	}

	public void SetSkylanderFromData(string dataString)
	{
		if (dataString != "Random" && dataString != "Any")
		{
			_skylander = BountyChooser.Instance.allCharacters.GetCharacterDataByName(dataString);
			if (_skylander == null)
			{
				Debug.LogError("Unable to find skylander named '" + dataString + "'. Going with random instead,,,");
			}
		}
		if (_skylander == null)
		{
			ChooseRandomSkylander();
		}
	}

	private void ChooseRandomSkylander()
	{
		CharacterData[] allReleasedSkylanders = BountyChooser.Instance.allCharacters.GetAllReleasedSkylanders();
		_skylander = allReleasedSkylanders[Random.Range(0, allReleasedSkylanders.Length)];
	}

	public override string GetSaveState()
	{
		return _skylander.name;
	}

	public override void LoadFromSaveState(string saveState)
	{
		_skylander = BountyChooser.Instance.allCharacters.GetCharacterDataByName(saveState);
	}

	public override void PerformDescriptionReplacement(StringBuilder stringBuilder)
	{
		stringBuilder.Replace("{skylander}", _skylander.charName);
	}
}
