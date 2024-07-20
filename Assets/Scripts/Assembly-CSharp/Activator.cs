using System;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(Activator), LogLevel.Debug);

	public bool activeOnStart;

	public List<GameObject> delayedActivationGOs = new List<GameObject>();

	public static float minimumTriggerWidth = 13f;

	public static event EventHandler<ActivatorEventArgs> RoomActivated;

	private void OnEnable()
	{
		_log.LogDebug("OnEnable");
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
	}

	private void OnDisable()
	{
		_log.LogDebug("OnDisable");
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
	}

	private void Start()
	{
		_log.LogDebug("Start()");
		DelayActivationUntilRoomArrival[] componentsInChildren = base.gameObject.GetComponentsInChildren<DelayActivationUntilRoomArrival>();
		DelayActivationUntilRoomArrival[] array = componentsInChildren;
		foreach (DelayActivationUntilRoomArrival delayActivationUntilRoomArrival in array)
		{
			_log.LogDebug("Found DelayActivationUntilRoomArrival coponent on " + delayActivationUntilRoomArrival.gameObject.name);
			if (delayActivationUntilRoomArrival.doDelay)
			{
				delayedActivationGOs.Add(delayActivationUntilRoomArrival.gameObject);
			}
		}
		BoxCollider component = GetComponent<BoxCollider>();
		if (component != null && component.size.x < minimumTriggerWidth)
		{
			component.size = new Vector3(minimumTriggerWidth, component.size.y, component.size.z);
		}
	}

	public void SetChildrenActive(bool doAllEvenDelayed)
	{
		_log.LogDebug("SetChildrenActive({0})", doAllEvenDelayed);
		for (int i = 0; i < base.gameObject.transform.childCount; i++)
		{
			Transform child = base.gameObject.transform.GetChild(i);
			_log.LogDebug("Activating object {0}", child);
			GameObjectUtils.SetActiveRecursiveHack(base.gameObject, true);
		}
		if (!doAllEvenDelayed)
		{
			foreach (GameObject delayedActivationGO in delayedActivationGOs)
			{
				_log.LogDebug("De-Activating object {0}", delayedActivationGO.name);
				delayedActivationGO.SetActive(false);
			}
		}
		OnRoomActivated(new ActivatorEventArgs(base.gameObject));
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		_log.LogDebug("HandleLevelManagerArrivedAtNextRoom");
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
		if (delayedActivationGOs == null)
		{
			_log.LogError("delayedActivationGO's list was NULL! Setting it to placeholder (empty) value.");
			delayedActivationGOs = new List<GameObject>();
		}
		foreach (GameObject delayedActivationGO in delayedActivationGOs)
		{
			if (delayedActivationGO != null)
			{
				_log.LogDebug("Activating delayed activation object {0}", delayedActivationGO.name);
				delayedActivationGO.SetActive(true);
			}
			else
			{
				_log.LogError("Room had NULL object in delayted activation GO's list.");
			}
		}
		delayedActivationGOs.Clear();
	}

	public void SetChildrenInactive()
	{
		_log.LogDebug("SetChildrenInactive()");
		for (int i = 0; i < base.gameObject.transform.childCount; i++)
		{
			Transform child = base.gameObject.transform.GetChild(i);
			_log.LogDebug("Deactivating object {0}", child.gameObject.name);
			child.gameObject.SetActive(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other == Camera.main.GetComponent<Collider>() && base.GetComponent<Collider>().bounds.Intersects(other.bounds))
		{
			_log.Log("Camera entered room bounds. Activating children.");
			SetChildrenActive(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other == Camera.main.GetComponent<Collider>() && !base.GetComponent<Collider>().bounds.Intersects(other.bounds))
		{
			_log.Log("Camera exited room bounds. Deactivating and marking as cleared.");
			SetChildrenInactive();
			GameManager.RoomCleared();
		}
	}

	private void OnDrawGizmos()
	{
		DrawTriggerCube(Color.magenta);
	}

	private void OnDrawGizmosSelected()
	{
		DrawTriggerCube(Color.green);
	}

	private void DrawTriggerCube(Color color)
	{
		Gizmos.color = color;
		BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
		Vector3 center = base.transform.position + boxCollider.center;
		Gizmos.DrawWireCube(center, boxCollider.size);
	}

	protected void OnRoomActivated(ActivatorEventArgs args)
	{
		_log.LogDebug("OnRoomActivated(...)");
		if (Activator.RoomActivated != null)
		{
			Activator.RoomActivated(this, args);
		}
	}
}
