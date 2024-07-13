using UnityEngine;

public class DelayActivationUntilRoomArrival : MonoBehaviour
{
	[HideInInspector]
	public bool doDelay = true;

	private void OnEnable()
	{
		TrollBase trollBase = base.gameObject.GetComponent<TrollBase>();
		if (trollBase == null)
		{
			trollBase = GetComponentInChildren<TrollBase>();
		}
		if (trollBase != null)
		{
			trollBase.StartTrollBehaviour();
			Debug.Log("Started Troll Behavior on LevelManager.ArrivedAtNextRoom instead of Activator.RoomActivated.");
		}
		else
		{
			Debug.LogError("No troll base found in delayed activation component!");
		}
	}
}
