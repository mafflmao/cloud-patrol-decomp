using System;
using UnityEngine;

public class Cannon : MonoBehaviour
{
	public Transform target;

	private void Start()
	{
		GameManager.GameStarted += HandleGameManagerInstanceGameStarted;
		GameManager.GameOver += HandleGameManagerInstanceGameOver;
    }

	private void OnDestroy()
	{
		GameManager.GameStarted -= HandleGameManagerInstanceGameStarted;
		GameManager.GameOver -= HandleGameManagerInstanceGameOver;
	}

	private void HandleGameManagerInstanceGameOver(object sender, EventArgs e)
	{
    }

	private void HandleGameManagerInstanceGameStarted(object sender, EventArgs e)
	{
    }

	private void Update()
	{
		base.transform.LookAt(target, Vector3.up);
	}
}
