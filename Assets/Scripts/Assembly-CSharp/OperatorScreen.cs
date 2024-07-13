using UnityEngine;

public class OperatorScreen : MonoBehaviour
{
	public SpriteText m_CreditsPerGame;

	public SpriteText m_GameAudioVolume;

	public SpriteText m_AttractAudioVolume;

	public SpriteText m_PointsPerTicket;

	public SpriteText m_RedemptionMode;

	public SpriteText m_FixedTicketPayout;

	public SpriteText m_MinimumTicketPayout;

	public SpriteText m_TicketValue;

	public SpriteText m_PaymentType;

	public SpriteText m_RedemptionUnit;

	public SpriteText m_ShowIntroVideo;

	public SpriteText m_CountdownPlayAgain;

	public SpriteText m_CreditsPerPlayAgain;

	public SpriteText m_ShortGameMode;

	public Camera m_Camera;

	public GameObject m_OperatorMenuPanel;

	public GameObject m_StatsPanel;

	public SpriteText m_Version;

	public SpriteText m_LT_NumberOfTickets;

	public SpriteText m_LT_AvgTicketsPerGame;

	public SpriteText m_LT_AvgPointsPerGame;

	public SpriteText m_LT_TotalGames;

	public SpriteText m_LT_TotalCredits;

	public SpriteText m_SLR_NumberOfTickets;

	public SpriteText m_SLR_AvgTicketsPerGame;

	public SpriteText m_SLR_AvgPointsPerGame;

	public SpriteText m_SLR_TotalGames;

	public SpriteText m_SLR_TotalCredits;

	public SpriteText m_SLR_Date;

	public GameObject m_GO_RedemptionMode;

	public GameObject m_GO_PtsPerTicket;

	public GameObject m_GO_FixedTicketPayout;

	public GameObject m_GO_MinimumTicketPayout;

	public GameObject m_GO_TicketValue;

	public GameObject m_GO_RedemptionUnit;

	public GameObject m_GO_LT_NumberOfTickets;

	public GameObject m_GO_LT_AvgTicketsPerGame;

	public GameObject m_GO_SLR_NumberOfTickets;

	public GameObject m_GO_SLR_AvgTicketsPerGame;

	private void OnDestroy()
	{
		if (ProfileManager.Instance != null)
		{
			ProfileManager.Instance.SaveGame();
		}
	}

	private void Start()
	{
		MoviePlayer.Instance.EndMovie(true);
		InitAllText();
		if (LoadingPanel.InstanceNoAutocreate != null)
		{
			LoadingPanel.InstanceNoAutocreate.Dismiss();
		}
		m_Camera.clearFlags = CameraClearFlags.Color;
		m_Camera.backgroundColor = Color.black;
		onOperator();
	}

	private void InitAllText()
	{
		m_Version.Text = "Version " + OperatorMenu.Version.ToString("f2");
		RefreshCreditPerGameText();
		RefreshGameAudioVolumeText();
		RefreshAttractAudioVolumeText();
		RefreshPointsPerTicketText();
		RefreshRedemptionModeText();
		RefreshFixedTicketPayoutText();
		RefreshMinimumTicketPayoutText();
		RefreshTicketValueText();
		RefreshPaymentTypeText();
		RefreshRedemptionUnit();
		RefreshShowIntroVideo();
		RefreshCountdownPlayAgainText();
		RefreshCreditsPerPlayAgainText();
		RefreshShortGameMode();
		if (OperatorMenu.Instance.m_IsItalieBuild)
		{
			if (m_GO_RedemptionMode != null)
			{
				m_GO_RedemptionMode.SetActive(false);
			}
			if (m_GO_PtsPerTicket != null)
			{
				m_GO_PtsPerTicket.SetActive(false);
			}
			if (m_GO_FixedTicketPayout != null)
			{
				m_GO_FixedTicketPayout.SetActive(false);
			}
			if (m_GO_MinimumTicketPayout != null)
			{
				m_GO_MinimumTicketPayout.SetActive(false);
			}
			if (m_GO_TicketValue != null)
			{
				m_GO_TicketValue.SetActive(false);
			}
			if (m_GO_RedemptionUnit != null)
			{
				m_GO_RedemptionUnit.SetActive(false);
			}
			if (m_GO_LT_NumberOfTickets != null)
			{
				m_GO_LT_NumberOfTickets.SetActive(false);
			}
			if (m_GO_LT_AvgTicketsPerGame != null)
			{
				m_GO_LT_AvgTicketsPerGame.SetActive(false);
			}
			if (m_GO_SLR_NumberOfTickets != null)
			{
				m_GO_SLR_NumberOfTickets.SetActive(false);
			}
			if (m_GO_SLR_AvgTicketsPerGame != null)
			{
				m_GO_SLR_AvgTicketsPerGame.SetActive(false);
			}
		}
		InitStatsText();
	}

	private void InitStatsText()
	{
		m_LT_NumberOfTickets.Text = OperatorMenu.Instance.m_LT_NumberOfTickets.ToString();
		m_LT_AvgTicketsPerGame.Text = OperatorMenu.Instance.GetAvgTicketPerGame(true).ToString("F2");
		m_LT_AvgPointsPerGame.Text = OperatorMenu.Instance.GetAvgPointsPerGame(true).ToString("F2");
		m_LT_TotalGames.Text = OperatorMenu.Instance.GetTotalGames(true).ToString();
		m_LT_TotalCredits.Text = OperatorMenu.Instance.GetTotalCredits(true).ToString();
		m_SLR_NumberOfTickets.Text = OperatorMenu.Instance.m_SLR_NumberOfTickets.ToString();
		m_SLR_AvgTicketsPerGame.Text = OperatorMenu.Instance.GetAvgTicketPerGame(false).ToString("F2");
		m_SLR_AvgPointsPerGame.Text = OperatorMenu.Instance.GetAvgPointsPerGame(false).ToString("F2");
		m_SLR_TotalGames.Text = OperatorMenu.Instance.GetTotalGames(false).ToString();
		m_SLR_TotalCredits.Text = OperatorMenu.Instance.GetTotalCredits(false).ToString();
		m_SLR_Date.Text = OperatorMenu.Instance.m_Date_Since_Last_Reset.ToString() + " / " + OperatorMenu.Instance.m_Month_Since_Last_Reset.ToString() + " / " + OperatorMenu.Instance.m_Year_Since_Last_Reset;
	}

	private void OnResetHighScores()
	{
		LeaderboardManager.Instance.ResetData();
		ProfileManager.Instance.SaveGame();
	}

	private void onOperator()
	{
		m_OperatorMenuPanel.SetActive(true);
		m_StatsPanel.SetActive(false);
	}

	private void OnStats()
	{
		m_OperatorMenuPanel.SetActive(false);
		m_StatsPanel.SetActive(true);
	}

	private void OnResetStats()
	{
		OperatorMenu.Instance.ResetSinceLastResetStats();
		InitStatsText();
	}

	private void OnResume()
	{
		m_Camera.clearFlags = CameraClearFlags.Nothing;
		ProgressionManager.Instance.OnOperatorBtnClicked();
	}

	private void OnQuit()
	{
		if (ProfileManager.Instance != null)
		{
			ProfileManager.Instance.SaveGame();
		}
		Application.Quit();
	}

	private void onCreditsPerGameDown()
	{
		OperatorMenu.Instance.m_CreditsPerGame--;
		if (OperatorMenu.Instance.m_CreditsPerGame < 0)
		{
			OperatorMenu.Instance.m_CreditsPerGame = 20;
		}
		RefreshCreditPerGameText();
	}

	private void onCreditsPerGameUp()
	{
		OperatorMenu.Instance.m_CreditsPerGame++;
		if (OperatorMenu.Instance.m_CreditsPerGame > 20)
		{
			OperatorMenu.Instance.m_CreditsPerGame = 0;
		}
		RefreshCreditPerGameText();
	}

	private void RefreshCreditPerGameText()
	{
		m_CreditsPerGame.Text = OperatorMenu.Instance.m_CreditsPerGame.ToString();
	}

	private void onGameAudioVolumDown()
	{
		OperatorMenu.Instance.m_GameAudioVolume--;
		if (OperatorMenu.Instance.m_GameAudioVolume < 0)
		{
			OperatorMenu.Instance.m_GameAudioVolume = 20;
		}
		RefreshGameAudioVolumeText();
	}

	private void onGameAudioVolumUp()
	{
		OperatorMenu.Instance.m_GameAudioVolume++;
		if (OperatorMenu.Instance.m_GameAudioVolume > 20)
		{
			OperatorMenu.Instance.m_GameAudioVolume = 0;
		}
		RefreshGameAudioVolumeText();
	}

	private void RefreshGameAudioVolumeText()
	{
		m_GameAudioVolume.Text = OperatorMenu.Instance.m_GameAudioVolume.ToString();
	}

	private void onAttractAudioVolumDown()
	{
		OperatorMenu.Instance.m_AttractModeVolume--;
		if (OperatorMenu.Instance.m_AttractModeVolume < 0)
		{
			OperatorMenu.Instance.m_AttractModeVolume = 20;
		}
		RefreshAttractAudioVolumeText();
	}

	private void onAttractAudioVolumUp()
	{
		OperatorMenu.Instance.m_AttractModeVolume++;
		if (OperatorMenu.Instance.m_AttractModeVolume > 20)
		{
			OperatorMenu.Instance.m_AttractModeVolume = 0;
		}
		RefreshAttractAudioVolumeText();
	}

	private void RefreshAttractAudioVolumeText()
	{
		m_AttractAudioVolume.Text = OperatorMenu.Instance.m_AttractModeVolume.ToString();
	}

	private void onPointsPerTicketDown()
	{
		OperatorMenu.Instance.m_PointsPerTicket -= 100;
		if (OperatorMenu.Instance.m_PointsPerTicket < 0)
		{
			OperatorMenu.Instance.m_PointsPerTicket = 0;
		}
		RefreshPointsPerTicketText();
	}

	private void onPointsPerTicketUp()
	{
		OperatorMenu.Instance.m_PointsPerTicket += 100;
		RefreshPointsPerTicketText();
	}

	private void RefreshPointsPerTicketText()
	{
		m_PointsPerTicket.Text = OperatorMenu.Instance.m_PointsPerTicket.ToString();
	}

	private void onToggleRedemptionMode()
	{
		OperatorMenu.Instance.m_RedemptionMode = !OperatorMenu.Instance.m_RedemptionMode;
		RefreshRedemptionModeText();
	}

	private void RefreshRedemptionModeText()
	{
		m_RedemptionMode.Text = OperatorMenu.Instance.m_RedemptionMode.ToString();
	}

	private void onFixedTicketPayoutDown()
	{
		OperatorMenu.Instance.m_FixedTicketPayout--;
		if (OperatorMenu.Instance.m_FixedTicketPayout < 0)
		{
			OperatorMenu.Instance.m_FixedTicketPayout = 50;
		}
		RefreshFixedTicketPayoutText();
	}

	private void onFixedTicketPayoutUp()
	{
		OperatorMenu.Instance.m_FixedTicketPayout++;
		if (OperatorMenu.Instance.m_FixedTicketPayout > 50)
		{
			OperatorMenu.Instance.m_FixedTicketPayout = 0;
		}
		RefreshFixedTicketPayoutText();
	}

	private void RefreshFixedTicketPayoutText()
	{
		m_FixedTicketPayout.Text = OperatorMenu.Instance.m_FixedTicketPayout.ToString();
	}

	private void onMinimumTicketPayoutDown()
	{
		OperatorMenu.Instance.m_MinimumTicketPayout--;
		if (OperatorMenu.Instance.m_MinimumTicketPayout < 0)
		{
			OperatorMenu.Instance.m_MinimumTicketPayout = 20;
		}
		RefreshMinimumTicketPayoutText();
	}

	private void onMinimumTicketPayoutUp()
	{
		OperatorMenu.Instance.m_MinimumTicketPayout++;
		if (OperatorMenu.Instance.m_MinimumTicketPayout > 20)
		{
			OperatorMenu.Instance.m_MinimumTicketPayout = 0;
		}
		RefreshMinimumTicketPayoutText();
	}

	private void RefreshMinimumTicketPayoutText()
	{
		m_MinimumTicketPayout.Text = OperatorMenu.Instance.m_MinimumTicketPayout.ToString();
	}

	private void onToggleTicketValue()
	{
		if (OperatorMenu.Instance.m_TicketValue == 1)
		{
			OperatorMenu.Instance.m_TicketValue = 2;
		}
		else
		{
			OperatorMenu.Instance.m_TicketValue = 1;
		}
		RefreshTicketValueText();
	}

	private void RefreshTicketValueText()
	{
		m_TicketValue.Text = OperatorMenu.Instance.m_TicketValue.ToString();
	}

	private void onTogglePaymentType()
	{
		if (OperatorMenu.Instance.m_PaymentType == 1)
		{
			OperatorMenu.Instance.m_PaymentType = 2;
		}
		else
		{
			OperatorMenu.Instance.m_PaymentType = 1;
		}
		RefreshPaymentTypeText();
	}

	private void RefreshPaymentTypeText()
	{
		if (OperatorMenu.Instance.m_PaymentType == 1)
		{
			m_PaymentType.Text = "CREDIT";
		}
		else
		{
			m_PaymentType.Text = "CARD";
		}
	}

	private void onToggleRedemptionUnit()
	{
		if (OperatorMenu.Instance.m_RedemptionUnit == 1)
		{
			OperatorMenu.Instance.m_RedemptionUnit = 2;
		}
		else
		{
			OperatorMenu.Instance.m_RedemptionUnit = 1;
		}
		RefreshRedemptionUnit();
	}

	private void RefreshRedemptionUnit()
	{
		if (OperatorMenu.Instance.m_RedemptionUnit == 1)
		{
			m_RedemptionUnit.Text = "TICKETS";
		}
		else
		{
			m_RedemptionUnit.Text = "COUPONS";
		}
	}

	private void onToggleShowIntroVideo()
	{
		OperatorMenu.Instance.m_ShowIntroVideo = !OperatorMenu.Instance.m_ShowIntroVideo;
		RefreshShowIntroVideo();
	}

	private void RefreshShowIntroVideo()
	{
		m_ShowIntroVideo.Text = OperatorMenu.Instance.m_ShowIntroVideo.ToString();
	}

	private void onCountdownPlayAgainDown()
	{
		OperatorMenu.Instance.m_CountdownPlayAgain--;
		if (OperatorMenu.Instance.m_CountdownPlayAgain < 5)
		{
			OperatorMenu.Instance.m_CountdownPlayAgain = 60;
		}
		RefreshCountdownPlayAgainText();
	}

	private void onCountdownPlayAgainUp()
	{
		OperatorMenu.Instance.m_CountdownPlayAgain++;
		if (OperatorMenu.Instance.m_CountdownPlayAgain > 60)
		{
			OperatorMenu.Instance.m_CountdownPlayAgain = 5;
		}
		RefreshCountdownPlayAgainText();
	}

	private void RefreshCountdownPlayAgainText()
	{
		m_CountdownPlayAgain.Text = OperatorMenu.Instance.m_CountdownPlayAgain.ToString();
	}

	private void onCreditsPerPlayAgainDown()
	{
		OperatorMenu.Instance.m_CreditsPerPlayAgain--;
		if (OperatorMenu.Instance.m_CreditsPerPlayAgain < 1)
		{
			OperatorMenu.Instance.m_CreditsPerPlayAgain = 20;
		}
		RefreshCreditsPerPlayAgainText();
	}

	private void onCreditsPerPlayAgainUp()
	{
		OperatorMenu.Instance.m_CreditsPerPlayAgain++;
		if (OperatorMenu.Instance.m_CreditsPerPlayAgain > 20)
		{
			OperatorMenu.Instance.m_CreditsPerPlayAgain = 1;
		}
		RefreshCreditsPerPlayAgainText();
	}

	private void RefreshCreditsPerPlayAgainText()
	{
		m_CreditsPerPlayAgain.Text = OperatorMenu.Instance.m_CreditsPerPlayAgain.ToString();
	}

	private void onToggleShortGameMode()
	{
		OperatorMenu.Instance.m_ShortGameMode = !OperatorMenu.Instance.m_ShortGameMode;
		RefreshShortGameMode();
	}

	private void RefreshShortGameMode()
	{
		m_ShortGameMode.Text = OperatorMenu.Instance.m_ShortGameMode.ToString();
	}
}
