using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDayManager : SingletonMonoBehaviour
{
	private const int numBossRoomsPerLevel = 2;

	public List<TimeOfDayConfiguration> configurations;

	public float timeOfDay;

	public float transitionDelay = 1f;

	public float transitionTime = 1f;

	public float nightToDayShiftTime = 5f;

	public bool debugMode;

	public int roomsToMax = 10;

	public bool randomizeOnStart;

	private float _roomIncrement;

	private float _targetTimeOfDay;

	public static TimeOfDayManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<TimeOfDayManager>();
		}
	}

	private void OnEnable()
	{
		foreach (TimeOfDayConfiguration configuration in configurations)
		{
			configuration.RememberColors();
		}
		if (randomizeOnStart)
		{
			timeOfDay = UnityEngine.Random.value;
			_targetTimeOfDay = timeOfDay;
		}
		UpdateTimeOfDayTween(timeOfDay);
		LevelManager.RoomClear += AdvanceTimeOfDay;
		LevelManager.DifficultyUp += HandleLevelChange;
	}

	public void OnDisable()
	{
		LevelManager.RoomClear -= AdvanceTimeOfDay;
		LevelManager.LevelChanged -= HandleLevelChange;
		foreach (TimeOfDayConfiguration configuration in configurations)
		{
			configuration.RestoreColors();
		}
	}

	private void Start()
	{
		SetTransitionRoomCount(roomsToMax);
	}

	public void SetTransitionRoomCount(int count)
	{
		roomsToMax = count;
		if (roomsToMax > 0)
		{
			_roomIncrement = 1f / (float)roomsToMax;
		}
	}

	private void AdvanceTimeOfDay(object sender, EventArgs args)
	{
		timeOfDay = _targetTimeOfDay;
		_targetTimeOfDay = timeOfDay + _roomIncrement;
		if (timeOfDay < 1f)
		{
			iTween.ValueTo(base.gameObject, iTween.Hash("from", timeOfDay, "to", _targetTimeOfDay, "time", transitionTime, "delay", transitionDelay, "onupdate", "UpdateTimeOfDayTween", "easetype", "easeInOutSine"));
		}
	}

	private void Update()
	{
		if (debugMode)
		{
			UpdateTimeOfDayTween(timeOfDay);
		}
	}

	private void UpdateTimeOfDayTween(float val)
	{
		timeOfDay = Mathf.Clamp01(val);
		foreach (TimeOfDayConfiguration configuration in configurations)
		{
			configuration.SetTime(timeOfDay);
		}
	}

	private void HandleLevelChange(object sender, EventArgs args)
	{
	}

	public void LevelChangeComplete()
	{
	}

	public void UpdateGameObjectOriginalColor(GameObject go, Color newColor)
	{
		foreach (TimeOfDayConfiguration configuration in configurations)
		{
			configuration.UpdateGameObjectOriginalColor(go, newColor);
		}
	}
}
