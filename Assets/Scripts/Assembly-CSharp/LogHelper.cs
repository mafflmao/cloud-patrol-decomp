using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LogHelper
{
	public static void LogHistogram(IEnumerable<int> values)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		foreach (int value in values)
		{
			if (dictionary.ContainsKey(value))
			{
				Dictionary<int, int> dictionary2;
				Dictionary<int, int> dictionary3 = (dictionary2 = dictionary);
				int key;
				int key2 = (key = value);
				key = dictionary2[key];
				dictionary3[key2] = key + 1;
			}
			else
			{
				dictionary[value] = 1;
			}
		}
		Debug.Log(string.Join("\n", dictionary.Select((KeyValuePair<int, int> entry) => entry.Key + "\t" + entry.Value).ToArray()));
	}
}
