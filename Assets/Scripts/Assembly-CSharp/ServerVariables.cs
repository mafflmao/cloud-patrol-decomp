using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerVariables : MonoBehaviour
{
	private const string BedrockBonusFactorPerSkylanderKey = "BonusFactorPerSkylander";

	private const string CollectionVaultOpenUrlKey = "CollectionVaultOpenUrl";

	private const string CollectionVaultStoreUrlKey = "CollectionVaultStoreUrl";

	private const string HideCollectionVaultFeaturesKey = "HideCollectionVaultFeatures";

	private const string TrackToyUsageKey = "TrackToyUsage";

	private const string SwrveOptionsObjectName = "game_options";

	private const string HidePortalButtonOptionName = "hide_portal_button";

	public static bool HidePortalButton
	{
		get
		{
			return true;
		}
	}

	public static string CollectionVaultOpenUrl
	{
		get
		{
			string text = "bedrockexample://";
			Dictionary<string, string> resourceDictionary;
			if (Bedrock.GetRemoteUserResources("game_options", out resourceDictionary))
			{
				text = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "CollectionVaultOpenUrl", text);
			}
			return text;
		}
	}

	public static string CollectionVaultStoreUrl
	{
		get
		{
			string text = "https://www.google.com";
			Dictionary<string, string> resourceDictionary;
			if (Bedrock.GetRemoteUserResources("game_options", out resourceDictionary))
			{
				text = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "CollectionVaultStoreUrl", text);
			}
			return text;
		}
	}

	public static bool HideCollectionVaultFeatures
	{
		get
		{
			bool flag = true;
			Dictionary<string, string> resourceDictionary;
			if (Bedrock.GetRemoteUserResources("game_options", out resourceDictionary))
			{
				flag = Bedrock.GetFromResourceDictionaryAsBool(resourceDictionary, "HideCollectionVaultFeatures", flag);
			}
			return flag;
		}
	}

	public static bool TrackToyUsage
	{
		get
		{
			bool flag = false;
			Dictionary<string, string> resourceDictionary;
			if (Bedrock.GetRemoteUserResources("game_options", out resourceDictionary))
			{
				flag = Bedrock.GetFromResourceDictionaryAsBool(resourceDictionary, "TrackToyUsage", flag);
			}
			return flag;
		}
	}

	private void Start()
	{
		Debug.Log("Bedrock - asking for new server variables.");
		UpdateLocalSettingsFromServerVariables();
		Bedrock.TitleParametersChanged += HandleTitleParametersAvailable;
		Bedrock.UserResourcesChanged += HandleServerParametersAvailable;
	}

	private void OnDestroy()
	{
		Bedrock.TitleParametersChanged -= HandleTitleParametersAvailable;
		Bedrock.UserResourcesChanged -= HandleServerParametersAvailable;
	}

	private void HandleTitleParametersAvailable(object sender, EventArgs args)
	{
		Debug.Log("Bedrock - finished getting new title parameters.");
	}

	private void HandleServerParametersAvailable(object sender, EventArgs args)
	{
		Debug.Log("Bedrock - finished getting new server variables.");
		UpdateLocalSettingsFromServerVariables();
		LocalizationManager.Instance.ReloadStringTable();
	}

	private void UpdateLocalSettingsFromServerVariables()
	{
		SetBonusElementAmount();
	}

	private void SetBonusElementAmount()
	{
		GameManager.BonusFactorPerSkylander = Bedrock.GetRemoteVariableAsFloat("BonusFactorPerSkylander", GameManager.BonusFactorPerSkylander);
	}
}
