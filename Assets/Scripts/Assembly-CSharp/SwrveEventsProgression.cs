public class SwrveEventsProgression
{
	public static void RankAwarded(int gemsAwarded)
	{
		SwrveEventsUtil.SendSwrveMessage("Progression.Ranking.Awarded", BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "GemsAwarded", gemsAwarded.ToString()));
	}

	public static void PrestigeRankAwarded(int gemsAwarded)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "GemsAwarded", gemsAwarded.ToString());
		SwrveEventsUtil.SendSwrveMessage("Progression.Ranking.PrestigeAwarded", payload);
	}

	public static void GoalNew(int goalID)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "GoalID", goalID.ToString(), "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay);
		SwrveEventsUtil.SendSwrveMessage("Progression.Goal.New", payload);
	}

	public static void GoalCompleted(int goalID, int starsAwarded)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "GoalID", goalID.ToString(), "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "StarsAwarded", starsAwarded.ToString());
		SwrveEventsUtil.SendSwrveMessage("Progression.Goal.Completed", payload);
	}

	public static void ChallengeJoined(uint challengeId)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ChallengeID", challengeId.ToString(), "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay);
		SwrveEventsUtil.SendSwrveMessage("Progression.Challenge.Joined", payload);
	}

	public static void ChallengeStopped(uint challengeId)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ChallengeID", challengeId.ToString(), "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay);
		SwrveEventsUtil.SendSwrveMessage("Progression.Challenge.Stopped", payload);
	}

	public static void ChallengeFinished(uint challengeId, bool timedOut)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ChallengeID", challengeId.ToString(), "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "ChallengeTimedOut", timedOut.ToString());
		SwrveEventsUtil.SendSwrveMessage("Progression.Challenge.Finished", payload);
	}

	public static void AchievementAwarded(string achievementName)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "AchievementName", achievementName, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay);
		SwrveEventsUtil.SendSwrveMessage("Progression.Achievements.Awarded", payload);
	}

	public static void ElementalBonusCoinsAwarded(int coinsAwarded)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "CoinsAwarded", coinsAwarded.ToString());
		SwrveEventsUtil.SendSwrveMessage("Progression.Results.ElementalBonusCoinsAwarded", payload);
	}

	public static void PresentGemsAwarded(int gemsAwarded)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "CurrentCoinTotal", Payload.CurrentCoinTotal, "CurrentGemTotal", Payload.CurrentGemTotal, "ElementOfTheDay", Payload.ElementOfTheDay, "GemsAwarded", gemsAwarded.ToString());
		SwrveEventsUtil.SendSwrveMessage("Progression.Results.PresentGemsAwarded", payload);
	}

	public static void ToyRegistration(int gemsReimbursed, string toyName, bool usedPortal)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "RegisteredToy", toyName, "UsedPortalToUnlock", usedPortal.ToString(), "GemsReimbursed", gemsReimbursed.ToString());
		SwrveEventsUtil.SendSwrveMessage("Progression.Toy.Registration", payload);
	}

	public static void ToyRegistrationCancelled(string toyName)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "RegisteredToy", toyName);
		SwrveEventsUtil.SendSwrveMessage("Progression.Toy.RegistrationCancelled", payload);
	}

	public static void ToyRegistrationServerFailed(string toyName, string failReason)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentRank", Payload.CurrentRank, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "RegisteredToy", toyName, "ServerFailReason", failReason);
		SwrveEventsUtil.SendSwrveMessage("Progression.Toy.RegistrationServerFailed", payload);
	}

	public static void PortalLinkEnterScreen(string toyName, int gemsToReimburse)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentGemTotal", Payload.CurrentGemTotal, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "GemsToReimburse", gemsToReimburse.ToString(), "Device", Payload.Device, "ToyToRegister", toyName);
		SwrveEventsUtil.SendSwrveMessage("Progression.Portal.EnterScreen", payload);
	}

	public static void PortalLinkConnectedPortal(string toyName, int gemsToReimburse)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentGemTotal", Payload.CurrentGemTotal, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "GemsToReimburse", gemsToReimburse.ToString(), "Device", Payload.Device, "ToyToRegister", toyName);
		SwrveEventsUtil.SendSwrveMessage("Progression.Portal.ConnectedPortal", payload);
	}

	public static void PortalLinkToySuccess(string toyName, int gemsToReimburse)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentGemTotal", Payload.CurrentGemTotal, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "GemsToReimburse", gemsToReimburse.ToString(), "Device", Payload.Device, "ToyToRegister", toyName);
		SwrveEventsUtil.SendSwrveMessage("Progression.Portal.ToySuccess", payload);
	}

	public static void PortalLinkWrongToy(string toyName, int gemsToReimburse)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentGemTotal", Payload.CurrentGemTotal, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "GemsToReimburse", gemsToReimburse.ToString(), "Device", Payload.Device, "ToyToRegister", toyName);
		SwrveEventsUtil.SendSwrveMessage("Progression.Portal.WrongToy", payload);
	}

	public static void PortalLinkHelpButtonHit(string toyName, int gemsToReimburse, bool portalWasConnected, bool wrongToyWasPlaced)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentGemTotal", Payload.CurrentGemTotal, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "GemsToReimburse", gemsToReimburse.ToString(), "Device", Payload.Device, "WrongToyWasPlaced", wrongToyWasPlaced.ToString(), "PortalWasConnected", portalWasConnected.ToString(), "ToyToRegister", toyName);
		SwrveEventsUtil.SendSwrveMessage("Progression.Portal.HelpButtonHit", payload);
	}

	public static void PortalLinkLinkToRegisteredYes(string toyName, int gemsToReimburse)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentGemTotal", Payload.CurrentGemTotal, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "GemsToReimburse", gemsToReimburse.ToString(), "Device", Payload.Device, "ToyToRegister", toyName);
		SwrveEventsUtil.SendSwrveMessage("Progression.Portal.LinkToRegisteredYes", payload);
	}

	public static void PortalLinkLinkToRegisteredNo(string toyName, int gemsToReimburse)
	{
		Bedrock.brKeyValueArray payload = BedrockUtils.Hash("CurrentGemTotal", Payload.CurrentGemTotal, "ActiveSkylander", Payload.ActiveSkylander, "ElementOfTheDay", Payload.ElementOfTheDay, "GemsToReimburse", gemsToReimburse.ToString(), "Device", Payload.Device, "ToyToRegister", toyName);
		SwrveEventsUtil.SendSwrveMessage("Progression.Portal.LinkToRegisteredNo", payload);
	}
}
