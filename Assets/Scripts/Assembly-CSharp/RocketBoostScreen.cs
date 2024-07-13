using System.Collections;
using UnityEngine;

public class RocketBoostScreen : MonoBehaviour
{
	public SpriteText costText;

	public SpriteText playerMoneyText;

	public UIButtonComposite button;

	public PowerupData rocketBoostData;

	public float displayDuration;

	public PrefabPlaceholder placeholderSaleTag;

	public SoundEventData sfxRocketUse;

	private bool _wasRocketUsed;

	private SaleTag _saleTag;

	private Vector3 _offscreenOffset = new Vector3(0f, -1200f, 0f);

	private void OnEnable()
	{
		Shooter.ComboCompleted += HandleShooterComboCompleted;
		if (placeholderSaleTag != null)
		{
			_saleTag = placeholderSaleTag.InstantiatePrefab().GetComponent<SaleTag>();
		}
		_saleTag.IsVisible = rocketBoostData.isUpgradeOnSale;
		_saleTag.SaleText = rocketBoostData.upgradeSaleText;
		costText.Text = SwrveEconomy.RocketBoosterCoinCost.ToString("n0");
	}

	private void HandleShooterComboCompleted(object sender, Shooter.ComboCompletedEventArgs e)
	{
		StopAllCoroutines();
		StartCoroutine(EndingCoroutine(0f));
	}

	private void OnDisable()
	{
		Shooter.ComboCompleted -= HandleShooterComboCompleted;
		if (_saleTag != null)
		{
			Object.Destroy(_saleTag);
		}
	}

	private void Start()
	{
		UpdatePlayerMoney();
		iTween.MoveFrom(base.gameObject, base.gameObject.transform.position + _offscreenOffset, 1f);
		StartCoroutine(EndingCoroutine(displayDuration));
	}

	private void UpdatePlayerMoney()
	{
		playerMoneyText.Text = "SecureStorage.Instance.CurrentCoins.ToString()";
	}

	public void OnClicked()
	{
		StopAllCoroutines();
		button.IsButtonColliderEnabled = false;
		if (sfxRocketUse != null)
		{
			SoundEventManager.Instance.Play2D(sfxRocketUse);
		}
		AchievementManager.Instance.TrackCoinsSpent(SwrveEconomy.RocketBoosterCoinCost);
		SwrveEventsPurchase.RocketBoosterUsed();
		_wasRocketUsed = true;
		UpdatePlayerMoney();
		rocketBoostData.Trigger(null, false);
		StartCoroutine(EndingCoroutine(0.5f));
	}

	public void Hide(bool isHidden)
	{
		base.gameObject.SetActive(!isHidden);
	}

	private IEnumerator EndingCoroutine(float timeBeforeStart)
	{
		yield return new WaitForSeconds(timeBeforeStart);
		iTween.MoveTo(base.gameObject, base.gameObject.transform.position + _offscreenOffset, 0.5f);
		yield return new WaitForSeconds(1f);
		if (!_wasRocketUsed)
		{
			SwrveEventsPurchase.RocketBoosterUsedFailed();
		}
		Object.Destroy(base.gameObject);
	}
}
