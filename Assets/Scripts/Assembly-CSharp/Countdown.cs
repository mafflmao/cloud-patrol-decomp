using System;
using UnityEngine;

public class Countdown : MonoBehaviour
{
	public float m_Duration = 12f;

	private float m_SetTime;

	public SoundEventData m_StandardSound;

	public SoundEventData m_LastThreeSound;

	public SpriteText m_StandardText;

	public SpriteText m_LastThreeText;

	private float m_TimeLeft;

	public string m_StandardAnim;

	public string m_LastThreeAnim;

	public MeshRenderer[] m_Renderers;

	public bool m_Visible;

	private int m_LastTime;

	public static event EventHandler CountdownFinished;

	private void Awake()
	{
		m_Renderers = GetComponentsInChildren<MeshRenderer>();
	}

	public void Activate(bool iActivate, bool iVisible = true, float time = 0f)
	{
		m_SetTime = time;
		base.gameObject.SetActive(iActivate);
		m_Visible = iVisible;
		if (!iVisible)
		{
			for (int i = 0; i < m_Renderers.Length; i++)
			{
				m_Renderers[i].enabled = false;
			}
		}
	}

	private void OnEnable()
	{
		if (m_SetTime != 0f)
		{
			m_TimeLeft = m_SetTime;
		}
		else
		{
			m_TimeLeft = m_Duration;
		}
	}

	private void Update()
	{
		m_TimeLeft -= Time.deltaTime;
		if (m_TimeLeft < 0f)
		{
			OnCountdownFinished();
		}
		else
		{
			if (!m_Visible)
			{
				return;
			}
			int num = Mathf.CeilToInt(m_TimeLeft);
			if (num != m_LastTime)
			{
				SpriteText spriteText = m_StandardText;
				string text = m_StandardAnim;
				if (num <= 3)
				{
					spriteText = m_LastThreeText;
					text = m_LastThreeAnim;
					SoundEventManager.Instance.Play2D(m_LastThreeSound);
				}
				base.GetComponent<Animation>().Play(text);
				spriteText.Text = num.ToString();
			}
			m_LastTime = num;
		}
	}

	private void OnCountdownFinished()
	{
		if (Countdown.CountdownFinished != null)
		{
			Countdown.CountdownFinished(this, new EventArgs());
		}
	}
}
