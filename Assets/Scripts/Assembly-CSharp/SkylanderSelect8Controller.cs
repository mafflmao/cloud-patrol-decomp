using System;
using System.Collections;

public class SkylanderSelect8Controller : StateController
{
	public static SkylanderSelect7Controller.ELEMENTS m_CurrentElement;

	public PackedSprite[] m_IconsElements;

	public SkylanderDisplay[] m_SkylanderDisplay;

	public SkylanderSelect7Controller.ElementData[] m_ElementsDatas;

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
		SkylanderSelect7Controller.ElementData[] elementsDatas = m_ElementsDatas;
		foreach (SkylanderSelect7Controller.ElementData elementData in elementsDatas)
		{
			if (elementData.m_Element == m_CurrentElement)
			{
				for (int j = 0; j < elementData.m_SkylanderData.Length; j++)
				{
					SkylanderSelect7Controller.SkylanderData skylanderData = elementData.m_SkylanderData[j];
					m_SkylanderDisplay[j].InitSkylander(skylanderData.m_CharacterData.charName, skylanderData.m_Giant, skylanderData.m_Legendary, skylanderData.m_Picture, skylanderData.m_CharacterData);
				}
				break;
			}
		}
		ApplicationManager.Instance.m_CountdownObj.Activate(true);
		Countdown.CountdownFinished += OnCountdownFinished;
	}

	private void OnDestroy()
	{
		Countdown.CountdownFinished -= OnCountdownFinished;
	}

	private void OnCountdownFinished(object sender, EventArgs e)
	{
		m_SkylanderDisplay[0].OnSkylanderSelect();
	}

	public void Start()
	{
		for (int i = 0; i < m_IconsElements.Length; i++)
		{
			if (i != (int)m_CurrentElement)
			{
				m_IconsElements[i].GetComponent<UnityEngine.Renderer>().enabled = false;
			}
		}
	}

	protected override void HideState()
	{
		base.HideState();
	}
}
