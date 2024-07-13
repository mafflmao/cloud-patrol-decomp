using System.Collections;
using UnityEngine;

public class CoinStoreButton : MonoBehaviour
{
	private static ILogger _log = LogBuilder.Instance.GetLogger(typeof(CoinStoreButton), LogLevel.Debug);

	public SpriteText txtQuantity;

	public SpriteText txtPrice;

	public float quantity;

	public float price;

	public PackedSprite packedSprite;

	public SoundEventData coinPurchaseSelectSFX;

	public ConfirmationDialog confirmDialogPrefab;

	public SoundEventData confirmDialogOkSound;

	public SoundEventData confirmDialogCancelSound;

	public SoundEventData confirmDialogDismissSound;

	public PrefabPlaceholder saleTagPlaceholder;

	private CoinStoreItemData _data;

	private SaleTag _saleTagInstance;

	public CoinStoreItemData Data
	{
		get
		{
			return _data;
		}
		set
		{
			_log.LogDebug("Data={0}", value);
			_data = value;
			UpdateGraphic();
		}
	}

	private void UpdateGraphic()
	{
		_log.LogDebug("UpdateGraphic()");
		txtQuantity.Text = _data.coins.ToString("n0");
		txtPrice.Text = _data.gemCost.ToString();
		packedSprite.PlayAnim(_data.graphic.ToString());
		if (_data.IsOnSale)
		{
			_saleTagInstance = saleTagPlaceholder.InstantiatePrefab().GetComponent<SaleTag>();
			_saleTagInstance.SaleText = _data.SaleText;
		}
		else if (_saleTagInstance != null)
		{
			Object.Destroy(_saleTagInstance);
		}
	}

	private void CoinButtonClicked()
	{
		StartCoroutine(CoinButtonClickedRte());
		SoundEventManager.Instance.Play2D(coinPurchaseSelectSFX);
	}

	private void PurchaseConfirmed()
	{
	}

	private void PurchaseCancelled()
	{
		_log.Log("Purchase confirmation cancelled.");
		SoundEventManager.Instance.Play2D(confirmDialogCancelSound);
		SwrveEventsPurchase.CoinPackFailed((int)_data.graphic, (ulong)_data.gemCost);
	}

	private IEnumerator CoinButtonClickedRte()
	{
		UIManager.instance.blockInput = true;
		CommonAnimations.AnimateButton(base.gameObject);
		yield return new WaitForSeconds(0.5f);
		UIManager.instance.blockInput = false;
	}
}
