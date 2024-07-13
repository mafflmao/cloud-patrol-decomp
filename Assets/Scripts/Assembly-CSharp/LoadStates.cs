using System.Collections.Generic;
using UnityEngine;

public class LoadStates : MonoBehaviour
{
	public string[] loadedStates;

	public StateRoot myState;

	public bool loadDefaults = true;

	private void Awake()
	{
		if (loadDefaults)
		{
			List<string> list = new List<string>();
			list.Add("Title");
			list.Add("Loadout");
			list.Add("StoreHub");
			list.Add("MagicItemSelect");
			list.Add("SkylanderDetails");
			list.Add("StartGame");
			list.Add("Help");
			list.Add("SkylanderSelect");
			list.Add("Credits");
			List<string> list2 = list;
			loadedStates = list2.ToArray();
		}
	}

	private void OnStateActivate(string oldState)
	{
		string[] array = StateManager.Instance.LoadedStates;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!text.Equals(myState.stateName) && !IsStateReferenced(text))
			{
				StateManager.Instance.UnloadState(text);
			}
		}
		string[] array3 = loadedStates;
		foreach (string text2 in array3)
		{
			if (!StateManager.Instance.Exists(text2))
			{
				StateManager.Instance.LoadFromSaveState(text2);
			}
		}
	}

	private bool IsStateReferenced(string name)
	{
		string[] array = loadedStates;
		foreach (string text in array)
		{
			if (text.Equals(name))
			{
				return true;
			}
		}
		return false;
	}
}
