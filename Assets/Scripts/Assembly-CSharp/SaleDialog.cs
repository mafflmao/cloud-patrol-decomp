using System.Collections;
using UnityEngine;

public class SaleDialog : WhatsNewDialog
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(SaleDialog), LogLevel.Log);

	public SpriteText goButtonText;

	public UIButtonComposite goButton;

	public UIButtonComposite closeButton;

	public CharacterDataList allCharacters;

	private void Start()
	{
		UpdateGoToSaleButton();
	}

	public void GoToSaleButtonPressed()
	{
		_log.LogDebug("GoToSaleButtonPressed()");
		PostDismissAction = TryGoToNextScreen;
		ButtonPressed();
	}

	private void TryGoToNextScreen()
	{
		SwrveSalePopupData.GoButtonDestinations goButtonDestination = SwrveSalePopupData.Instance.GoButtonDestination;
		string goButtonData = SwrveSalePopupData.Instance.GoButtonData;
		_log.Log("SwrveSalePopupData - Destination='{0}',Data='{1}'", goButtonDestination, goButtonData);
		string text = null;
		switch (SwrveSalePopupData.Instance.GoButtonDestination)
		{
		case SwrveSalePopupData.GoButtonDestinations.CoinStore:
			text = "GemConverter";
			break;
		case SwrveSalePopupData.GoButtonDestinations.GemStore:
			text = "GemStore";
			break;
		case SwrveSalePopupData.GoButtonDestinations.MagicItemStore:
			text = "MagicItemSelect";
			PowerUpStoreController.OneTimeItemToSelect = SwrveSalePopupData.Instance.GoButtonData;
			break;
		case SwrveSalePopupData.GoButtonDestinations.SkylanderSelect:
		{
			text = "SkylanderSelect";
			CharacterData characterDataByName = allCharacters.GetCharacterDataByName(SwrveSalePopupData.Instance.GoButtonData);
			SkylanderSelectController.OneTimeScrollToSkylander = characterDataByName;
			break;
		}
		case SwrveSalePopupData.GoButtonDestinations.ElementSelect:
			text = "ElementSelect";
			break;
		case SwrveSalePopupData.GoButtonDestinations.CollectionScreen:
			text = "StoreHub";
			break;
		case SwrveSalePopupData.GoButtonDestinations.SkylanderSelect7:
			text = "SkylanderSelect7";
			break;
		}
		if (string.IsNullOrEmpty(text))
		{
			_log.Log("Unable to translate '{0}' into state name.", goButtonDestination);
			return;
		}
		_log.Log("Translated '{0}' into state '{1}'. Going there now.", goButtonDestination, text);
		StateManager.Instance.LoadAndActivateState(text);
	}

	private void UpdateGoToSaleButton()
	{
		bool flag = SwrveSalePopupData.Instance.GoButtonDestination != SwrveSalePopupData.GoButtonDestinations.None;
		closeButton.Hide(!flag);
		closeButton.IsButtonColliderEnabled = flag;
		if (!flag)
		{
			goButton.transform.localPosition = new Vector3(0f, goButton.transform.localPosition.y, goButton.transform.localPosition.z);
			goButtonText.Text = LocalizationManager.Instance.GetString("DIALOG_OK");
		}
	}

	public static IEnumerator TryShowSaleDialogCoroutine(SaleDialog saleDialogPrefab)
	{
		_log.LogDebug("TryShowSaleDialog()");
		SwrveSalePopupData saleData = SwrveSalePopupData.Instance;
		saleData.UpdateFromSwrve();
		if (saleData.IsSaleActive)
		{
			yield return new WaitForEndOfFrame();
		}
		else
		{
			_log.Log("Not showing sale dialog - no sale is active.");
		}
	}
}
