using System;
using UnityEngine;

public class RocketBoostScreenManager : MonoBehaviour
{
	public RocketBoostScreen rocketBoostScreenPrefab;

	public PowerupData rocketBoostData;

	private RocketBoostScreen _rocketBoostScreenInstance;

	private void OnEnable()
	{
		GameManager.GameStarted += HandleGameManagerGameStarted;
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
		GameManager.GameStateChanged += HandleGameManagerGameStateChanged;
		GameManager.PauseChanged += HandleGameManagerPauseChanged;
	}

	private void OnDisable()
	{
		GameManager.GameStarted -= HandleGameManagerGameStarted;
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
		GameManager.GameStateChanged -= HandleGameManagerGameStateChanged;
		GameManager.PauseChanged -= HandleGameManagerPauseChanged;
	}

	private void HandleGameManagerGameStateChanged(object sender, GameManager.GameStateChangedEventArgs e)
	{
		if (_rocketBoostScreenInstance != null && (e.NewState == GameManager.GameState.Dead || e.NewState == GameManager.GameState.Dying))
		{
			UnityEngine.Object.Destroy(_rocketBoostScreenInstance.gameObject);
		}
	}

	private void HandleGameManagerPauseChanged(object sender, PauseChangeEventArgs e)
	{
		if (_rocketBoostScreenInstance != null)
		{
			_rocketBoostScreenInstance.Hide(GameManager.Instance.IsPaused);
		}
	}

	private void HandleGameManagerGameStarted(object sender, EventArgs e)
	{
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		if (LevelManager.Instance.RoomsCleared >= 1 && _rocketBoostScreenInstance != null)
		{
			UnityEngine.Object.Destroy(_rocketBoostScreenInstance.gameObject);
		}
	}

	private void ShowBootsPrompt()
	{
		_rocketBoostScreenInstance = (RocketBoostScreen)UnityEngine.Object.Instantiate(rocketBoostScreenPrefab);
	}
}
