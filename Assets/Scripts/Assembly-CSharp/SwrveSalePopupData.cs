using System;
using System.Collections.Generic;

public class SwrveSalePopupData
{
	public enum GoButtonDestinations
	{
		None = 0,
		SkylanderSelect = 1,
		MagicItemStore = 2,
		CoinStore = 3,
		GemStore = 4,
		CollectionScreen = 5,
		ElementSelect = 6,
		SkylanderSelect7 = 7
	}

	private const string RemoteUserResourceId = "SalePopup";

	private const string IdFieldName = "Id";

	private const string GoButtonDestinationFieldName = "GoButtonDestination";

	private const string GoButtonDataFieldName = "GoButtonData";

	private static SwrveSalePopupData _instance;

	private Dictionary<string, string> _remoteUserResource;

	public static SwrveSalePopupData Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new SwrveSalePopupData();
			}
			return _instance;
		}
	}

	public bool IsSaleActive
	{
		get
		{
			if (_remoteUserResource == null)
			{
				return false;
			}
			string id = Id;
			return !string.IsNullOrEmpty(Id) && id != "-1" && id != "0" && id != "disabled";
		}
	}

	public string Id
	{
		get
		{
			return Bedrock.GetFromResourceDictionaryAsString(_remoteUserResource, "Id", null);
		}
	}

	public GoButtonDestinations GoButtonDestination
	{
		get
		{
			string fromResourceDictionaryAsString = Bedrock.GetFromResourceDictionaryAsString(_remoteUserResource, "GoButtonDestination", string.Empty);
			GoButtonDestinations value;
			if (EnumUtils.TryParse<GoButtonDestinations>(fromResourceDictionaryAsString, out value))
			{
				return value;
			}
			return GoButtonDestinations.None;
		}
	}

	public string GoButtonData
	{
		get
		{
			return Bedrock.GetFromResourceDictionaryAsString(_remoteUserResource, "GoButtonData", string.Empty);
		}
	}

	private SwrveSalePopupData()
	{
		UpdateFromSwrve();
	}

	public void UpdateFromSwrve()
	{
		if (!Bedrock.GetRemoteUserResources("SalePopup", out _remoteUserResource))
		{
			_remoteUserResource = new Dictionary<string, string>();
		}
		if (DebugSettingsUI.forceShowSaleDialog)
		{
			_remoteUserResource["Id"] = DateTime.Now.ToString();
		}
	}
}
