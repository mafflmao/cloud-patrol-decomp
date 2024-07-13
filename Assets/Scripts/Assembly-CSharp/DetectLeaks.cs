using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DetectLeaks : MonoBehaviour
{
	private StringBuilder _stringBuilder = new StringBuilder();

	private void OnGUI()
	{
		Object[] array = Object.FindObjectsOfType(typeof(Object));
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			string text = @object.GetType().ToString();
			if (dictionary.ContainsKey(text))
			{
				Dictionary<string, int> dictionary2;
				Dictionary<string, int> dictionary3 = (dictionary2 = dictionary);
				string key;
				string key2 = (key = text);
				int num = dictionary2[key];
				dictionary3[key2] = num + 1;
			}
			else
			{
				dictionary[text] = 1;
			}
		}
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>(dictionary);
		list.Sort((KeyValuePair<string, int> firstPair, KeyValuePair<string, int> nextPair) => nextPair.Value.CompareTo(firstPair.Value));
		_stringBuilder.Length = 0;
		foreach (KeyValuePair<string, int> item in list)
		{
			_stringBuilder.AppendFormat("{0}: {1}", item.Key, item.Value).AppendLine();
		}
		GUILayout.Label(_stringBuilder.ToString());
	}
}
