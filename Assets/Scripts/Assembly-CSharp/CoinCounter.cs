using System;
using UnityEngine;

public class CoinCounter : MonoBehaviour
{
	public SpriteText spriteText;

	public PackedSprite coinIcon;

	public void Start()
	{
		spriteText.Text = GameManager.moneyCollectedInVoyage.ToString();
	}

	private void OnEnable()
	{
		GameManager.MoneyCollected += HandleGameManagerMoneyCollected;
		GameManager.GameOver += HandleGameOver;
	}

	private void OnDisable()
	{
		GameManager.MoneyCollected -= HandleGameManagerMoneyCollected;
		GameManager.GameOver -= HandleGameOver;
	}

	private void HandleGameOver(object sender, EventArgs e)
	{
		coinIcon.Hide(true);
		spriteText.Hide(true);
	}

	private void HandleGameManagerMoneyCollected(object sender, EventArgs e)
	{
		spriteText.Text = GameManager.moneyCollectedInVoyage.ToString();
	}
}
