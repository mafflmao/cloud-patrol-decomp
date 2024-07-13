using System;
using System.Collections;
using UnityEngine;

public class SwapForceController : StateController
{
	public Renderer m_Skylander_TOP_0;

	public Renderer m_Skylander_TOP_1;

	public Renderer m_Skylander_BOT_0;

	public Renderer m_Skylander_BOT_1;

	public SpriteText m_Text_Top;

	public SpriteText m_Text_Top_Shadow;

	public SpriteText m_Text_Bot;

	public SpriteText m_Text_Bot_Shadow;

	public CharacterData m_CD_Blast_Zone;

	public CharacterData m_CD_Blast_Buckler;

	public CharacterData m_CD_Wash_Zone;

	public CharacterData m_CD_Wash_Buckler;

	public Animation m_AnimSwap;

	private bool m_Top0Active;

	private bool m_Bot0Active;

	public static string LastStateName;

	protected override IEnumerator AnimateStateIn()
	{
		yield return StartCoroutine(base.AnimateStateIn());
		if (!string.IsNullOrEmpty(LastStateName))
		{
			GetComponent<StateRoot>().backStateName = LastStateName;
		}
		if (UIBackground.Instance != null)
		{
			UIBackground.Instance.FadeTo(UIBackground.SkyTime.DAY);
		}
	}

	public void Awake()
	{
		m_Top0Active = true;
		m_Bot0Active = true;
		ApplicationManager.Instance.m_CountdownObj.Activate(true);
		Countdown.CountdownFinished += OnCountdownFinished;
	}

	private void OnDestroy()
	{
		Countdown.CountdownFinished -= OnCountdownFinished;
	}

	private void OnCountdownFinished(object sender, EventArgs e)
	{
		OnSelectSwapForce();
	}

	public void Start()
	{
		SetupScreen();
	}

	protected override void HideState()
	{
		base.HideState();
	}

	private void SetupScreen()
	{
		if (m_Top0Active)
		{
			m_Skylander_TOP_0.enabled = true;
			m_Skylander_TOP_1.enabled = false;
			m_Text_Top.Text = "WASH";
			m_Text_Top_Shadow.Text = "WASH";
		}
		else
		{
			m_Skylander_TOP_0.enabled = false;
			m_Skylander_TOP_1.enabled = true;
			m_Text_Top.Text = "BLAST";
			m_Text_Top_Shadow.Text = "BLAST";
		}
		if (m_Bot0Active)
		{
			m_Skylander_BOT_0.enabled = true;
			m_Skylander_BOT_1.enabled = false;
			m_Text_Bot.Text = "BUCKLER";
			m_Text_Bot_Shadow.Text = "BUCKLER";
		}
		else
		{
			m_Skylander_BOT_0.enabled = false;
			m_Skylander_BOT_1.enabled = true;
			m_Text_Bot.Text = "ZONE";
			m_Text_Bot_Shadow.Text = "ZONE";
		}
	}

	public void OnBtnChangeTop()
	{
		m_AnimSwap.Stop();
		m_AnimSwap.Play();
		m_Top0Active = !m_Top0Active;
		SetupScreen();
	}

	public void OnBtnChangeBot()
	{
		m_AnimSwap.Stop();
		m_AnimSwap.Play();
		m_Bot0Active = !m_Bot0Active;
		SetupScreen();
	}

	public void OnSelectSwapForce()
	{
		CharacterData characterData = m_CD_Wash_Buckler;
		if (!m_Top0Active && m_Bot0Active)
		{
			characterData = m_CD_Blast_Buckler;
		}
		else if (!m_Top0Active && !m_Bot0Active)
		{
			characterData = m_CD_Blast_Zone;
		}
		else if (m_Top0Active && !m_Bot0Active)
		{
			characterData = m_CD_Wash_Zone;
		}
		Debug.Log("*************** SELECT : " + characterData.charName);
		SwrveEventsUI.SkylanderTouched(characterData.charName);
		UIManager.instance.blockInput = true;
		StartGameSettings.Instance.activeSkylander = characterData;
		if (OperatorMenu.Instance.m_ShowIntroVideo)
		{
			MoviePlayer.Instance.PlayMovie(StartGameSettings.Instance.activeSkylander.movieIntro);
		}
		else
		{
			TransitionController.Instance.StartTransitionFromFrontEnd();
		}
		base.gameObject.SetActive(false);
		ApplicationManager.Instance.m_CountdownObj.Activate(false);
	}
}
