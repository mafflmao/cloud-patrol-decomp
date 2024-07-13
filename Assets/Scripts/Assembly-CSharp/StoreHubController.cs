using System.Collections;
using UnityEngine;

public class StoreHubController : StateController
{
	public UIButtonComposite btnMagicItems;

	public UIButtonComposite btnSkylanders;

	public UIButtonComposite btnGems;

	public UIButtonComposite btnCoins;

	public string nextState;

	public CharacterDataList characterDataList;

	public PowerupList powerupList;

	public SaleTag magicItemSaleTag;

	public SaleTag skylanderSaleTag;

	public SaleTag gemSaleTag;

	public SaleTag coinSaleTag;

	public bool AnySkylandersOnSale
	{
		get
		{
			if (characterDataList != null)
			{
			}
			return false;
		}
	}

	public bool AnyMagicItemsOnSale
	{
		get
		{
			if (powerupList != null)
			{
				foreach (PowerupData powerup in powerupList.powerups)
				{
					if ((powerup.isUnlockOnSale && !powerup.IsLocked) || (powerup.isUpgradeOnSale && !powerup.IsAtMaxLevel))
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	protected override void ShowState()
	{
		base.ShowState();
		skylanderSaleTag.UpdateVisibility();
		magicItemSaleTag.UpdateVisibility();
		gemSaleTag.UpdateVisibility();
		coinSaleTag.UpdateVisibility();
	}

	protected override void HideState()
	{
		base.HideState();
	}

	private void MagicItemsButtonPressed()
	{
		StartCoroutine(WaitForTransitions(0.5f));
		nextState = "MagicItemSelect";
		SwrveEventsUI.MagicItemButtonTouched();
	}

	private void SkylandersButtonPressed()
	{
		StartCoroutine(WaitForTransitions(0.5f));
		SkylanderSelectController.LastStateName = "StoreHub";
		nextState = "SkylanderSelect";
		SwrveEventsUI.SkylanderButtonTouched();
	}

	private void CoinsButtonPressed()
	{
		StartCoroutine(WaitForTransitions(0.5f));
		nextState = "GemConverter";
		SwrveEventsUI.CoinButtonTouched();
	}

	private void GemsButtonPressed()
	{
		StartCoroutine(WaitForTransitions(0.5f));
		nextState = "GemStore";
		SwrveEventsUI.GemButtonTouched();
	}

	private IEnumerator WaitForTransitions(float time)
	{
		UIManager.instance.blockInput = true;
		yield return new WaitForSeconds(time);
		UIManager.instance.blockInput = false;
		StateManager.Instance.LoadAndActivateState(nextState);
	}
}
