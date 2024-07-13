using System;
using UnityEngine;

public class BombShipTrollManager : SingletonMonoBehaviour
{
	public GameObject trollShipPrefab;

	public bool forceSpawn;

	private GameObject _trollShip;

	public static BombShipTrollManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<BombShipTrollManager>();
		}
	}

	public EnemyBombShip Ship
	{
		get
		{
			if (_trollShip != null)
			{
				return _trollShip.GetComponent<EnemyBombShip>();
			}
			return null;
		}
	}

	public void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += ArrivedAtNextRoomHandler;
		GameManager.GameOver += GameOverHandler;
	}

	public void OnDisable()
	{
		CleanupShip();
		LevelManager.ArrivedAtNextRoom -= ArrivedAtNextRoomHandler;
		GameManager.GameOver -= GameOverHandler;
	}

	public void SpawnShip(Vector3 position)
	{
		if (_trollShip == null && trollShipPrefab != null)
		{
			_trollShip = UnityEngine.Object.Instantiate(trollShipPrefab, base.transform.position, Quaternion.identity) as GameObject;
			_trollShip.transform.position = position;
			_trollShip.transform.parent = base.transform;
			_trollShip.GetComponent<EnemyBombShip>().IsChasingFinger = false;
		}
	}

	public void StartChasing()
	{
		if (Ship != null)
		{
			Ship.IsChasingFinger = true;
		}
	}

	private void MoveShip(Vector3 position)
	{
		if (_trollShip != null)
		{
			_trollShip.transform.position = position;
			Ship.IsChasingFinger = false;
			InvokeHelper.InvokeSafe(StartChasing, 0.5f, this);
		}
	}

	public void ArrivedAtNextRoomHandler(object sender, LevelManager.NextRoomEventArgs args)
	{
		if (Ship != null && Ship.IsChasingFinger)
		{
			Vector3 vector = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, _trollShip.transform.position.z);
			switch (args.MoveDirection)
			{
			case LevelManager.MoveDirections.Down:
				MoveShip(vector + new Vector3(0f, 2f, 0f));
				break;
			case LevelManager.MoveDirections.Up:
				MoveShip(vector + new Vector3(0f, -2f, 0f));
				break;
			case LevelManager.MoveDirections.Right:
				MoveShip(vector + new Vector3(2f, 0f, 0f));
				break;
			}
		}
	}

	public void GameOverHandler(object sender, EventArgs args)
	{
		CleanupShip();
	}

	private void CleanupShip()
	{
		if (_trollShip != null)
		{
			UnityEngine.Object.Destroy(_trollShip);
		}
	}
}
