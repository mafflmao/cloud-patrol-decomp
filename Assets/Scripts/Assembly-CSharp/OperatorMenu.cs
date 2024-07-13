using System;
using System.IO;
using UnityEngine;
using XmlTool;

public class OperatorMenu : MonoBehaviour, IGameData
{
	public const int PAYMENT_TYPE_CREDITS = 1;

	public const int PAYMENT_TYPE_SWIPE_CARD = 2;

	public const int REDEMPTION_TYPE_TICKET = 1;

	public const int REDEMPTION_TYPE_COUPONS = 2;

	public ProfileManager.ExecutionOrder m_ExecutionOrder;

	[HideInInspector]
	public int m_CreditsPerGame;

	[HideInInspector]
	public int m_GameAudioVolume;

	[HideInInspector]
	public int m_AttractModeVolume;

	[HideInInspector]
	public int m_PointsPerTicket;

	[HideInInspector]
	public bool m_RedemptionMode;

	[HideInInspector]
	public int m_FixedTicketPayout;

	[HideInInspector]
	public int m_MinimumTicketPayout;

	[HideInInspector]
	public int m_TicketValue;

	[HideInInspector]
	public int m_PaymentType;

	[HideInInspector]
	public int m_RedemptionUnit;

	[HideInInspector]
	public bool m_ShowIntroVideo;

	[HideInInspector]
	public int m_CountdownPlayAgain;

	[HideInInspector]
	public int m_CreditsPerPlayAgain;

	[HideInInspector]
	public bool m_ShortGameMode;

	[HideInInspector]
	public long m_LT_NumberOfTickets;

	[HideInInspector]
	public uint m_LT_TotalGames;

	[HideInInspector]
	public long m_LT_TotalPoints;

	[HideInInspector]
	public uint m_LT_TotalCredits;

	[HideInInspector]
	public long m_SLR_NumberOfTickets;

	[HideInInspector]
	public uint m_SLR_TotalGames;

	[HideInInspector]
	public long m_SLR_TotalPoints;

	[HideInInspector]
	public uint m_SLR_TotalCredits;

	[HideInInspector]
	public int m_Date_Since_Last_Reset;

	[HideInInspector]
	public int m_Month_Since_Last_Reset;

	[HideInInspector]
	public int m_Year_Since_Last_Reset;

	[HideInInspector]
	public int m_Pts_Removed_Per_Bombs;

	[HideInInspector]
	public int m_Pts_Removed_Per_Projectile;

	[HideInInspector]
	public int m_Pts_Added_Per_Coins;

	[HideInInspector]
	public int m_ScoreBonus1;

	[HideInInspector]
	public int m_ScoreBonus2;

	[HideInInspector]
	public int m_ScoreBonus3;

	private string m_HugeStringToSave;

	public bool m_InGame;

	private static OperatorMenu m_Instance;

	[HideInInspector]
	public bool m_IsItalieBuild;

	private string characters = "0123456789abcdefghijklmnopqrstuvwxABCDEFGHIJKLMNOPQRSTUVWXYZ";

	public ProfileManager.ExecutionOrder ExecutionOrder
	{
		get
		{
			return m_ExecutionOrder;
		}
	}

	public static OperatorMenu Instance
	{
		get
		{
			return m_Instance;
		}
	}

	public static float Version
	{
		get
		{
			return 1.11f;
		}
	}

	private void Awake()
	{
		m_IsItalieBuild = false;
		ResetData();
	}

	private void Start()
	{
		m_Instance = this;
		m_InGame = false;
		GenerateHugeStringToSave();
		ResetData();
		ResetStats();
		InitAllValue();
		Register();
	}

	private void GenerateHugeStringToSave()
	{
		m_HugeStringToSave = RandomString(8500);
	}

	private string RandomString(int size)
	{
		string text = string.Empty;
		for (int i = 0; i < size; i++)
		{
			int index = UnityEngine.Random.Range(0, characters.Length);
			text += characters[index];
		}
		return text;
	}

	public void Register()
	{
		ProfileManager.Instance.Register(this);
	}

	public void UnRegister()
	{
		ProfileManager.Instance.Unregister(this);
	}

	public void SaveGame(StreamWriter i_Writer)
	{
		i_Writer.WriteLine("\t<OperatorMenu>");
		i_Writer.WriteLine("\t\t<CreditsPerGame>" + m_CreditsPerGame + "</CreditsPerGame>");
		i_Writer.WriteLine("\t\t<GameAudioVolume>" + m_GameAudioVolume + "</GameAudioVolume>");
		i_Writer.WriteLine("\t\t<AttractModeVolume>" + m_AttractModeVolume + "</AttractModeVolume>");
		i_Writer.WriteLine("\t\t<PointsPerTicket>" + m_PointsPerTicket + "</PointsPerTicket>");
		i_Writer.WriteLine("\t\t<RedemptionMode>" + m_RedemptionMode + "</RedemptionMode>");
		i_Writer.WriteLine("\t\t<FixedTicketPayout>" + m_FixedTicketPayout + "</FixedTicketPayout>");
		i_Writer.WriteLine("\t\t<MinimumTicketPayout>" + m_MinimumTicketPayout + "</MinimumTicketPayout>");
		i_Writer.WriteLine("\t\t<TicketValue>" + m_TicketValue + "</TicketValue>");
		i_Writer.WriteLine("\t\t<PaymentType>" + m_PaymentType + "</PaymentType>");
		i_Writer.WriteLine("\t\t<RedemptionUnit>" + m_RedemptionUnit + "</RedemptionUnit>");
		i_Writer.WriteLine("\t\t<ShowIntroVideo>" + m_ShowIntroVideo + "</ShowIntroVideo>");
		i_Writer.WriteLine("\t\t<CountdownPlayAgain>" + m_CountdownPlayAgain + "</CountdownPlayAgain>");
		i_Writer.WriteLine("\t\t<CreditsPerPlayAgain>" + m_CreditsPerPlayAgain + "</CreditsPerPlayAgain>");
		i_Writer.WriteLine("\t\t<ShortGameMode>" + m_ShortGameMode + "</ShortGameMode>");
		i_Writer.WriteLine("\t\t<NumberOfTickets>" + m_LT_NumberOfTickets + "</NumberOfTickets>");
		i_Writer.WriteLine("\t\t<TotalGames>" + m_LT_TotalGames + "</TotalGames>");
		i_Writer.WriteLine("\t\t<TotalPoints>" + m_LT_TotalPoints + "</TotalPoints>");
		i_Writer.WriteLine("\t\t<TotalCredits>" + m_LT_TotalCredits + "</TotalCredits>");
		i_Writer.WriteLine("\t\t<PtsRemovedPerBombs>" + m_Pts_Removed_Per_Bombs + "</PtsRemovedPerBombs>");
		i_Writer.WriteLine("\t\t<PtsRemovedPerProjectile>" + m_Pts_Removed_Per_Projectile + "</PtsRemovedPerProjectile>");
		i_Writer.WriteLine("\t\t<PtsAddedPerCoins>" + m_Pts_Added_Per_Coins + "</PtsAddedPerCoins>");
		i_Writer.WriteLine("\t\t<HugeStringToSave>" + m_HugeStringToSave + "</HugeStringToSave>");
		i_Writer.WriteLine("\t\tScoreBonus1>" + m_ScoreBonus1 + "</ScoreBonus1>");
		i_Writer.WriteLine("\t\tScoreBonus2>" + m_ScoreBonus2 + "</ScoreBonus2>");
		i_Writer.WriteLine("\t\tScoreBonus3>" + m_ScoreBonus3 + "</ScoreBonus3>");
		i_Writer.WriteLine("\t</OperatorMenu>");
	}

	public void LoadGame(XmlNode i_RootNode)
	{
		XmlNode child = i_RootNode.GetChild("OperatorMenu");
		if (child != null)
		{
			m_CreditsPerGame = int.Parse(child.GetChild("CreditsPerGame").GetElement());
			m_GameAudioVolume = int.Parse(child.GetChild("GameAudioVolume").GetElement());
			m_AttractModeVolume = int.Parse(child.GetChild("AttractModeVolume").GetElement());
			m_PointsPerTicket = int.Parse(child.GetChild("PointsPerTicket").GetElement());
			m_RedemptionMode = bool.Parse(child.GetChild("RedemptionMode").GetElement());
			m_FixedTicketPayout = int.Parse(child.GetChild("FixedTicketPayout").GetElement());
			m_MinimumTicketPayout = int.Parse(child.GetChild("MinimumTicketPayout").GetElement());
			m_TicketValue = int.Parse(child.GetChild("TicketValue").GetElement());
			m_PaymentType = int.Parse(child.GetChild("PaymentType").GetElement());
			m_RedemptionUnit = int.Parse(child.GetChild("RedemptionUnit").GetElement());
			if (child.GetChild("ShowIntroVideo") != null)
			{
				m_ShowIntroVideo = bool.Parse(child.GetChild("ShowIntroVideo").GetElement());
			}
			if (child.GetChild("CountdownPlayAgain") != null)
			{
				m_CountdownPlayAgain = int.Parse(child.GetChild("CountdownPlayAgain").GetElement());
			}
			if (child.GetChild("CreditsPerPlayAgain") != null)
			{
				m_CreditsPerPlayAgain = int.Parse(child.GetChild("CreditsPerPlayAgain").GetElement());
			}
			if (child.GetChild("ShortGameMode") != null)
			{
				m_ShortGameMode = bool.Parse(child.GetChild("ShortGameMode").GetElement());
			}
			if (child.GetChild("ScoreBonus1") != null)
			{
				m_ScoreBonus1 = int.Parse(child.GetChild("ScoreBonus1").GetElement());
			}
			if (child.GetChild("ScoreBonus2") != null)
			{
				m_ScoreBonus2 = int.Parse(child.GetChild("ScoreBonus2").GetElement());
			}
			if (child.GetChild("ScoreBonus3") != null)
			{
				m_ScoreBonus1 = int.Parse(child.GetChild("ScoreBonus3").GetElement());
			}
			m_LT_NumberOfTickets = long.Parse(child.GetChild("NumberOfTickets").GetElement());
			m_LT_TotalGames = uint.Parse(child.GetChild("TotalGames").GetElement());
			m_LT_TotalPoints = long.Parse(child.GetChild("TotalPoints").GetElement());
			m_LT_TotalCredits = uint.Parse(child.GetChild("TotalCredits").GetElement());
			m_Pts_Removed_Per_Bombs = int.Parse(child.GetChild("PtsRemovedPerBombs").GetElement());
			m_Pts_Removed_Per_Projectile = int.Parse(child.GetChild("PtsRemovedPerProjectile").GetElement());
			m_Pts_Added_Per_Coins = int.Parse(child.GetChild("PtsAddedPerCoins").GetElement());
		}
		else
		{
			ResetData();
			ResetStats();
		}
		InitAllValue();
	}

	public void ResetData()
	{
		m_CreditsPerGame = 4;
		m_GameAudioVolume = 20;
		m_AttractModeVolume = 20;
		m_PointsPerTicket = 1000;
		m_RedemptionMode = true;
		m_FixedTicketPayout = 0;
		m_MinimumTicketPayout = 0;
		m_TicketValue = 1;
		m_PaymentType = 1;
		m_RedemptionUnit = 1;
		m_ShowIntroVideo = true;
		m_CountdownPlayAgain = 15;
		m_CreditsPerPlayAgain = 4;
		m_ShortGameMode = false;
		m_Pts_Removed_Per_Bombs = -500;
		m_Pts_Removed_Per_Projectile = -500;
		m_Pts_Added_Per_Coins = 10;
		m_ScoreBonus1 = 1;
		m_ScoreBonus2 = 2;
		m_ScoreBonus3 = 3;
		if (m_IsItalieBuild)
		{
			m_RedemptionMode = false;
		}
	}

	public void ResetStats()
	{
		m_LT_NumberOfTickets = 0L;
		m_LT_TotalGames = 0u;
		m_LT_TotalPoints = 0L;
		m_LT_TotalCredits = 0u;
		m_SLR_NumberOfTickets = 0L;
		m_SLR_TotalGames = 0u;
		m_SLR_TotalPoints = 0L;
		m_SLR_TotalCredits = 0u;
		DateTime now = DateTime.Now;
		m_Date_Since_Last_Reset = now.Day;
		m_Month_Since_Last_Reset = now.Month;
		m_Year_Since_Last_Reset = now.Year;
	}

	public void ResetSinceLastResetStats()
	{
		m_SLR_NumberOfTickets = 0L;
		m_SLR_TotalGames = 0u;
		m_SLR_TotalPoints = 0L;
		m_SLR_TotalCredits = 0u;
		DateTime now = DateTime.Now;
		m_Date_Since_Last_Reset = now.Day;
		m_Month_Since_Last_Reset = now.Month;
		m_Year_Since_Last_Reset = now.Year;
	}

	public void ServerForceValue()
	{
		if (ProfileManager.Instance != null)
		{
			ProfileManager.Instance.SaveGame();
		}
		ChangeAudioVolume();
		GameManager.coinsToPtsFactor = m_Pts_Added_Per_Coins;
	}

	public void InitAllValue()
	{
		ChangeAudioVolume();
		GameManager.coinsToPtsFactor = m_Pts_Added_Per_Coins;
		if (m_IsItalieBuild)
		{
			m_RedemptionMode = false;
		}
	}

	public void ChangeAudioVolume()
	{
		if (m_InGame)
		{
			AudioListener.volume = (float)m_GameAudioVolume / 20f;
		}
		else
		{
			AudioListener.volume = (float)m_AttractModeVolume / 20f;
		}
	}

	public long GetNumTicket(bool LT)
	{
		if (LT)
		{
			return m_LT_NumberOfTickets;
		}
		return m_SLR_NumberOfTickets;
	}

	public long GetTotalPoints(bool LT)
	{
		if (LT)
		{
			return m_LT_TotalPoints;
		}
		return m_SLR_TotalPoints;
	}

	public float GetAvgTicketPerGame(bool LT)
	{
		if (GetTotalGames(LT) == 0)
		{
			return 0f;
		}
		return GetNumTicket(LT) / GetTotalGames(LT);
	}

	public float GetAvgPointsPerGame(bool LT)
	{
		if (GetTotalGames(LT) == 0)
		{
			return 0f;
		}
		return GetTotalPoints(LT) / GetTotalGames(LT);
	}

	public uint GetTotalGames(bool LT)
	{
		if (LT)
		{
			return m_LT_TotalGames;
		}
		return m_SLR_TotalGames;
	}

	public uint GetTotalCredits(bool LT)
	{
		if (LT)
		{
			return m_LT_TotalCredits;
		}
		return m_SLR_TotalCredits;
	}

	public void AddNumberOfTickets(uint tickets)
	{
		m_LT_NumberOfTickets += tickets;
		m_SLR_NumberOfTickets += tickets;
	}

	public void AddTotalGames()
	{
		m_LT_TotalGames++;
		m_SLR_TotalGames++;
	}

	public void AddTotalPoints(uint points)
	{
		m_LT_TotalPoints += points;
		m_SLR_TotalPoints += points;
	}

	public void AddTotalCredits()
	{
		m_LT_TotalCredits++;
		m_SLR_TotalCredits++;
	}
}
