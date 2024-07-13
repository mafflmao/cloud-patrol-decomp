using System;
using UnityEngine;

public class SuicideOnGameOver : MonoBehaviour
{
	private void Start()
	{
		GameManager.GameOver += HandleGameManagerGameOver;
	}

	private void OnDestroy()
	{
		GameManager.GameOver -= HandleGameManagerGameOver;
	}

	private void HandleGameManagerGameOver(object sender, EventArgs e)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
