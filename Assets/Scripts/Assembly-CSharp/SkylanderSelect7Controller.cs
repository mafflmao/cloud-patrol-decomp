using System;
using System.Collections;
using UnityEngine;

public class SkylanderSelect7Controller : StateController
{
	public enum ELEMENTS
	{
		AIR = 0,
		LIFE = 1,
		UNDEAD = 2,
		EARTH = 3,
		FIRE = 4,
		WATER = 5,
		MAGIC = 6,
		TECH = 7,
		SWAP_FORCE = 8
	}

	[Serializable]
	public class ElementData
	{
		public ELEMENTS m_Element;

		public SkylanderData[] m_SkylanderData;
	}

	[Serializable]
	public class SkylanderData
	{
		public string m_Name;

		public bool m_Giant;

		public bool m_Legendary;

		public Texture m_Picture;

		public CharacterData m_CharacterData;
	}

	public static ELEMENTS m_CurrentElement;

	public PackedSprite[] m_IconsElements;

	public SkylanderDisplay[] m_SkylanderDisplay;

	public ElementData[] m_ElementsDatas;

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
		ElementData[] elementsDatas = m_ElementsDatas;
		foreach (ElementData elementData in elementsDatas)
		{
			if (elementData.m_Element == m_CurrentElement)
			{
				for (int j = 0; j < elementData.m_SkylanderData.Length; j++)
				{
					SkylanderData skylanderData = elementData.m_SkylanderData[j];
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
				m_IconsElements[i].GetComponent<Renderer>().enabled = false;
			}
		}
	}

	protected override void HideState()
	{
		base.HideState();
	}
}
