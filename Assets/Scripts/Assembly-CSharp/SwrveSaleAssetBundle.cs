using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwrveSaleAssetBundle
{
	private const string RemoteUserResourceId = "SaleAssetBundle";

	private const string UrlFieldName = "AssetBundleURL";

	private const string VersionFieldName = "AssetBundleVersion";

	private const string CompatibleGameVersionsFieldName = "CompatibleGameVersions";

	private const string AllowCachingFieldName = "AllowCaching";

	private static SwrveSaleAssetBundle _instance;

	private Dictionary<string, string> _remoteUserResource;

	public static SwrveSaleAssetBundle Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new SwrveSaleAssetBundle();
			}
			return _instance;
		}
	}

	public bool HasAnyData
	{
		get
		{
			return _remoteUserResource.Any();
		}
	}

	public string Url
	{
		get
		{
			return Bedrock.GetFromResourceDictionaryAsString(_remoteUserResource, "AssetBundleURL", null);
		}
	}

	public int Version
	{
		get
		{
			return Bedrock.GetFromResourceDictionaryAsInt(_remoteUserResource, "AssetBundleVersion", 0);
		}
	}

	public string CompatibleVersionsString
	{
		get
		{
			return Bedrock.GetFromResourceDictionaryAsString(_remoteUserResource, "CompatibleGameVersions", null);
		}
	}

	public bool AllowCaching
	{
		get
		{
			return Bedrock.GetFromResourceDictionaryAsBool(_remoteUserResource, "AllowCaching", true);
		}
	}

	private SwrveSaleAssetBundle()
	{
	}

	public void UpdateFromSwrve()
	{
		if (!Bedrock.GetRemoteUserResources("SaleAssetBundle", out _remoteUserResource))
		{
			Debug.LogWarning("Unable to find swrve resource 'SaleAssetBundle' to load sale AssetBundle data.");
			_remoteUserResource = new Dictionary<string, string>();
		}
	}

	public bool IsCompatibleWithApplicationVersion(float version)
	{
		string compatibleVersionsString = CompatibleVersionsString;
		if (string.IsNullOrEmpty(compatibleVersionsString))
		{
			return false;
		}
		string[] source = compatibleVersionsString.Split(',');
		return source.Contains(version.ToString(), StringComparer.OrdinalIgnoreCase);
	}
}
