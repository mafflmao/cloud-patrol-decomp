using System;
using System.Collections;
using UnityEngine;

public class ElementSelectController : StateController
{
	[Serializable]
	public class InfosPowerUp
	{
		public SkylanderSelect7Controller.ELEMENTS m_Element;

		public string m_Name;

		public string m_Description;

		public Renderer m_Icon;
	}

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(ElementSelectController), LogLevel.Debug);

	public static string LastStateName;

	public PackedSprite[] m_AllElements;

	public SoundEventData m_ClickSound;

	public GameObject m_BG_PowerUp;

	public SpriteText m_Name_PowerUp;

	public SpriteText m_Desc_PowerUp;

	public InfosPowerUp[] m_InfosPowerUps;

	public float m_TimeSeePowerUp = 3.5f;

	private string m_StateToLoad = string.Empty;

	private bool m_CanClickButton;

	private void Awake()
	{
		GameObjectUtils.SetLayerRecursive(ApplicationManager.Instance.m_CountdownObj.gameObject, LayerMask.NameToLayer("UI"));
		ApplicationManager.Instance.m_CountdownObj.Activate(true);
		Countdown.CountdownFinished += OnCountdownFinished;
	}

	private void OnDestroy()
	{
		Countdown.CountdownFinished -= OnCountdownFinished;
	}

	private void OnCountdownFinished(object sender, EventArgs e)
	{
		OnElementBtnClick("0");
	}

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
		if (StateManager.Instance.lastStateName == "SkylanderDetails")
		{
			_log.LogDebug("Returning from skylander details. Not moving scroll bar because we must have gone there from here.");
		}
		m_CanClickButton = true;
	}

	protected override void HideState()
	{
		base.HideState();
	}

	public void OnElementBtnClick(string _id)
	{
		if (m_CanClickButton)
		{
			m_CanClickButton = true;
			switch (int.Parse(_id))
			{
			}
			StartCoroutine(OnElementBtnClickRte(int.Parse(_id)));
		}
	}

	private IEnumerator OnElementBtnClickRte(int _element)
	{
		SoundEventManager.Instance.Play2D(m_ClickSound);
		UIManager.instance.blockInput = true;
		CommonAnimations.AnimateButtonElement(m_AllElements[_element].gameObject);
		yield return new WaitForSeconds(0.5f);
		UIManager.instance.blockInput = false;
		switch (_element)
		{
		case 0:
			SkylanderSelect7Controller.m_CurrentElement = SkylanderSelect7Controller.ELEMENTS.AIR;
			m_StateToLoad = "SkylanderSelectAir";
			break;
		case 1:
			SkylanderSelect7Controller.m_CurrentElement = SkylanderSelect7Controller.ELEMENTS.LIFE;
			m_StateToLoad = "SkylanderSelectLife";
			break;
		case 2:
			SkylanderSelect7Controller.m_CurrentElement = SkylanderSelect7Controller.ELEMENTS.FIRE;
			m_StateToLoad = "SkylanderSelectFire";
			break;
		case 5:
			SkylanderSelect7Controller.m_CurrentElement = SkylanderSelect7Controller.ELEMENTS.UNDEAD;
			m_StateToLoad = "SkylanderSelectUndead";
			break;
		case 6:
			SkylanderSelect7Controller.m_CurrentElement = SkylanderSelect7Controller.ELEMENTS.EARTH;
			m_StateToLoad = "SkylanderSelectEarth";
			break;
		case 8:
			SkylanderSelect8Controller.m_CurrentElement = SkylanderSelect7Controller.ELEMENTS.TECH;
			m_StateToLoad = "SkylanderSelectTech";
			break;
		case 3:
			SkylanderSelect8Controller.m_CurrentElement = SkylanderSelect7Controller.ELEMENTS.WATER;
			m_StateToLoad = "SkylanderSelectWater";
			break;
		case 7:
			SkylanderSelect9Controller.m_CurrentElement = SkylanderSelect7Controller.ELEMENTS.MAGIC;
			m_StateToLoad = "SkylanderSelectMagic";
			break;
		default:
			SkylanderSelect9Controller.m_CurrentElement = SkylanderSelect7Controller.ELEMENTS.SWAP_FORCE;
			m_StateToLoad = "SwapForce";
			break;
		}
		ShowPowerUp(_element);
	}

	public void ShowPowerUp(int _element)
	{
		StartCoroutine("WaitAndChangeState");
	}

	private IEnumerator WaitAndChangeState()
	{
		yield return new WaitForSeconds(0f);
		StateManager.Instance.LoadAndActivateState(m_StateToLoad);
	}
}
