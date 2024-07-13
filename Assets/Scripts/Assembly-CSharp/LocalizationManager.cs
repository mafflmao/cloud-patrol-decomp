using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager
{
	private static readonly Dictionary<string, string> languageNameToFilePrefix = new Dictionary<string, string>();

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(LocalizationManager), LogLevel.Log);

	private static LocalizationManager _instance = null;

	private readonly IDictionary<string, string> _stringTable = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	private string _currentLanguage;

	public static LocalizationManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new LocalizationManager();
			}
			return _instance;
		}
	}

	public bool IsEnglish
	{
		get
		{
			return _currentLanguage == "en" || _currentLanguage == "dev";
		}
	}

	public string CurrentLanguageCode
	{
		get
		{
			return _currentLanguage;
		}
	}

	private LocalizationManager()
	{
		languageNameToFilePrefix.Add("English", "en");
		languageNameToFilePrefix.Add("French", "fr");
		languageNameToFilePrefix.Add("Italian", "it");
		languageNameToFilePrefix.Add("German", "de");
		languageNameToFilePrefix.Add("Spanish", "es");
		ReloadStringTable();
	}

	private string GetCurrentLanguage()
	{
		string text = Application.systemLanguage.ToString();
		string value;
		if (!languageNameToFilePrefix.TryGetValue(text, out value))
		{
			value = "en";
			_log.LogWarning("Unsupported language '{0}', defaulting to '{1}'", text, value);
		}
		return value;
	}

	public void ReloadStringTable()
	{
		_currentLanguage = GetCurrentLanguage();
		_stringTable.Clear();
		List<List<string>> list = CsvUtilities.LoadCsvDataFromResource(GetResourceName(_currentLanguage));
		foreach (List<string> item in list)
		{
			GenerateLocalizationDataFromLine(item);
		}
		string itemId = string.Format("{0}_strings", _currentLanguage);
		Dictionary<string, string> resourceDictionary;
		if (Bedrock.GetRemoteUserResources(itemId, out resourceDictionary))
		{
			foreach (KeyValuePair<string, string> item2 in resourceDictionary)
			{
				_stringTable[item2.Key] = item2.Value;
			}
			return;
		}
		_log.LogDebug("No swrve overrides for language '{0}' found. Using baked-in localization strings.", _currentLanguage);
	}

	public static string GetResourceName(string language)
	{
		return string.Format("Strings/{0}_strings", language);
	}

	private void GenerateLocalizationDataFromLine(List<string> csvLine)
	{
		string valueFromListOrNull = CsvUtilities.GetValueFromListOrNull(csvLine, 0);
		string valueFromListOrNull2 = CsvUtilities.GetValueFromListOrNull(csvLine, 1);
		if (valueFromListOrNull2 == null)
		{
			_log.LogWarning("No Value for key '{0}' found. Skipping.", valueFromListOrNull);
			return;
		}
		if (_stringTable.ContainsKey(valueFromListOrNull))
		{
			_log.LogError("Duplicate key '{0}' in strings file for '{1}' detected.", valueFromListOrNull, _currentLanguage);
		}
		_stringTable.Add(valueFromListOrNull, valueFromListOrNull2);
	}

	public string GetString(string key)
	{
		string value;
		if (!_stringTable.TryGetValue(key, out value))
		{
			return key;
		}
		return value;
	}

	public string GetFormatString(string key, params object[] args)
	{
		return string.Format(GetString(key), args);
	}
}
