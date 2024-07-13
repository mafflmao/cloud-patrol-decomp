using System;
using UnityEngine;

public class DebugInfo : MonoBehaviour
{
	public enum DebugInfoMode
	{
		GLOBAL = 0,
		ROOM = 1
	}

	public SpriteText spriteTextA;

	public SpriteText spriteTextB;

	public UIButton button;

	private float calculatedFrameTime;

	private float calculatedMinFrameTime = 1000f;

	private float averageFrameTime;

	private int frameCounter;

	private int ftCounts;

	private float accumulatedFrameTime;

	private float accumulatedAverageFrameTime;

	private float currentRoomAverageFrameTime;

	private float lastRoomAverageFrameTime;

	private float accumulatedRoomAverageFrameTime;

	private string lastRoomName = "...";

	private string roomName = "...";

	private float worstFrameTime = 1000f;

	private string worstRoom = "...";

	public DebugInfoMode mode;

	private int roomFtCounts = 1;

	private void Start()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += BeginCalculatingRoomAverageFrameTime;
	}

	private void OnDisable()
	{
		LevelManager.ArrivedAtNextRoom -= BeginCalculatingRoomAverageFrameTime;
	}

	private void SwitchMode()
	{
		if (mode == DebugInfoMode.GLOBAL)
		{
			mode = DebugInfoMode.ROOM;
		}
		else
		{
			mode = DebugInfoMode.GLOBAL;
		}
	}

	private void CalculateFrameTime()
	{
		if (frameCounter % 30 == 0 && accumulatedFrameTime > 0f)
		{
			calculatedFrameTime = accumulatedFrameTime / 30f * 1000f;
			ftCounts++;
			if (calculatedFrameTime < calculatedMinFrameTime)
			{
				calculatedMinFrameTime = calculatedFrameTime;
			}
			if (calculatedFrameTime < worstFrameTime)
			{
				worstFrameTime = calculatedFrameTime;
				worstRoom = roomName;
			}
			accumulatedAverageFrameTime += calculatedFrameTime;
			averageFrameTime = accumulatedAverageFrameTime / (float)ftCounts;
			accumulatedFrameTime = 0f;
			frameCounter = 0;
		}
		else
		{
			accumulatedFrameTime += Time.deltaTime;
		}
		frameCounter++;
	}

	private void BeginCalculatingRoomAverageFrameTime(object sender, EventArgs args)
	{
		lastRoomName = roomName;
		roomName = ((!(LevelManager.Instance.currentScreenRoot != null)) ? "(NONE)" : LevelManager.Instance.currentScreenRoot.name);
		lastRoomAverageFrameTime = currentRoomAverageFrameTime;
		roomFtCounts = 1;
		accumulatedRoomAverageFrameTime = 0f;
		currentRoomAverageFrameTime = 0f;
	}

	private void CalculateRoomAverageFrameTime()
	{
		if (frameCounter % 30 == 0 && accumulatedFrameTime > 0f)
		{
			accumulatedRoomAverageFrameTime += calculatedFrameTime;
			currentRoomAverageFrameTime = accumulatedRoomAverageFrameTime / (float)roomFtCounts;
			roomFtCounts++;
		}
	}

	private void Update()
	{
		CalculateFrameTime();
		CalculateRoomAverageFrameTime();
		if (GameManager.debugMode)
		{
			spriteTextA.GetComponent<Renderer>().enabled = true;
			spriteTextB.GetComponent<Renderer>().enabled = true;
			switch (mode)
			{
			case DebugInfoMode.GLOBAL:
				spriteTextA.Text = "GLOBAL INFO\n----------\nVersion: " + GameManager.Instance.versionData.versionNumber + "\nFrameTime: " + calculatedFrameTime.ToString("F2") + "\nFrameTime(avg): " + averageFrameTime.ToString("F2");
				break;
			case DebugInfoMode.ROOM:
				spriteTextA.Text = "ROOM INFO\n----------\nRoom: " + roomName + "\nThis Room Avg FrameTime: " + currentRoomAverageFrameTime.ToString("F2") + "\nLast Room: " + lastRoomName + "\nLast Room Avg FrameTime: " + lastRoomAverageFrameTime.ToString("F2") + "\nWorst FrameTime: " + worstFrameTime.ToString("F2") + "\nWorst FrameTime Room: " + worstRoom;
				break;
			}
			spriteTextB.Text = spriteTextA.Text;
		}
		else
		{
			spriteTextA.GetComponent<Renderer>().enabled = false;
			spriteTextB.GetComponent<Renderer>().enabled = false;
		}
	}
}
