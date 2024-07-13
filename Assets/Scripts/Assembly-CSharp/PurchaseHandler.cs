using System;
using System.Collections;
using UnityEngine;

public class PurchaseHandler : SingletonMonoBehaviour
{
	private enum PurchaseState
	{
		Starting = 0,
		Disconnected = 1,
		WaitingForProducts = 2,
		Purchasing = 3,
		Idle = 4
	}

	public enum PurchaseFailedReason
	{
		NormalStoreError = 0,
		UserCancelledPurchase = 1,
		BedrockError = 2
	}

	public delegate void PurchaseSuccessDelegate(string id);

	public delegate void PurchaseFailedDelegate(PurchaseFailedReason reason);

	private const int MAX_VALIDATION_TASK_START_FAILURES = 5;

	public const string CancelledErrorMessage = "cancelled";

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(PurchaseHandler), LogLevel.Debug);

	[NonSerialized]
	public bool gotProducts;

	private short _purchaseTaskHandle = -1;

	private short _receiptValidationTaskHandle = -1;

	private int _receiptValidationFailures;

	public static PurchaseHandler Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<PurchaseHandler>();
		}
	}

	public bool WaitingForProducts { get; private set; }

	public static event PurchaseSuccessDelegate PurchaseSuccess;

	public static event PurchaseFailedDelegate PurchaseFailed;

	public Bedrock.IAPCatalogEntry GetEntryForProduct(string productIdentifier)
	{
		return Bedrock.GetIAPCatalogEntry(productIdentifier);
	}

	protected override void AwakeOnce()
	{
		base.AwakeOnce();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void OnEnable()
	{
		Bedrock.IAPCatalogRetrieved += HandleIAPCatalogRetrieved;
		Bedrock.IAPRequestCompleted += HandleIAPRequestCompleted;
	}

	public void OnDisable()
	{
		Bedrock.IAPCatalogRetrieved -= HandleIAPCatalogRetrieved;
		Bedrock.IAPRequestCompleted -= HandleIAPRequestCompleted;
	}

	private void Start()
	{
		_log.LogDebug("Start()");
	}

	public void GetProducts()
	{
		gotProducts = false;
		WaitingForProducts = true;
		_purchaseTaskHandle = -1;
		string[] iapProductIdentifiers = ITunesConnectNameManager.Instance.IapProductIdentifiers;
		Bedrock.brIAPProductCategory[] iapProductCategories = ITunesConnectNameManager.Instance.IapProductCategories;
		string[] array = iapProductIdentifiers;
		foreach (string text in array)
		{
			Debug.Log("ProductID: " + text);
		}
		Bedrock.InitializeIAPCatalog(iapProductIdentifiers, iapProductCategories, (uint)iapProductIdentifiers.Length);
	}

	public IEnumerator Buy(string id)
	{
		if (_purchaseTaskHandle == -1)
		{
			_log.LogDebug("New Purchase Started for id '{0}'.", id);
			SwrveEconomy.GemPack gp = ITunesConnectNameManager.Instance.GetGemPackFromId(id);
			ulong gemCount = (ulong)SwrveEconomy.GetGemPackAmount(gp);
			_log.LogDebug("Updating bedrock gemCount to current value ({0}) for gempack {1}", gemCount, gp);
			Bedrock.SetInAppPurchasingCatalogEntryVirtualCurrencyInfo(id, "Gems", gemCount);
			_log.LogDebug("Starting task to request IAP");
			_purchaseTaskHandle = Bedrock.RequestInAppPurchase(id);
			if (_purchaseTaskHandle == -1)
			{
				_log.LogError("RequestInAppPurchase failure");
				HandleUserPurchaseFailed();
				yield break;
			}
			_log.LogDebug("Task started OK. Starting purchase of {0}", id);
			using (BedrockTask task = new BedrockTask(_purchaseTaskHandle))
			{
				_log.LogDebug("Waiting for task...");
				yield return StartCoroutine(task.WaitForTaskToCompleteCoroutine());
				_log.LogDebug("Task completed with status '{0}'", task.Status);
			}
			_purchaseTaskHandle = -1;
			if (!ReCheckForPendingCompletedPurchases())
			{
				OnPurchaseFailed(PurchaseFailedReason.BedrockError);
			}
		}
		else
		{
			_log.LogError("Attempting to start purchase while one is already pending.");
		}
	}

	private void HandleProductListFailed(string error)
	{
		_log.LogError("Failed to get product list: {0}", error);
		WaitingForProducts = false;
		gotProducts = false;
	}

	private void HandlePurchaseSuccessful(Bedrock.IAPCatalogEntry entry)
	{
		_log.Log("HandlePurchaseSuccessful(...)");
		string iAPProductID = entry.IAPProductID;
		_log.Log("Purchase success: " + iAPProductID);
		SwrveEconomy.GemPack gemPackFromId = ITunesConnectNameManager.Instance.GetGemPackFromId(iAPProductID);
		SwrveEventsPurchase.GemPackPurchased(gemPackFromId, (ulong)SwrveEconomy.GetGemPackPrice(gemPackFromId), (ulong)SwrveEconomy.GetGemPackAmount(gemPackFromId));
		if (PurchaseHandler.PurchaseSuccess != null)
		{
			PurchaseHandler.PurchaseSuccess(iAPProductID);
			return;
		}
		_log.LogWarning("No one was Listening for purchase success event! Showing notification.");
		PurchaseNotificationPanelSettings settings = new PurchaseNotificationPanelSettings(LocalizationManager.Instance.GetString("GEMPURCHASE_SUCCEEDED"));
		NotificationPanel.Instance.Display(settings);
	}

	private void HandleUserPurchaseFailed()
	{
		_log.Log("HandleUserPurchaseFailed");
		SwrveEventsPurchase.GemPackFailed();
		OnPurchaseFailed(PurchaseFailedReason.NormalStoreError);
	}

	private void HandlePurchaseCancel()
	{
		_log.Log("HandlePurchaseCancel");
		SwrveEventsPurchase.GemPackCancelled();
		OnPurchaseFailed(PurchaseFailedReason.UserCancelledPurchase);
	}

	private void OnPurchaseFailed(PurchaseFailedReason reason)
	{
		_log.LogDebug("OnPurchaseFailed({0})", reason);
		if (PurchaseHandler.PurchaseFailed != null)
		{
			PurchaseHandler.PurchaseFailed(reason);
			return;
		}
		_log.LogWarning("No one was Listening for purchase failure event ({0})!", reason);
		if (reason == PurchaseFailedReason.NormalStoreError)
		{
			_log.Log("Showing notification.");
			ActivateNotificationPanelSettings settings = new ActivateNotificationPanelSettings(LocalizationManager.Instance.GetString("GEMPURCHASE_FAILED_TEXT"), 4f);
			NotificationPanel.Instance.Display(settings);
		}
	}

	private void HandleIAPCatalogRetrieved(object sender, EventArgs e)
	{
		_log.Log("IAP Products retrieved.");
		WaitingForProducts = false;
		bool flag = true;
		string[] iapProductIdentifiers = ITunesConnectNameManager.Instance.IapProductIdentifiers;
		string[] array = iapProductIdentifiers;
		foreach (string text in array)
		{
			Bedrock.IAPCatalogEntry iAPCatalogEntry = Bedrock.GetIAPCatalogEntry(text);
			if (DebugSettingsUI.forceInvalidProductInResult || iAPCatalogEntry == null)
			{
				_log.LogError("Failed to get catalog item for: {0}", text);
				flag = false;
				continue;
			}
			_log.LogDebug("Retrieved Product {0} with status {1}", iAPCatalogEntry.IAPProductID, iAPCatalogEntry.IAPProductStatus);
			switch (iAPCatalogEntry.IAPProductStatus)
			{
			case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_UNKNOWN:
			case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_NOT_VALID:
			case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PENDING_CATALOG_UPDATE:
				_log.LogWarning("Product '{0}' was in non-final status '{1}'. Assuming product retrieval failed.", text, iAPCatalogEntry.IAPProductStatus);
				flag = false;
				break;
			}
		}
		if (DebugSettingsUI.forceFailProductRetrieval)
		{
			_log.LogWarning("Debug Option forcing product retreival to fail.");
			flag = false;
		}
		gotProducts = flag;
	}

	public bool ReCheckForPendingCompletedPurchases()
	{
		_log.Log("ReCheckForPendingCompletedPurchases");
		if (Application.isEditor)
		{
			_log.LogDebug("In editor. Aborting.");
			return true;
		}
		uint numberOfItems;
		if (!Bedrock.GetInAppPurchasingStoredCompletedPurchaseCount(out numberOfItems) || DebugSettingsUI.forceFailReadingPurchaseQueue)
		{
			_log.LogError("Unable to get number of items in queue. Aborting.");
			return false;
		}
		if (numberOfItems == 0)
		{
			_log.Log("No items are in completed item queue. Aborting.");
			return true;
		}
		_log.LogDebug("Found {0} Pending purchase item(s). Retrieving.", numberOfItems);
		Bedrock.IAPCatalogEntry catalogEntry;
		if (!Bedrock.GetInAppPurchasingFirstCompletedStoredPurchase(out catalogEntry))
		{
			_log.LogError("Failed to retrieve top item.");
			return false;
		}
		string iAPProductID = catalogEntry.IAPProductID;
		Bedrock.brIAPProductStatus iAPProductStatus = catalogEntry.IAPProductStatus;
		_log.LogDebug("Retrieved purchase data for product '{0}' with status '{1}'.", iAPProductID, iAPProductStatus);
		bool flag = true;
		switch (iAPProductStatus)
		{
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_SUCCEEDED_VALIDATED:
			HandlePurchaseSuccessful(catalogEntry);
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_CANCELED:
			_log.LogWarning("Purchase of '{0}' cancelled.", iAPProductID);
			HandlePurchaseCancel();
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_FAILED:
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_VALIDATION_FAILED:
			_log.LogWarning("Purchase of '{0}' failed with status '{1}'", iAPProductID, iAPProductStatus);
			HandleUserPurchaseFailed();
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_SUCCEEDED_VALIDATING:
			flag = false;
			if (_receiptValidationTaskHandle == -1)
			{
				_log.LogDebug("No validation task underway. Starting one.");
				if (_receiptValidationFailures > 5)
				{
					_log.LogWarning("We failed to start validation task {0} times. Giving up and forcing removal from Queue.", _receiptValidationFailures);
					_receiptValidationFailures = 0;
					HandlePurchaseSuccessful(catalogEntry);
					flag = true;
				}
				else
				{
					_log.LogDebug("Starting new validation task.");
					StartCoroutine(StartValidatingLastItemInQueue());
				}
			}
			else
			{
				_log.LogDebug("Validation task is already underway. Waiting for completion.");
			}
			break;
		default:
			flag = false;
			break;
		}
		if (flag)
		{
			_log.LogDebug("Status {0} for top item was a final status. Removing from Queue.", iAPProductStatus);
			if (!Bedrock.ClearInAppPurchasingFirstCompletedStoredPurchase())
			{
				_log.LogError("Failed to remove item from purchase queue, but gem-reward was alread given! Ruh Roh!");
				return false;
			}
			_log.LogDebug("Removed Item Successfully");
			ReCheckForPendingCompletedPurchases();
		}
		else
		{
			_log.LogDebug("Status {0} for top item not a final status. Not removing from Queue.", iAPProductStatus);
		}
		return true;
	}

	private IEnumerator StartValidatingLastItemInQueue()
	{
		_log.LogDebug("StartValidatingLastItemInQueue");
		if (_receiptValidationTaskHandle != -1)
		{
			_log.LogError("Called StartValidatingLastItemInQueue() while a validation task was already underway.");
			yield break;
		}
		if (DebugSettingsUI.forceFailValidationTask)
		{
			_log.LogWarning("Debug Forcing ValidationTask to not start. Pretending the task couldn't start.");
		}
		else
		{
			_receiptValidationTaskHandle = Bedrock.ValidateLastInAppPurchaseReceipt();
		}
		if (_receiptValidationTaskHandle == -1)
		{
			_receiptValidationFailures++;
			_log.LogError("Failed to start validation task (failureCount: {0}). This is bad waiting 1s.", _receiptValidationFailures);
			yield return new WaitForSeconds(1f);
		}
		else
		{
			_receiptValidationFailures = 0;
			using (BedrockTask task = new BedrockTask(_receiptValidationTaskHandle))
			{
				_log.LogDebug("Waiting for validation task...");
				yield return StartCoroutine(task.WaitForTaskToCompleteCoroutine());
			}
			_log.LogDebug("Validation task complete. Re-checking for completed purchase.");
			_receiptValidationTaskHandle = -1;
		}
		ReCheckForPendingCompletedPurchases();
	}

	private void HandleIAPRequestCompleted(object sender, EventArgs e)
	{
		_log.LogDebug("HandleIAPRequestCompleted Event Raised From Bedrock");
		ReCheckForPendingCompletedPurchases();
	}
}
