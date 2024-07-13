using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XmlTool;

public class LeaderboardManager : BaseManager, IGameData
{
	public class LeaderboardEntry
	{
		public int m_Position;

		public string m_Name;

		public int m_Score;
	}

	public ProfileManager.ExecutionOrder m_ExecutionOrder;

	public int m_MaxEntry = 11;

	public string m_DefaultName = string.Empty;

	private List<LeaderboardEntry> m_ListEntry;

	private static LeaderboardManager m_Instance;

	public static LeaderboardManager Instance
	{
		get
		{
			return m_Instance;
		}
	}

	public ProfileManager.ExecutionOrder ExecutionOrder
	{
		get
		{
			return m_ExecutionOrder;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (m_Instance == null)
		{
			m_Instance = this;
			return;
		}
		Debug.Log("More than one instance of LeaderboardManager.", this);
		Object.Destroy(this);
	}

	private void Start()
	{
		ResetData();
		Register();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
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
		i_Writer.WriteLine("\t<Leaderboard>");
		foreach (LeaderboardEntry item in m_ListEntry)
		{
			if (item.m_Position < 10)
			{
				i_Writer.WriteLine("\t\t<Entry pos=\"" + item.m_Position + "\" score=\"" + item.m_Score + "\">" + item.m_Name + "</Entry>");
			}
		}
		i_Writer.WriteLine("\t</Leaderboard>");
	}

	public void LoadGame(XmlNode i_RootNode)
	{
		m_ListEntry = new List<LeaderboardEntry>();
		XmlNode child = i_RootNode.GetChild("Leaderboard");
		if (child != null)
		{
			foreach (XmlNode child2 in child.GetChildList())
			{
				LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
				leaderboardEntry.m_Position = child2.GetAttributeAsInt("pos");
				leaderboardEntry.m_Name = child2.GetElement();
				leaderboardEntry.m_Score = child2.GetAttributeAsInt("score");
				if (leaderboardEntry.m_Position == 0)
				{
					GameManager.highScore = leaderboardEntry.m_Score;
				}
				m_ListEntry.Add(leaderboardEntry);
			}
			LeaderboardEntry leaderboardEntry2 = new LeaderboardEntry();
			leaderboardEntry2.m_Position = 10;
			leaderboardEntry2.m_Name = "YOUR SCORE";
			leaderboardEntry2.m_Score = 0;
		}
		else
		{
			ResetData();
		}
	}

	public void ResetData()
	{
		m_ListEntry = new List<LeaderboardEntry>();
		for (int i = 0; i < m_MaxEntry; i++)
		{
			LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
			leaderboardEntry.m_Position = i;
			leaderboardEntry.m_Name = m_DefaultName;
			leaderboardEntry.m_Score = 0;
			m_ListEntry.Add(leaderboardEntry);
		}
	}

	public string GetRightText()
	{
		string text = string.Empty;
		foreach (LeaderboardEntry item in m_ListEntry)
		{
			text = text + item.m_Score.ToString("###,###,##0") + "\n";
		}
		return text;
	}

	public string GetRightText(int _pos, int _score)
	{
		string text = string.Empty;
		for (int num = m_MaxEntry - 1; num >= 0; num--)
		{
			text = ((num >= _pos) ? ((num != _pos) ? (m_ListEntry[num - 1].m_Score.ToString("###,###,##0") + "\n" + text) : (Color.yellow.ToString() + _score.ToString("###,###,##0") + "\n" + Color.white.ToString() + text)) : (m_ListEntry[num].m_Score.ToString("###,###,##0") + "\n" + text));
		}
		return text;
	}

	public int GetPosInLeaderboard(int _score)
	{
		int num = 10;
		foreach (LeaderboardEntry item in m_ListEntry)
		{
			if (_score > item.m_Score && item.m_Position < num)
			{
				num = item.m_Position;
			}
		}
		return num;
	}

	public string GetLeftText()
	{
		string text = string.Empty;
		foreach (LeaderboardEntry item in m_ListEntry)
		{
			text = text + item.m_Name + "\n";
		}
		return text;
	}

	public string GetLeftText(int _pos, string _name)
	{
		string text = string.Empty;
		for (int num = m_MaxEntry - 1; num >= 0; num--)
		{
			text = ((num >= _pos) ? ((num != _pos) ? (m_ListEntry[num - 1].m_Name + "\n" + text) : (Color.yellow.ToString() + _name + " \n" + Color.white.ToString() + text)) : (m_ListEntry[num].m_Name + "\n" + text));
		}
		return text;
	}

	public void SaveNewEntry(int _pos, string _name, int _score)
	{
		if (_pos <= 9)
		{
			LeaderboardEntry leaderboardEntry;
			for (int num = m_MaxEntry - 1; num > _pos; num--)
			{
				leaderboardEntry = new LeaderboardEntry();
				leaderboardEntry.m_Position = num;
				leaderboardEntry.m_Name = m_ListEntry[num - 1].m_Name;
				leaderboardEntry.m_Score = m_ListEntry[num - 1].m_Score;
				m_ListEntry[num] = leaderboardEntry;
			}
			leaderboardEntry = new LeaderboardEntry();
			leaderboardEntry.m_Position = _pos;
			leaderboardEntry.m_Name = _name;
			leaderboardEntry.m_Score = _score;
			m_ListEntry[_pos] = leaderboardEntry;
			ProfileManager.Instance.SaveGame();
		}
	}
}
