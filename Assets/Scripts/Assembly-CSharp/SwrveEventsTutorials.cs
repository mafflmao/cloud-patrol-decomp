using UnityEngine;

public class SwrveEventsTutorials
{
	private static float _roomTimer;

	public static float RoomDuration
	{
		get
		{
			return Time.time - _roomTimer;
		}
	}

	public static void StartRoomTimer()
	{
		_roomTimer = Time.time;
	}

	private static void SendEvent(string eventName)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "TimeInRoom", RoomDuration.ToString());
		SwrveEventsUtil.SendSwrveMessage(eventName, payload);
	}

	public static void TapShootCompleted()
	{
		SendEvent("Tutorials.TapShoot.Completed");
	}

	public static void ComboCompleted()
	{
		SendEvent("Tutorials.Combo.Completed");
	}

	public static void ComboFailed()
	{
		SendEvent("Tutorials.Combo.Failed");
	}

	public static void CoinCollectCompleted()
	{
		SendEvent("Tutorials.CoinCollect.Completed");
	}

	public static void MaxComboCompleted()
	{
		SendEvent("Tutorials.MaxCombo.Completed");
	}

	public static void MaxComboFailed()
	{
		SendEvent("Tutorials.MaxCombo.Failed");
	}

	public static void ComboCoinCollectCompleted()
	{
		SendEvent("Tutorials.ComboCoinCollect.Completed");
	}

	public static void MagicItemCollectCompleted()
	{
		SendEvent("Tutorials.MagicItem.CollectCompleted");
	}

	public static void MagicItemUsedCompleted()
	{
		SendEvent("Tutorials.MagicItem.UsedCompleted");
	}
}
