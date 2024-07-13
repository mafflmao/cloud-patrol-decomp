public class SwrveEventsUI
{
	public static void BadUpgradeMessageSkipped()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Start.BadUpgradeMessageSkipped", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ActiveSkylander", Payload.ActiveSkylander));
	}

	public static void ContinueTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Pause.ContinueButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "CurrentArea", Payload.CurrentArea, "AreasCleared", Payload.AreasCleared));
	}

	public static void QuitTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Pause.QuitButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "CurrentArea", Payload.CurrentArea, "AreasCleared", Payload.AreasCleared));
	}

	public static void GoalButtonTouched(int currentGoalID)
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Loadout.GoalButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "GoalID", currentGoalID.ToString(), "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "CurrentCoinTotal", Payload.CurrentCoinTotal, "ActiveSkylander", Payload.ActiveSkylander));
	}

	public static void CollectionButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Loadout.CollectionButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveSkylander", Payload.ActiveSkylander));
	}

	public static void ElementGroupButtonTouched(string elementSelected)
	{
		SwrveEventsUtil.SendSwrveMessage("UI.SkylanderStore.ElementGroupButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "ElementOfTheDay", Payload.ElementOfTheDay, "ChosenElement", elementSelected));
	}

	public static void GemBarButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Loadout.GemButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ActiveSkylander", Payload.ActiveSkylander));
	}

	public static void CoinBarButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Loadout.CoinButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ActiveSkylander", Payload.ActiveSkylander));
	}

	public static void ElementOfTheDayTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Loadout.ElementButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ElementOfTheDay", Payload.ElementOfTheDay, "ActiveSkylander", Payload.ActiveSkylander));
	}

	public static void TutorialButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Help.PlayTutorialButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank));
	}

	public static void FAQButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Help.FAQButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank));
	}

	public static void FeedbackButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Help.FeedbackButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank));
	}

	public static void CreditsButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Help.CreditsButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank));
	}

	public static void SaveDataImportButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Help.CreditsButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank));
	}

	public static void SaveDataImportCompleted()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Help.SaveDataImportCompleted", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank));
	}

	public static void SaveDataImportFailed()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Help.SaveDataImportFailed", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank));
	}

	public static void SaveDataImportCancelled()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Help.SaveDataImportCancelled", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank));
	}

	public static void MagicItemTouched(string selectedItem)
	{
		SwrveEventsUtil.SendSwrveMessage("UI.MagicItemStore.MagicItemTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ChosenMagicItem", selectedItem, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "ElementOfTheDay", Payload.ElementOfTheDay));
	}

	public static void SkylanderTouched(string selectedCharacter)
	{
		SwrveEventsUtil.SendSwrveMessage("UI.SkylanderStore.SkylanderTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ChosenSkylander", selectedCharacter, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "ElementOfTheDay", Payload.ElementOfTheDay));
	}

	public static void MagicItemButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Collection.MagicItemButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "ElementOfTheDay", Payload.ElementOfTheDay));
	}

	public static void SkylanderButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Collection.SkylanderButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ActiveGoal1", Payload.ActiveGoal1, "ActiveGoal2", Payload.ActiveGoal2, "ActiveGoal3", Payload.ActiveGoal3, "ElementOfTheDay", Payload.ElementOfTheDay));
	}

	public static void GemButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Collection.GemButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ActiveSkylander", Payload.ActiveSkylander));
	}

	public static void CoinButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Collection.CoinButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ActiveSkylander", Payload.ActiveSkylander));
	}

	public static void AudioButtonTouched(bool toggle)
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Start.AudioButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ButtonToggleState", toggle.ToString()));
	}

	public static void MusicButtonTouched(bool toggle)
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Start.MusicButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ButtonToggleState", toggle.ToString()));
	}

	public static void HelpButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Start.HelpButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank));
	}

	public static void TwitterButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Start.TwitterButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank));
	}

	public static void FacebookButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Start.FacebookButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank));
	}

	public static void GCAchievementButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Start.GameCenterAchievementButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank));
	}

	public static void GCLeaderboardButtonTouched()
	{
		SwrveEventsUtil.SendSwrveMessage("UI.Start.GameCenterLeaderboardsButtonTouched", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank));
	}
}
