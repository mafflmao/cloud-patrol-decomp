using UnityEngine;

public class BombShipTrollIntro : MonoBehaviour
{
	private void Start()
	{
		BombShipTrollManager.Instance.SpawnShip(base.transform.position);
	}

	private void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += ArrivedAtRoom;
	}

	private void OnDisable()
	{
		LevelManager.ArrivedAtNextRoom -= ArrivedAtRoom;
	}

	private void ArrivedAtRoom(object sender, LevelManager.NextRoomEventArgs args)
	{
		InvokeHelper.InvokeSafe(EnableShip, 1f, this);
	}

	private void EnableShip()
	{
		BombShipTrollManager.Instance.StartChasing();
		Object.Destroy(base.gameObject);
	}
}
