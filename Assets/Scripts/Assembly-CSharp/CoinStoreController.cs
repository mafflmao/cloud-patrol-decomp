using System;
using System.Collections;
using UnityEngine;

public class CoinStoreController : StateController
{
	public const float WaitForProductsTimeout = 20f;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(CoinStoreController), LogLevel.Debug);

	public GameObject btnGem1;

	public GameObject btnGem2;

	public GameObject btnGem3;

	public GameObject btnGem4;

	private GameObject[] _btnArray;

	private IDisposable _backButtonDisabledContext;

	private bool _showingActivityBezel;

	public SoundEventData sfxGemButtonClick;

	private void Start()
	{
		_btnArray = new GameObject[4] { btnGem1, btnGem2, btnGem3, btnGem4 };
		UpdateButtonText(btnGem1, SwrveEconomy.GemPack.Pack1);
		UpdateButtonText(btnGem2, SwrveEconomy.GemPack.Pack2);
		UpdateButtonText(btnGem3, SwrveEconomy.GemPack.Pack3);
		UpdateButtonText(btnGem4, SwrveEconomy.GemPack.Pack4);
	}

	private void OnEnable()
	{
		UIManager.BlockInputChanging += CancelBlockInputChanging;
	}

	private void OnDisable()
	{
	}

	private void StartWaitingForProducts()
	{
		if (PurchaseHandler.Instance != null && PurchaseHandler.Instance.gotProducts)
		{
			HandlePurchaseHandlerProductListRecieved();
			return;
		}
		Bedrock.brIAPAvailabilityStatus iAPAvailabilityStatus = Bedrock.GetIAPAvailabilityStatus();
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			InvokeHelper.InvokeSafe(ShowWifiErrorMessage, 0.25f, this);
			return;
		}
		if (iAPAvailabilityStatus == Bedrock.brIAPAvailabilityStatus.BR_IAP_AVAILABILITY_PURCHASES_DISABLED)
		{
			InvokeHelper.InvokeSafe(ShowRestrictionMessage, 0.25f, this);
			return;
		}
		_log.Log("Getting products...");
		StartActivityViewWithLabel(LocalizationManager.Instance.GetString("GEMPURCHASE_LOADINGPRODUCTS"));
		if (!PurchaseHandler.Instance.WaitingForProducts)
		{
			PurchaseHandler.Instance.GetProducts();
		}
		StartCoroutine(WaitForProducts());
	}

	private IEnumerator WaitForProducts()
	{
		float timeoutTime = Time.time + 20f;
		while (PurchaseHandler.Instance.WaitingForProducts && Time.time < timeoutTime)
		{
			yield return new WaitForEndOfFrame();
		}
		FinishActivtiyView();
		if (PurchaseHandler.Instance.gotProducts)
		{
			HandlePurchaseHandlerProductListRecieved();
			yield break;
		}
		_log.LogError("PurchaseHandler said we didn't get any products");
		OnRetrieveProductsFailed(LocalizationManager.Instance.GetString("GENERIC_UNABLETOCONNECT"), LocalizationManager.Instance.GetString("GEMPURCHASE_CANNOTCONNECTTOSTORE"));
	}

	private void OnRetrieveProductsFailed(string title, string text)
	{
	}

	private void HandleUnableToConnectButtonPressed(string buttonText)
	{
	}

	private void ShowRestrictionMessage()
	{
	}

	private void ShowWifiErrorMessage()
	{
	}

	private void UpdateButtonText(GameObject button, SwrveEconomy.GemPack gp)
	{
		GemStoreButton component = button.GetComponent<GemStoreButton>();
		component.quantity = SwrveEconomy.GetGemPackAmount(gp);
		component.price = "-";
		component.SaleTagText = SwrveEconomy.GetGemPackOnSaleText(gp);
		component.UpdateText();
	}

	private void HandlePurchaseHandlerProductListRecieved()
	{
		SwrveEventsPurchase.ProductListRecieved();
		_log.Log("Product List Recieved!");
		for (int i = 0; i < ITunesConnectNameManager.Instance.IapProductIdentifiers.Length; i++)
		{
			string text = ITunesConnectNameManager.Instance.IapProductIdentifiers[i];
			Bedrock.IAPCatalogEntry entryForProduct = PurchaseHandler.Instance.GetEntryForProduct(text);
			if (entryForProduct == null)
			{
				_log.LogError("Failed to get catalog entry from bedrock for '" + text + "'");
				break;
			}
			if (Enum.IsDefined(typeof(SwrveEconomy.GemPack), i))
			{
				if (i < _btnArray.Length)
				{
					UpdateButtonWithProductInfo(_btnArray[i], (SwrveEconomy.GemPack)i, entryForProduct);
				}
				else
				{
					_log.LogError("Trying to modify button out-of-range.");
				}
			}
			else
			{
				_log.LogError("GemPack Enum can't convert " + i + ". Product not updating.");
			}
		}
		PurchaseHandler.Instance.ReCheckForPendingCompletedPurchases();
	}

	private void UpdateButtonWithProductInfo(GameObject button, SwrveEconomy.GemPack gp, Bedrock.IAPCatalogEntry product)
	{
		product.DebugPrint();
		GemStoreButton component = button.GetComponent<GemStoreButton>();
		if (SwrveEconomy.GetGemPackAmount(gp) != 0)
		{
			component.quantity = SwrveEconomy.GetGemPackAmount(gp);
			component.SaleTagText = SwrveEconomy.GetGemPackOnSaleText(gp);
		}
		component.price = product.IAPLocalizedProductPrice;
		component.UpdateText();
	}

	protected override void ShowState()
	{
		PurchaseHandler.PurchaseSuccess += HandlePurchaseSuccessful;
		PurchaseHandler.PurchaseFailed += HandlePurchaseFailed;
		_log.Log("Showing CoinStoreController.");
		StartWaitingForProducts();
	}

	protected override void HideState()
	{
		PurchaseHandler.PurchaseSuccess -= HandlePurchaseSuccessful;
		PurchaseHandler.PurchaseFailed -= HandlePurchaseFailed;
	}

	private void OnGems1BtnClick()
	{
		if (PurchaseHandler.Instance.gotProducts)
		{
			StartCoroutine(OnGemsBtnClickRte(btnGem1, SwrveEconomy.GemPack.Pack1));
		}
	}

	private void OnGems2BtnClick()
	{
		if (PurchaseHandler.Instance.gotProducts)
		{
			StartCoroutine(OnGemsBtnClickRte(btnGem2, SwrveEconomy.GemPack.Pack2));
		}
	}

	private void OnGems3BtnClick()
	{
		if (PurchaseHandler.Instance.gotProducts)
		{
			StartCoroutine(OnGemsBtnClickRte(btnGem3, SwrveEconomy.GemPack.Pack3));
		}
	}

	private void OnGems4BtnClick()
	{
		if (PurchaseHandler.Instance.gotProducts)
		{
			StartCoroutine(OnGemsBtnClickRte(btnGem4, SwrveEconomy.GemPack.Pack4));
		}
	}

	private IEnumerator OnGemsBtnClickRte(GameObject go, SwrveEconomy.GemPack gemPack)
	{
		SoundEventManager.Instance.Play2D(sfxGemButtonClick);
		UIManager.instance.blockInput = true;
		CommonAnimations.AnimateButton(go);
		yield return new WaitForSeconds(0.75f);
		if (Application.isEditor)
		{
			UIManager.instance.blockInput = false;
			yield break;
		}
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			UIManager.instance.blockInput = false;
			yield break;
		}
		Bedrock.brIAPAvailabilityStatus iapStatus = Bedrock.GetIAPAvailabilityStatus();
		if (iapStatus == Bedrock.brIAPAvailabilityStatus.BR_IAP_AVAILABILITY_PURCHASES_ENABLED || iapStatus == Bedrock.brIAPAvailabilityStatus.BR_IAP_AVAILABILITY_PURCHASES_ENABLED_NO_VERIFICATION)
		{
			string id = ITunesConnectNameManager.Instance.GetIdentifierForGemPack(gemPack);
			StartCoroutine(PurchaseHandler.Instance.Buy(id));
			StartActivityViewWithLabel(LocalizationManager.Instance.GetString("GEMPURCHASE_PURCHASING"));
		}
		else
		{
			UIManager.instance.blockInput = false;
		}
	}

	private void OnDebugBtnClick()
	{
	}

	private void StartActivityViewWithLabel(string label)
	{
		UIManager.instance.blockInput = true;
		HeaderUI.Instance.backBtn.controlIsEnabled = false;
		_showingActivityBezel = true;
	}

	private void FinishActivtiyView()
	{
		_showingActivityBezel = false;
		HeaderUI.Instance.backBtn.controlIsEnabled = true;
		UIManager.instance.blockInput = false;
	}

	private void CancelBlockInputChanging(object sender, UIManager.BlockInputChangingEventArgs e)
	{
		if (_showingActivityBezel)
		{
			e.Cancel();
		}
	}

	private void HandlePurchaseSuccessful(string id)
	{
		_log.Log("Purchase Succeeded! " + id);
		FinishActivtiyView();
	}

	private void HandlePurchaseFailed(PurchaseHandler.PurchaseFailedReason reason)
	{
		_log.Log("Purchase Failed: " + reason);
		FinishActivtiyView();
		switch (reason)
		{
		}
	}
}
