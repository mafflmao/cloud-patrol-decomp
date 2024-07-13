using System.Collections.Generic;
using UnityEngine;

public class DebugScreen : MonoBehaviour
{
	public SpriteText m_DebugText;

	public int m_NbDisplayLines = 40;

	private Queue<string> m_Text = new Queue<string>();

	private static DebugScreen m_Instance;

	public static DebugScreen Instance
	{
		get
		{
			return m_Instance;
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		if (m_Instance == null)
		{
			m_Instance = this;
		}
		else
		{
			Debug.LogError("More than one instance of DebugScreen in the scene");
		}
	}

	public void DebugLog(string i_Text)
	{
		if (m_Text == null)
		{
			m_Text = new Queue<string>();
		}
		m_Text.Enqueue(i_Text);
		while (m_Text.Count > m_NbDisplayLines)
		{
			m_Text.Dequeue();
		}
		string text = string.Empty;
		foreach (string item in m_Text)
		{
			text = text + "\n" + item;
		}
		if (m_DebugText == null)
		{
			m_DebugText = base.transform.Find("Content/Debug").GetComponent<SpriteText>();
		}
		m_DebugText.Text = text;
	}

	public static void Log(string i_Text)
	{
		if (m_Instance != null)
		{
			m_Instance.DebugLog(i_Text);
		}
	}
}
