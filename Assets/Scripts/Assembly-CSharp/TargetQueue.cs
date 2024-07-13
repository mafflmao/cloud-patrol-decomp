using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetQueue : MonoBehaviour
{
	public Queue<GameObject> targetQueue = new Queue<GameObject>();

	private Dictionary<GameObject, TrackingCrosshair> trackingCrosshairPairs = new Dictionary<GameObject, TrackingCrosshair>();

	private Queue<GameObject> _trackingCrosshairPool = new Queue<GameObject>();

	public SoundEventData Target_SFX_Acquired;

	public SoundEventData Target_SFX_Acquired_Combo;

	public float targetPitchShiftAmount;

	public GameObject trackingCrosshair;

	public float bombKillDelayTime = 1f;

	public static bool bombActive;

	[NonSerialized]
	public TargetLine targetLine;

	[NonSerialized]
	public bool isAttachingCrosshair = true;

	private float pitch = 1f;

	public GameObject[] Targets
	{
		get
		{
			return targetQueue.ToArray();
		}
	}

	public int Count
	{
		get
		{
			if (targetQueue != null)
			{
				return targetQueue.Count;
			}
			return 0;
		}
	}

	public static event EventHandler<EventArgs> TargetAdded;

	public static event EventHandler<EventArgs> TargetRemoved;

	public static event EventHandler<EventArgs> TargetsCleared;

	private void Start()
	{
		for (int i = 0; i < 6; i++)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(trackingCrosshair);
			TrackingCrosshair component = gameObject.GetComponent<TrackingCrosshair>();
			component.Owner = this;
			_trackingCrosshairPool.Enqueue(gameObject);
		}
		targetLine = GetComponent<TargetLine>();
	}

	private void OnEnable()
	{
		LevelManager.MovingToNextRoom += Clear;
		GameManager.PlayerTookDamage += Clear;
		TrackingCrosshair.ReadyForRecycle += HandleTrackingCrosshairReadyForRecycle;
	}

	private void OnDisable()
	{
		LevelManager.MovingToNextRoom -= Clear;
		GameManager.PlayerTookDamage -= Clear;
		TrackingCrosshair.ReadyForRecycle -= HandleTrackingCrosshairReadyForRecycle;
	}

	private void OnDestroy()
	{
		foreach (GameObject item in _trackingCrosshairPool)
		{
			if (item != null)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		_trackingCrosshairPool.Clear();
		foreach (KeyValuePair<GameObject, TrackingCrosshair> trackingCrosshairPair in trackingCrosshairPairs)
		{
			if (trackingCrosshairPair.Value != null)
			{
				UnityEngine.Object.Destroy(trackingCrosshairPair.Value.gameObject);
			}
		}
		trackingCrosshairPairs.Clear();
	}

	public GameObject GetNextTarget()
	{
		pitch = 1f;
		GameObject gameObject = null;
		while (gameObject == null && targetQueue.Count > 0)
		{
			gameObject = targetQueue.Dequeue();
			SetTargetLock(gameObject);
		}
		return gameObject;
	}

	public void AddTarget(GameObject gameObject)
	{
		if (targetQueue.Contains(gameObject) || !(gameObject != Shooter.currentTarget))
		{
			return;
		}
		Health component = gameObject.GetComponent<Health>();
		if (component != null)
		{
			EnemyDodger component2 = gameObject.GetComponent<EnemyDodger>();
			if (component2 != null)
			{
				component2.Selected(this);
			}
		}
		targetQueue.Enqueue(gameObject);
		AttachCrosshair(gameObject);
		if (GameManager.gunSlotCount == targetQueue.Count)
		{
			SoundEventManager.Instance.Play(Target_SFX_Acquired_Combo, gameObject);
			PlayCrosshairFeedback();
		}
		else
		{
			if (targetQueue.Count > 0)
			{
				pitch += targetPitchShiftAmount;
			}
			SoundEventManager.Instance.Play(Target_SFX_Acquired, gameObject, 0f, pitch);
		}
		OnTargetAdded();
	}

	public void PlayCrosshairFeedback()
	{
		foreach (KeyValuePair<GameObject, TrackingCrosshair> trackingCrosshairPair in trackingCrosshairPairs)
		{
			if (trackingCrosshairPair.Value != null)
			{
				trackingCrosshairPair.Value.MaxOut();
			}
		}
	}

	public bool RemoveGameObject(GameObject gameObjectToRemove)
	{
		if (targetQueue.Contains(gameObjectToRemove))
		{
			RemoveCrosshair(gameObjectToRemove);
			pitch -= targetPitchShiftAmount;
			GameObject[] array = targetQueue.ToArray();
			targetQueue.Clear();
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				if (gameObject != gameObjectToRemove)
				{
					targetQueue.Enqueue(gameObject);
				}
			}
			OnTargetRemoved();
			return true;
		}
		return false;
	}

	public void Clear()
	{
		while (targetQueue.Count > 0)
		{
			GameObject nextTarget = GetNextTarget();
			OnTargetRemoved();
			RemoveCrosshair(nextTarget);
			OnTargetsCleared();
		}
	}

	public void Clear(object sender, EventArgs e)
	{
		InvokeHelper.InvokeSafe(Clear, 0.5f, this);
	}

	public bool Contains(GameObject gameObject)
	{
		return targetQueue.Contains(gameObject);
	}

	public GameObject GetTargetGameObject(int index)
	{
		return targetQueue.ElementAt(index);
	}

	private void HandleTrackingCrosshairReadyForRecycle(object sender, EventArgs args)
	{
		TrackingCrosshair trackingCrosshair = (TrackingCrosshair)sender;
		if (trackingCrosshair.Owner == this)
		{
			_trackingCrosshairPool.Enqueue(trackingCrosshair.gameObject);
		}
	}

	private void AttachCrosshair(GameObject attachee)
	{
		if (!(attachee == null))
		{
			if (isAttachingCrosshair && _trackingCrosshairPool.Count > 0)
			{
				GameObject gameObject = _trackingCrosshairPool.Dequeue();
				TrackingCrosshair component = gameObject.GetComponent<TrackingCrosshair>();
				component.BeginTarget(attachee);
				trackingCrosshairPairs.Add(attachee, component);
			}
			else
			{
				trackingCrosshairPairs.Add(attachee, null);
			}
		}
	}

	private void RemoveCrosshair(GameObject attachee)
	{
		TrackingCrosshair value;
		if (trackingCrosshairPairs.TryGetValue(attachee, out value))
		{
			if (value != null)
			{
				value.RemoveTarget();
			}
			trackingCrosshairPairs.Remove(attachee);
		}
	}

	private void SetTargetLock(GameObject attachee)
	{
		TrackingCrosshair value;
		if (trackingCrosshairPairs.TryGetValue(attachee, out value))
		{
			if (value != null)
			{
				value.TargetLock();
			}
			trackingCrosshairPairs.Remove(attachee);
		}
	}

	public void OnTargetAdded()
	{
		if (TargetQueue.TargetAdded != null)
		{
			TargetQueue.TargetAdded(this, new EventArgs());
		}
	}

	public void OnTargetRemoved()
	{
		if (TargetQueue.TargetRemoved != null)
		{
			TargetQueue.TargetRemoved(this, new EventArgs());
		}
	}

	public void OnTargetsCleared()
	{
		if (TargetQueue.TargetsCleared != null)
		{
			TargetQueue.TargetsCleared(this, new EventArgs());
		}
	}
}
