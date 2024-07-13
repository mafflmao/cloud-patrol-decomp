using System;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseCounter : MonoBehaviour
{
	public enum Mode
	{
		All = 0,
		MagicItemOnly = 1,
		SkylanderOnly = 2
	}

	public PowerupList powerupList;

	public CharacterDataList characterDataList;

	public Mode mode;

	public List<int> coinCostList;

	public List<int> gemCostList;

	public List<int> skylanderGemCostList;

	public List<int> magicItemGemCostList;

	public List<int> magicItemCoinCostList;

	private int _skylanderUnlockPrice = 40;

	private int _playerGems;

	private int _playerCoins;

	public int itemCount;

	public UIButton myButton;

	private void Start()
	{
		UpdateCounter();
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void HandleSecureStoragePlayerRadianceChanged(object sender, EventArgs e)
	{
		UpdateCounter();
	}

	private void HandleSecureStoragePlayerGemsChanged(object sender, EventArgs e)
	{
		UpdateCounter();
	}

	private void UpdateCounter()
	{
		itemCount = 0;
		_playerGems = 0;
		_playerCoins = 0;
		if (mode == Mode.All || mode == Mode.MagicItemOnly)
		{
			magicItemCoinCostList = GetUpgradableMagicItemCosts();
			magicItemCoinCostList.Sort();
			itemCount += GetNumberPurchasable(magicItemCoinCostList, _playerCoins);
			magicItemGemCostList = GetUnlockableMagicItemCosts();
			magicItemGemCostList.Sort();
			itemCount += GetNumberPurchasable(magicItemGemCostList, _playerGems);
		}
		if (mode == Mode.All || mode == Mode.SkylanderOnly)
		{
			skylanderGemCostList = GetUnlockableSkylanderCosts();
			skylanderGemCostList.Sort();
			itemCount += GetNumberPurchasable(skylanderGemCostList, _playerGems);
		}
		if (itemCount > 0)
		{
			myButton.Hide(false);
			myButton.Text = itemCount.ToString();
		}
		else
		{
			myButton.Hide(true);
		}
	}

	private int GetNumberPurchasable(List<int> listOfCosts, int startingCurrency)
	{
		List<int> list = new List<int>(listOfCosts);
		int num = 0;
		int num2 = startingCurrency;
		for (int i = 0; i < 500; i++)
		{
			if (list.Count <= 0)
			{
				break;
			}
			if (list[0] <= num2)
			{
				num2 -= list[0];
				num++;
				list.RemoveAt(0);
			}
		}
		return num;
	}

	public List<int> GetUnlockableSkylanderCosts()
	{
		_skylanderUnlockPrice = Bedrock.GetRemoteVariableAsInt("SkylanderUnlockPrice", _skylanderUnlockPrice);
		List<int> list = new List<int>();
		CharacterData[] allReleasedSkylanders = characterDataList.GetAllReleasedSkylanders();
		foreach (CharacterData cd in allReleasedSkylanders)
		{
			CharacterUserData characterUserData = ElementDataManager.Instance.GetCharacterUserData(cd);
			if (!characterUserData.IsUnlocked)
			{
				list.Add(_skylanderUnlockPrice);
			}
		}
		return list;
	}

	public List<int> GetUnlockableMagicItemCosts()
	{
		List<int> list = new List<int>();
		foreach (PowerupData powerup in powerupList.powerups)
		{
			list.Add(powerup.cost);
		}
		return list;
	}

	public List<int> GetUpgradableMagicItemCosts()
	{
		List<int> list = new List<int>();
		foreach (PowerupData powerup in powerupList.powerups)
		{
			if (powerup.canUpgrade)
			{
				int level = powerup.GetLevel();
				if (level < 5)
				{
					int upgradeCost = powerup.GetUpgradeCost(level);
					list.Add(upgradeCost);
				}
			}
		}
		return list;
	}
}
