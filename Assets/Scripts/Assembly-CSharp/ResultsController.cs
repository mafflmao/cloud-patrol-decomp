using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsController : StateController
{
	private const float m_MaxContinueTime = 12f;

	public int m_MaxSizeName = 12;

	public LayerMask m_LayerMask;

	public BountyController bountyController;

	public UIButton3D btnSkylander;

	public Transform skylanderRoot;

	public GameObject resultScreenPanel;

	public GameObject leaderboardPanel;

	public GameObject KeyboardPanel;

	public GameObject TicketBanner;

	public GameObject centerPanel;

	public Camera skylanderCamera;

	public GameObject activeCharacterLight;

	public SoundEventData championOutSFX;

	public SoundEventData playBtnSFX;

	public SoundEventData characterSelectSound;

	public RateAppDialog rateAppDialogPrefab;

	public SaleTag saleTag;

	public SaleDialog saleDialogPrefab;

	public SpriteText coinScoreText;

	public SpriteText coinCountText;

	public SpriteText bombScoreText;

	public SpriteText bombCountText;

	public SpriteText coinScoreTextShadow;

	public SpriteText coinCountTextShadow;

	public SpriteText bombScoreTextShadow;

	public SpriteText bombCountTextShadow;

	public SpriteText totalScoreText;

	public SpriteText keyboardTotalScoreText;

	public SpriteText ticketText;

	public SpriteText ticketText_shadow;

	public SpriteText ticketLabel;

	public SpriteText m_ContinueTime;

	public SpriteText m_InsertCoin;

	public float resultScreenTime;

	public int coinsToPtsFactor = 10;

	public PackedSprite m_IconElement;

	public GameObject backGround;

	public SpriteText m_TextName;

	public Animation introAnim;

	private bool m_ShiftActive = true;

	public SpriteText[] m_TextChangeShift;

	public Animation m_AnimationStart;

	[HideInInspector]
	public GameObject riggedSkylander;

	public LeaderboardPanel m_LeaderboardPanel;

	public List<Camera> m_GameCamera;

	public Animation m_CountdownAnim;

	public float m_TicketPerSec;

	public Animation mTicketAnimation;

	public string mTicketIncAnim;

	public string mTicketFinalIncAnim;

	public Animation m_FireWorksAnimation;

	public float m_IncAnimDelay;

	public float m_FinalIncAnimDelay;

	public SoundEventData m_ScoreUpSound;

	public SoundEventData m_ScoreEndSound;

	public SoundEventData m_PannelInSound;

	public SoundEventData m_LetterTypeSound;

	public SoundEventData m_FireWorksSound;

	public GameObject m_TicketBar;

	private float m_PtsPerSec;

	private float m_CurrentPts;

	private float m_CurrentTickets;

	private bool m_CanPlayTicketAnim;

	private int m_LastTicketCount;

	private int m_TotalScore;

	private bool m_ResultScreenActive;

	private int m_TotalScoreForLeaderboard;

	private bool m_CanPlayCountdownAnim;

	private int m_LastContinueTime;

	private static ResultsController m_Instance;

	private List<string> mProfanities = new List<string>
	{
		"ANUS", "ARSE", "ASS", "BITCH", "BUTT", "CHINK", "CLIT", "COCK", "CRAP", "CUNT",
		"DAMN", "DICK", "FART", "FUCK", "JERK", "PISS", "POO", "SHIT", "SLUT", "TIT",
		"TWAT", "BUM", "CLAM", "CUM", "CUMS", "DIKE", "DINK", "DONG", "DYKE", "FAG",
		"FART", "FUCK", "FUK", "GOOK", "HOMO", "JAP", "JERK", "JISM", "JIZ", "JIZM",
		"JIZZ", "KIKE", "KOCK", "KUM", "KUMS", "MICK", "MUFF", "PHUK", "PHUQ", "PIMP",
		"PISS", "SLAG", "SMUT", "WOP", "FUK", "FUC", "SUC", "SUK"
	};

	public GameObject awardScreenPrefab;

	public static ResultsController Instance
	{
		get
		{
			return m_Instance;
		}
	}

	private void Awake()
	{
		ProgressionManager.OnCoinInserted += OnTokenInserted;
		m_ResultScreenActive = false;
		m_Instance = this;
		m_TextName.Text = string.Empty;
		m_GameCamera.Add(ApplicationManager.Instance.m_MovieCamera);
		skylanderCamera.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		ProgressionManager.OnCoinInserted -= OnTokenInserted;
		for (int i = 0; i < m_GameCamera.Count; i++)
		{
			if (m_GameCamera[i] != null)
			{
				m_GameCamera[i].enabled = true;
			}
		}
	}

	protected override void ShowState()
	{
		base.ShowState();
		SwrveEconomy.UpdateCoinPacksFromSwrve(true);
		SwrveEconomy.UpdateGemPacksFromSwrve(false);
		if (ActivateWatcher.Instance.isForcingReboot)
		{
			StateRoot component = GetComponent<StateRoot>();
			component.canGoBack = true;
			StateManager.Instance.LoadAndActivatePreviousState();
		}
	}

	protected override void HideState()
	{
		base.HideState();
	}

	public void ShowResultScreen()
	{
		ticketLabel.GetComponent<Renderer>().enabled = true;
		StartCoroutine(AnimateStateIn());
	}

	private void DisplaySkylander()
	{
		CharacterData activeSkylander = StartGameSettings.Instance.activeSkylander;
		riggedSkylander = (GameObject)UnityEngine.Object.Instantiate(activeSkylander.GetRiggedModelPrefab());
		riggedSkylander.transform.parent = skylanderRoot.transform;
		if (activeSkylander.loadoutPosition == Vector3.zero)
		{
			riggedSkylander.transform.localScale = activeSkylander.detailsScale;
		}
		else
		{
			riggedSkylander.transform.localScale = activeSkylander.loadoutScale;
		}
		riggedSkylander.transform.localPosition = Vector3.zero;
		riggedSkylander.transform.localRotation = Quaternion.identity;
		if (StartGameSettings.Instance.activeSkylander.charName.Contains("Blast") || StartGameSettings.Instance.activeSkylander.charName.Contains("Wash"))
		{
			riggedSkylander.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
		}
		GameObjectUtils.SetLayerRecursive(base.gameObject, LayerMask.NameToLayer("ResultScreen"));
		HideAccessory[] components = riggedSkylander.GetComponents<HideAccessory>();
		if (components != null)
		{
			HideAccessory[] array = components;
			foreach (HideAccessory hideAccessory in array)
			{
				if (hideAccessory != null)
				{
					hideAccessory.enabled = false;
				}
			}
		}
		HideParticle component = riggedSkylander.GetComponent<HideParticle>();
		if (component != null)
		{
			component.enabled = false;
		}
		AnimationUtils.PlayClip(riggedSkylander.GetComponent<Animation>(), "idle");
		activeCharacterLight.SetActive(true);
		int index = 0;
		switch (activeSkylander.elementData.elementType)
		{
		case Elements.Type.Air:
			index = 0;
			break;
		case Elements.Type.Life:
			index = 1;
			break;
		case Elements.Type.Undead:
			index = 2;
			break;
		case Elements.Type.Earth:
			index = 3;
			break;
		case Elements.Type.Fire:
			index = 4;
			break;
		case Elements.Type.Water:
			index = 5;
			break;
		case Elements.Type.Magic:
			index = 6;
			break;
		case Elements.Type.Tech:
			index = 7;
			break;
		}
		m_IconElement.PlayAnim(index);
	}

	protected override IEnumerator AnimateStateIn()
	{
		MusicManager.Instance.PlayGameOverStingerAndMusic();
		float gameDuration = ProgressionManager.Instance.GameDuration;
		ProgressionManager.Instance.GameDuration = -1f;
		skylanderCamera.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		m_TextName.Text = "____";
		Debug.Log("Results Animate State in");
		m_CountdownAnim.Stop();
		int coinScore = GameManager.moneyCollectedInVoyage * GameManager.coinsToPtsFactor;
		m_TotalScore = GameManager.currentScore;
		coinCountText.Text = "x " + GameManager.moneyCollectedInVoyage;
		bombCountText.Text = "x " + GameManager.bombsHitInVoyage;
		bombScoreText.Text = GameManager.bombsPtsLostInVoyage.ToString();
		coinScoreText.Text = "+" + coinScore;
		coinCountTextShadow.Text = coinCountText.Text;
		bombCountTextShadow.Text = bombCountText.Text;
		bombScoreTextShadow.Text = bombScoreText.Text;
		coinScoreTextShadow.Text = coinScoreText.Text;
		keyboardTotalScoreText.Text = m_TotalScore.ToString();
		m_TotalScoreForLeaderboard = m_TotalScore;
		m_CurrentPts = 0f;
		m_CurrentTickets = 0f;
		m_LastTicketCount = 0;
		totalScoreText.Text = m_CurrentPts.ToString();
		int ticketCount = 0;
		m_CanPlayTicketAnim = true;
		if (OperatorMenu.Instance.m_PointsPerTicket > 0)
		{
			ticketCount = Mathf.Max(Mathf.FloorToInt(m_TotalScore / OperatorMenu.Instance.m_PointsPerTicket), 1);
		}
		if (OperatorMenu.Instance.m_FixedTicketPayout > 0)
		{
			ticketCount = OperatorMenu.Instance.m_FixedTicketPayout;
		}
		if (ticketCount < OperatorMenu.Instance.m_MinimumTicketPayout)
		{
			ticketCount = OperatorMenu.Instance.m_MinimumTicketPayout;
		}
		OperatorMenu.Instance.AddNumberOfTickets((uint)ticketCount);
		OperatorMenu.Instance.AddTotalGames();
		OperatorMenu.Instance.AddTotalPoints((uint)m_TotalScore);
		if (KaboomMgr.Instance != null && OperatorMenu.Instance.m_RedemptionMode && OperatorMenu.Instance.m_CreditsPerGame > 0)
		{
			int ticketKaboom2 = ticketCount;
			if (OperatorMenu.Instance.m_TicketValue > 1)
			{
				if (ticketCount % 2 != 0)
				{
					ticketCount++;
				}
				ticketKaboom2 = ticketCount;
				ticketKaboom2 = (int)Mathf.Round((float)ticketKaboom2 * 0.5f);
			}
			KaboomMgr.Instance.SetTicketToFeed((ushort)ticketKaboom2);
		}
		if (ProfileManager.Instance != null)
		{
			ProfileManager.Instance.SaveGame();
		}
		m_CurrentTickets = 0f;
		ticketText.Text = m_CurrentTickets.ToString();
		TicketBar.Instance.TicketEarned = ticketCount;
		if (MetricManager.Instance != null)
		{
			MetricManager.Instance.AddMetric(ApplicationManager.METRICNAMES.METRIC_GAMEEND, false, Mathf.RoundToInt(gameDuration), m_TotalScore, GameManager.redBombsHitInVoyage, GameManager.projsHitInVoyage, TicketBar.Instance.TicketEarned, ProgressionManager.Instance.SameUser);
		}
		float ticketTime = (float)ticketCount / m_TicketPerSec;
		m_PtsPerSec = (float)m_TotalScore / ticketTime;
		iTween.MoveTo(resultScreenPanel, iTween.Hash("position", new Vector3(0f, 0f, 20f), "time", 0.5f, "islocal", true));
		iTween.MoveTo(centerPanel, iTween.Hash("position", new Vector3(0f, 20f, 0f), "time", 0.5f, "islocal", true));
		HideState();
		while (StateManager.Instance.Loading)
		{
			yield return new WaitForSeconds(0.1f);
		}
		ShowState();
		for (int i = 0; i < m_GameCamera.Count; i++)
		{
			m_GameCamera[i].enabled = false;
		}
		StartTransitionToResults(null, EventArgs.Empty);
		backGround.SetActive(true);
		LevelManager.Instance.HideBackground();
		yield return new WaitForSeconds(1.25f);
		SoundEventManager.Instance.Play2D(m_ScoreUpSound);
		m_ResultScreenActive = true;
	}

	protected override IEnumerator AnimateStateOut()
	{
		m_ResultScreenActive = false;
		TransitionController.Instance.StartCloudTransition();
		iTween.MoveTo(resultScreenPanel, iTween.Hash("position", new Vector3(-20f, 0f, 20f), "time", 0.5f, "islocal", true));
		iTween.MoveTo(centerPanel, iTween.Hash("position", new Vector3(0f, -20f, 0f), "time", 0.5f, "islocal", true));
		yield return new WaitForSeconds(1.25f);
		if (riggedSkylander != null)
		{
			UnityEngine.Object.Destroy(riggedSkylander);
			riggedSkylander = null;
		}
		for (int i = 0; i < m_GameCamera.Count; i++)
		{
			m_GameCamera[i].enabled = true;
		}
		LevelManager.Instance.ShowBackground();
		backGround.SetActive(false);
		HideState();
		LoadStateIfDefined();
		InvokeHelper.InvokeSafe(ShowInGameHUD, 0.5f, this);
	}

	private void ShowInGameHUD()
	{
		ShipManager.instance.shipVisual.Revive();
	}

	private void Start()
	{
		ScreenSequenceController.SequenceComplete += StartTransitionToResults;
		if (!OperatorMenu.Instance.m_RedemptionMode)
		{
			m_TicketBar.SetActive(false);
			return;
		}
		m_TicketBar.SetActive(true);
		if (OperatorMenu.Instance.m_RedemptionUnit == 2)
		{
			ticketLabel.Text = " COUPON!";
		}
		else
		{
			ticketLabel.Text = " TICKET!";
		}
	}

	private void OnDisable()
	{
		ScreenSequenceController.SequenceComplete -= StartTransitionToResults;
	}

	private void Update()
	{
		if (m_ResultScreenActive)
		{
			UpdateTicketCount();
		}
	}

	private void OnTokenInserted()
	{
		m_CountdownAnim.Stop();
	}

	private void UpdateCoinText()
	{
	}

	private void UpdateTicketCount()
	{
		if (m_CurrentTickets >= (float)TicketBar.Instance.TicketEarned && m_CurrentPts >= (float)m_TotalScore)
		{
			return;
		}
		m_CurrentPts += m_PtsPerSec * Time.deltaTime;
		m_CurrentTickets += m_TicketPerSec * Time.deltaTime;
		totalScoreText.Text = Mathf.Min(m_TotalScore, Mathf.FloorToInt(m_CurrentPts)).ToString();
		int num = Mathf.FloorToInt(m_CurrentTickets);
		ticketText.Text = Mathf.Min(TicketBar.Instance.TicketEarned, num).ToString();
		if (!OperatorMenu.Instance.m_RedemptionMode)
		{
			m_TicketBar.SetActive(false);
		}
		else
		{
			m_TicketBar.SetActive(true);
			if (OperatorMenu.Instance.m_RedemptionUnit == 2)
			{
				ticketLabel.Text = ((num != 1) ? " COUPONS!" : " COUPON!");
			}
			else
			{
				ticketLabel.Text = ((num != 1) ? " TICKETS!" : " TICKET!");
			}
		}
		if (m_CanPlayTicketAnim)
		{
			float value = m_IncAnimDelay;
			string text = mTicketIncAnim;
			if (m_CurrentTickets > (float)(TicketBar.Instance.TicketEarned - 1))
			{
				SoundEventManager.Instance.Stop2D(m_ScoreUpSound);
				SoundEventManager.Instance.Play2D(m_ScoreEndSound);
				value = m_FinalIncAnimDelay;
				text = mTicketFinalIncAnim;
			}
			float num2 = m_CurrentTickets - (float)Mathf.FloorToInt(m_CurrentTickets);
			value = Mathf.Clamp01(value);
			if (num2 > 1f - value)
			{
				mTicketAnimation.Stop();
				mTicketAnimation.Play(text);
				m_CanPlayTicketAnim = false;
			}
		}
		if (num != m_LastTicketCount)
		{
			m_CanPlayTicketAnim = true;
		}
		m_LastTicketCount = num;
		if (m_CurrentTickets >= (float)TicketBar.Instance.TicketEarned && m_CurrentPts >= (float)m_TotalScore)
		{
			StartCoroutine(LoadNextScreen());
		}
	}

	private void BringInLeaderBoard()
	{
		ApplicationManager.Instance.m_CountdownObj.Activate(true, false, OperatorMenu.Instance.m_CountdownPlayAgain);
		leaderboardPanel.gameObject.SetActive(true);
		MoveToSide(leaderboardPanel, -28.5f);
	}

	private void OnContinue()
	{
	}

	private void StartTransitionToResults(object sender, EventArgs args)
	{
		StartCoroutine(TransitionToResults());
	}

	public IEnumerator TransitionToResults()
	{
		yield return 0;
		DisplaySkylander();
		FooterUI.Instance.visible = true;
		if (RateAppDialog.RatingConditionsHaveBeenMet())
		{
			RateAppDialog dialog = (RateAppDialog)UnityEngine.Object.Instantiate(rateAppDialogPrefab);
			StartCoroutine(dialog.Display());
		}
		else
		{
			Debug.Log("Rate App conditions not met, attempting to show sale dialog instead....");
			StartCoroutine(TryShowSaleDialog());
		}
	}

	private IEnumerator TryShowSaleDialog()
	{
		return SaleDialog.TryShowSaleDialogCoroutine(saleDialogPrefab);
	}

	public IEnumerator LoadNextScreen()
	{
		yield return new WaitForSeconds(6f);
		while (KaboomMgr.Instance.m_isGivingTicket)
		{
			yield return null;
		}
		UIManager.instance.AddCamera(skylanderCamera, m_LayerMask, float.PositiveInfinity);
		m_ResultScreenActive = false;
		iTween.MoveTo(resultScreenPanel, iTween.Hash("position", new Vector3(-50f, 0f, 20f), "time", 0.5f, "islocal", true));
		iTween.MoveTo(centerPanel, iTween.Hash("position", new Vector3(0f, -20f, 0f), "time", 0.5f, "islocal", true));
		m_LeaderboardPanel.SetupLeaderboard(m_TotalScoreForLeaderboard);
		yield return new WaitForSeconds(0.5f);
		if (m_LeaderboardPanel.IsInLeaderBoard)
		{
			ApplicationManager.Instance.m_CountdownObj.Activate(true);
			LeaderboardManager.Instance.m_MaxEntry = 10;
			MoveToSide(KeyboardPanel, -28.5f);
			Countdown.CountdownFinished += OnKeyBoardCountdownFinished;
			GameObjectUtils.SetLayerRecursive(ApplicationManager.Instance.m_CountdownObj.gameObject, LayerMask.NameToLayer("ResultScreenHeader"));
		}
		else
		{
			LeaderboardManager.Instance.m_MaxEntry = 11;
			m_LeaderboardPanel.UpdateName("YOUR SCORE");
			BringInLeaderBoard();
			Countdown.CountdownFinished += OnLeaderBoardCountdownFinished;
			m_FireWorksAnimation.Stop();
		}
	}

	private void OnKeyBoardCountdownFinished(object sender, EventArgs e)
	{
		OnEnter();
	}

	public void OnLeaderBoardCountdownFinished(object sender, EventArgs e)
	{
		m_LeaderboardPanel.MakeSave();
		UIManager.instance.blockInput = true;
		UIManager.instance.RemoveCamera(ref skylanderCamera);
		StateManager.Instance.LoadAndActivateState("Title");
		GameObjectUtils.SetLayerRecursive(ApplicationManager.Instance.m_CountdownObj.gameObject, LayerMask.NameToLayer("UI"));
		ApplicationManager.Instance.m_CountdownObj.Activate(false);
		Countdown.CountdownFinished -= OnLeaderBoardCountdownFinished;
	}

	public void RestartGame()
	{
		Countdown.CountdownFinished -= OnLeaderBoardCountdownFinished;
	}

	private void MoveToSide(GameObject i_Object, float i_XOffset)
	{
		SoundEventManager.Instance.Play2D(m_PannelInSound);
		Vector3 position = i_Object.transform.position;
		position += new Vector3(i_XOffset, 0f, 0f);
		iTween.MoveTo(i_Object, iTween.Hash("position", position, "time", 0.5f, "islocal", true));
	}

	private IEnumerator AnimateOutOnPlay()
	{
		FooterUI.Instance.visible = false;
		yield return new WaitForSeconds(0.25f);
		SoundEventManager.Instance.Play2D(championOutSFX);
		AnimationUtils.PlayClip(riggedSkylander.GetComponent<Animation>(), "jump");
		yield return new WaitForSeconds(0.5f);
		if (riggedSkylander != null)
		{
			UnityEngine.Object.Destroy(riggedSkylander);
			riggedSkylander = null;
		}
		yield return new WaitForSeconds(0.5f);
		HideState();
	}

	private void LoadStateIfDefined()
	{
	}

	private void OnPlayBtnClick()
	{
		ElementOfTheDayChanger.AllowElementOfTheDayChanges = false;
		UIManager.instance.blockInput = true;
		StartCoroutine(AnimateOutOnPlay());
		SoundEventManager.Instance.Play2D(playBtnSFX);
	}

	private void OnSkylanderBtnClick()
	{
		UIManager.instance.blockInput = true;
		SoundEventManager.Instance.Play2D(characterSelectSound);
		iTween.PunchScale(riggedSkylander, new Vector3(0.2f, 0.2f, 0.2f), 0.5f);
		Action action = delegate
		{
			SkylanderSelectController.LastStateName = "Loadout";
			StartCoroutine(AnimateStateOut());
		};
		InvokeHelper.InvokeSafe(action, 0.3f, this);
	}

	private void OnStoreBtnClick()
	{
		StartCoroutine(AnimateStateOut());
	}

	public void AddLetter(string letter)
	{
		SoundEventManager.Instance.Play2D(m_LetterTypeSound);
		int num = m_TextName.Text.IndexOf('_');
		if (num == -1)
		{
			if (m_TextName.Text.Length >= 4)
			{
				return;
			}
			m_TextName.Text += letter.ToUpper();
		}
		else
		{
			m_TextName.Text = m_TextName.Text.Remove(num, 1);
			m_TextName.Text = m_TextName.Text.Insert(num, letter.ToUpper());
		}
		if (mProfanities.Contains(m_TextName.Text))
		{
			m_TextName.Text = m_TextName.Text.Remove(m_TextName.Text.Length - 1);
		}
		else
		{
			SoundEventManager.Instance.Play2D(playBtnSFX);
		}
		m_LeaderboardPanel.UpdateName(m_TextName.Text);
	}

	public void OnErase()
	{
		int num = m_TextName.Text.IndexOf('_');
		if (num == -1)
		{
			num = 4;
		}
		num--;
		if (num >= 0)
		{
			m_TextName.Text = m_TextName.Text.Remove(num, 1);
			m_TextName.Text = m_TextName.Text.Insert(num, "_");
		}
		m_LeaderboardPanel.UpdateName(m_TextName.Text);
	}

	public void OnShift()
	{
		m_ShiftActive = !m_ShiftActive;
		UpdateTextsShift();
	}

	public void OnEnter()
	{
		Countdown.CountdownFinished -= OnKeyBoardCountdownFinished;
		Countdown.CountdownFinished += OnLeaderBoardCountdownFinished;
		MoveToSide(KeyboardPanel, -28.5f);
		InvokeHelper.InvokeSafe(BringInLeaderBoard, 0.5f, this);
		GameObjectUtils.SetLayerRecursive(ApplicationManager.Instance.m_CountdownObj.gameObject, LayerMask.NameToLayer("UI"));
		ApplicationManager.Instance.m_CountdownObj.Activate(false);
	}

	private void UpdateTextsShift()
	{
		SpriteText[] textChangeShift = m_TextChangeShift;
		foreach (SpriteText spriteText in textChangeShift)
		{
			if (m_ShiftActive)
			{
				spriteText.Text = spriteText.Text.ToUpper();
			}
			else
			{
				spriteText.Text = spriteText.Text.ToLower();
			}
		}
	}
}
