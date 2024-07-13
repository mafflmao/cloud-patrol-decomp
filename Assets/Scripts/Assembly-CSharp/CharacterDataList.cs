using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterDataList : ScriptableObject
{
	public CharacterData[] air;

	public CharacterData[] life;

	public CharacterData[] undead;

	public CharacterData[] earth;

	public CharacterData[] fire;

	public CharacterData[] water;

	public CharacterData[] magic;

	public CharacterData[] tech;

	public CharacterData[] swapForce;

	private Dictionary<string, CharacterData> nameCharacterDict;

	private Dictionary<string, CharacterData> ASINCharacterDict;

	public IEnumerable<CharacterData> GetCharacterListByElement(Elements.Type elem)
	{
		switch (elem)
		{
		case Elements.Type.Air:
			return air;
		case Elements.Type.Life:
			return life;
		case Elements.Type.Undead:
			return undead;
		case Elements.Type.Earth:
			return earth;
		case Elements.Type.Fire:
			return fire;
		case Elements.Type.Water:
			return water;
		case Elements.Type.Magic:
			return magic;
		case Elements.Type.Tech:
			return tech;
		default:
			return null;
		}
	}

	public CharacterData[] GetAllReleasedSkylanders()
	{
		return (from skylander in air.Concat(life).Concat(undead).Concat(earth)
				.Concat(fire)
				.Concat(water)
				.Concat(magic)
				.Concat(tech)
			where skylander.IsReleased
			select skylander).ToArray();
	}

	public IEnumerable<string> GetMagicMomentSceneNames()
	{
		CharacterData[][] characterDataLists = new CharacterData[8][] { air, life, undead, earth, fire, water, magic, tech };
		if (DebugSettingsUI.BuildWithTriggerHappyOnly)
		{
			yield return tech.Where((CharacterData character) => character.charName == "Trigger Happy").First().magicMomentScene;
			yield break;
		}
		CharacterData[][] array = characterDataLists;
		foreach (CharacterData[] characterDataList in array)
		{
			CharacterData[] array2 = characterDataList;
			foreach (CharacterData characterData in array2)
			{
				if (characterData.magicMomentScene != null)
				{
					yield return characterData.magicMomentScene;
				}
			}
		}
	}

	public CharacterData GetCharacterDataByName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		if (nameCharacterDict == null)
		{
			BuildCharacterDataDictionary();
		}
		if (nameCharacterDict.ContainsKey(name))
		{
			return nameCharacterDict[name];
		}
		return null;
	}

	public void BuildCharacterDataDictionary()
	{
		nameCharacterDict = new Dictionary<string, CharacterData>(StringComparer.OrdinalIgnoreCase);
		AddCharacterNamesToDictionary(air);
		AddCharacterNamesToDictionary(life);
		AddCharacterNamesToDictionary(undead);
		AddCharacterNamesToDictionary(earth);
		AddCharacterNamesToDictionary(fire);
		AddCharacterNamesToDictionary(water);
		AddCharacterNamesToDictionary(magic);
		AddCharacterNamesToDictionary(tech);
		AddCharacterNamesToDictionary(swapForce);
	}

	public void AddCharacterNamesToDictionary(IEnumerable<CharacterData> elemList)
	{
		foreach (CharacterData elem in elemList)
		{
			nameCharacterDict.Add(elem.name, elem);
		}
	}
}
