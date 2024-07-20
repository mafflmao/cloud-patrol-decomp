using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : StateController
{
	private const float PLAY_MOVIE_DELAY = 4f;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(TitleController), LogLevel.Debug);

	public UIButton3D btnStart;

	public SoundEventData muteSfxButtonPressedSound;

	public ActivateSellDialog activateSellDialogPrefab;

	public WhatsNewDialog whatsNewDialogPrefab;

	public SoundEventData PlayClickedSound;

	public QAButton QAButtonPrefab;

	public MarqueeText marqueeText;

	public GameObject leaderBoardObj;

	public List<GameObject> skylanderTitleObj;

	public GameObject blackScreenObj;

	public GameObject skyObj;

	public int gamesBetweenActivateReminders;

	private static Dictionary<string, string> _versionData;

	private bool m_StartBtnClicked;

	public float mShowLeaderBoardTime;

	private float mLeaderBoardTime;

	private DateTime m_LastCMSCall = DateTime.MinValue;

	private int m_SecondsBetweenCalls = 300;

	private int m_GlobalDataVersion;

	private static bool m_FirstGame = true;

	private bool updatedVersionData;

	private bool? _sharedCredentialsDialogChoice;

	private bool _waitingForSharedCredentialsLogin;

	private static bool _hasAttemptedToConnectToGamecenterAutomatically = false;

	private bool m_CanPlayMovie;

	private float m_PlayMovieTime;

	private void Awake()
	{
		ProgressionManager.OnCoinInserted += OnTokenInserted;
		MoviePlayer.OnMovieEnd += HandleMoviePlayerOnMovieEnd;
		m_CanPlayMovie = true;
		m_PlayMovieTime = 4f;
		m_StartBtnClicked = false;
		MusicManager.Instance.StopMusic_Internal();
	}

	private void OnDestroy()
	{
		ProgressionManager.OnCoinInserted -= OnTokenInserted;
		MoviePlayer.OnMovieEnd -= HandleMoviePlayerOnMovieEnd;
	}

	private void Start()
	{
		_versionData = new Dictionary<string, string>();
		GetCMSConfig();
		mLeaderBoardTime = 0f;
		blackScreenObj.SetActive(false);
		OperatorMenu.Instance.m_InGame = false;
		OperatorMenu.Instance.ChangeAudioVolume();
		m_StartBtnClicked = false;
		MusicManager.Instance.PlayTitleMusic();
		if (m_FirstGame)
		{
			m_FirstGame = false;
			ProgressionManager.Instance.SameUser = false;
		}
		else
		{
			ProgressionManager.Instance.SameUser = true;
		}
		ProgressionManager.Instance.NoGameDuration = 0f;
		leaderBoardObj.SetActive(false);
		CheckStartGame();
	}

	private void ActivateTitleObj(bool i_activate)
	{
		for (int i = 0; i < skylanderTitleObj.Count; i++)
		{
			skylanderTitleObj[i].SetActive(i_activate);
		}
	}

	private void GetCMSConfig()
	{
		Debug.Log("GetConfig");
		if (DateTime.Now.Subtract(m_LastCMSCall).TotalSeconds > (double)m_SecondsBetweenCalls)
		{
			m_LastCMSCall = DateTime.Now;
			Hashtable hashtable = new Hashtable();
			hashtable.Add("credit_per_game", OperatorMenu.Instance.m_CreditsPerGame);
			hashtable.Add("game_audio_volume", OperatorMenu.Instance.m_GameAudioVolume);
			hashtable.Add("attract_mode_volume", OperatorMenu.Instance.m_AttractModeVolume);
			hashtable.Add("points_per_ticket", OperatorMenu.Instance.m_PointsPerTicket);
			hashtable.Add("redemption_mode", OperatorMenu.Instance.m_RedemptionMode);
			hashtable.Add("fixed_ticket_payout", OperatorMenu.Instance.m_FixedTicketPayout);
			hashtable.Add("minimum_ticket_payout", OperatorMenu.Instance.m_MinimumTicketPayout);
			hashtable.Add("ticket_value", OperatorMenu.Instance.m_TicketValue);
			hashtable.Add("payment_type", OperatorMenu.Instance.m_PaymentType);
			hashtable.Add("redemption_unit", OperatorMenu.Instance.m_RedemptionUnit);
			hashtable.Add("score_bonus_1", OperatorMenu.Instance.m_ScoreBonus1);
			hashtable.Add("score_bonus_2", OperatorMenu.Instance.m_ScoreBonus2);
			hashtable.Add("score_bonus_3", OperatorMenu.Instance.m_ScoreBonus3);
			Hashtable hashtable2 = new Hashtable();
			hashtable2.Add("version", m_GlobalDataVersion);
			hashtable2.Add("admin_params", hashtable);
			ServerRequestManager.Instance.SendRequest("getconfig", hashtable2, Callback_GetConfig);
		}
	}

	private void Callback_GetConfig(int i_Result, object i_Data)
	{
		if (i_Result == ServerRequestManager.SUCCESS)
		{
			Hashtable hashtable = (Hashtable)i_Data;
			m_GlobalDataVersion = hashtable["gdv"].PEToInt();
			if (hashtable.Contains("config"))
			{
				Hashtable hashtable2 = (Hashtable)hashtable["config"];
				if (hashtable2.ContainsKey("minimum_fetch_delay"))
				{
					m_SecondsBetweenCalls = hashtable2["minimum_fetch_delay"].PEToInt();
				}
				if (hashtable2.ContainsKey("bomb_penalty"))
				{
					OperatorMenu.Instance.m_Pts_Removed_Per_Bombs = hashtable2["bomb_penalty"].PEToInt();
				}
				if (hashtable2.ContainsKey("projectile_penalty"))
				{
					OperatorMenu.Instance.m_Pts_Removed_Per_Projectile = hashtable2["projectile_penalty"].PEToInt();
				}
				if (hashtable2.ContainsKey("points_per_coin"))
				{
					OperatorMenu.Instance.m_Pts_Added_Per_Coins = hashtable2["points_per_coin"].PEToInt();
				}
				if (hashtable2.ContainsKey("points_per_ticket"))
				{
					OperatorMenu.Instance.m_PointsPerTicket = hashtable2["points_per_ticket"].PEToInt();
				}
				if (hashtable2.ContainsKey("show_intro_video"))
				{
					OperatorMenu.Instance.m_ShowIntroVideo = hashtable2["show_intro_video"].PEToBool();
				}
				if (hashtable2.ContainsKey("short_game_mode"))
				{
					OperatorMenu.Instance.m_ShortGameMode = hashtable2["short_game_mode"].PEToBool();
				}
				if (hashtable2.ContainsKey("score_bonus_1"))
				{
					OperatorMenu.Instance.m_ScoreBonus1 = hashtable2["score_bonus_1"].PEToInt();
				}
				if (hashtable2.ContainsKey("score_bonus_2"))
				{
					OperatorMenu.Instance.m_ScoreBonus2 = hashtable2["score_bonus_2"].PEToInt();
				}
				if (hashtable2.ContainsKey("score_bonus_3"))
				{
					OperatorMenu.Instance.m_ScoreBonus3 = hashtable2["score_bonus_3"].PEToInt();
				}
				OperatorMenu.Instance.ServerForceValue();
			}
		}
		else
		{
			Debug.Log("-- ERROR: GetConfig failed");
		}
	}

	private void Update()
	{
		if (OperatorMenu.Instance.m_CreditsPerGame <= 0 && FingerGestures.InputFinger.IsDown)
		{
			MoviePlayer.Instance.EndMovie(true);
			leaderBoardObj.SetActive(false);
			ActivateTitleObj(false);
			ProgressionManager.Instance.m_CoinsInserted -= OperatorMenu.Instance.m_CreditsPerGame;
			OnStartBtnClick();
			blackScreenObj.SetActive(false);
			skyObj.SetActive(true);
		}
		if (m_CanPlayMovie && !m_StartBtnClicked)
		{
			m_PlayMovieTime -= Time.deltaTime;
			if (m_PlayMovieTime <= 0f)
			{
				CheckStartVideo();
			}
		}
	}

	private void CheckStartVideo()
	{
		if (ProgressionManager.Instance.m_CoinsInserted <= 0)
		{
			if (OperatorMenu.Instance.m_RedemptionMode)
			{
				MoviePlayer.Instance.PlayMovie("AttractModeComplete.mp4", false);
			}
			else
			{
				MoviePlayer.Instance.PlayMovie("FreeModeComplete.mp4", false);
			}
			ActivateTitleObj(false);
			blackScreenObj.SetActive(false);
			skyObj.SetActive(true);
			m_CanPlayMovie = false;
			m_PlayMovieTime = 0f;
			MusicManager.Instance.StopCurrentMusic();
		}
		else
		{
			ActivateTitleObj(true);
		}
	}

	private void HandleMoviePlayerOnMovieEnd(bool i_Skipped)
	{
		ActivateTitleObj(true);
		blackScreenObj.SetActive(false);
		skyObj.SetActive(true);
		m_CanPlayMovie = true;
		m_PlayMovieTime = 4f;
		MusicManager.Instance.PlayTitleMusic();
	}

	private void OnTokenInserted()
	{
		leaderBoardObj.SetActive(false);
		ActivateTitleObj(true);
		MoviePlayer.Instance.EndMovie(true);
		MusicManager.Instance.PlayTitleMusic();
		blackScreenObj.SetActive(false);
		CheckStartGame();
	}

	private void CheckStartGame()
	{
		if (!m_StartBtnClicked && OperatorMenu.Instance.m_CreditsPerGame > 0 && ProgressionManager.Instance.m_CoinsInserted >= OperatorMenu.Instance.m_CreditsPerGame)
		{
			MoviePlayer.Instance.EndMovie(true);
			leaderBoardObj.SetActive(false);
			ActivateTitleObj(false);
			blackScreenObj.SetActive(false);
			skyObj.SetActive(true);
			ProgressionManager.Instance.m_CoinsInserted -= OperatorMenu.Instance.m_CreditsPerGame;
			OnStartBtnClick();
		}
	}

	public void OnStartBtnClick()
	{
		if (!m_StartBtnClicked)
		{
			MusicManager.Instance.PlayTitleMusic();
			m_StartBtnClicked = true;
			UIManager.instance.blockInput = true;
			SoundEventManager.Instance.Play2D(PlayClickedSound);
			Action action = delegate
			{
				StateManager.Instance.LoadAndActivateState("ElementSelect");
			};
			action();
		}
	}
}
