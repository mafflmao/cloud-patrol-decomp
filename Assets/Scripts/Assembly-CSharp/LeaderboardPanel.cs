using UnityEngine;

public class LeaderboardPanel : MonoBehaviour
{
	public SpriteText m_RightText;

	public SpriteText m_LeftText;

	public Transform m_Highlight;

	public float[] m_PosYHighlight;

	public bool m_InitOnStart;

	private int m_CurrentPos = 10;

	private int m_CurrentScore;

	private string m_CurrentName = string.Empty;

	public bool IsInLeaderBoard
	{
		get
		{
			return m_CurrentPos < m_PosYHighlight.Length - 1;
		}
	}

	public void Awake()
	{
		if (m_InitOnStart)
		{
			m_RightText.Text = LeaderboardManager.Instance.GetRightText();
			m_LeftText.Text = LeaderboardManager.Instance.GetLeftText();
		}
	}

	public void OnEnable()
	{
		CheckCoinCount();
		ProgressionManager.OnCoinInserted += OnTokenInserted;
	}

	public void OnDisable()
	{
		ProgressionManager.OnCoinInserted -= OnTokenInserted;
	}

	public void OnDestroy()
	{
		ProgressionManager.OnCoinInserted -= OnTokenInserted;
	}

	public void OnGUI()
	{
		if (base.gameObject.activeInHierarchy)
		{
			ApplicationManager.Instance.DrawInsertCoins(true);
		}
	}

	public void UpdateLeaderboard()
	{
		m_RightText.Text = LeaderboardManager.Instance.GetRightText(m_CurrentPos, m_CurrentScore);
		m_LeftText.Text = LeaderboardManager.Instance.GetLeftText(m_CurrentPos, m_CurrentName);
	}

	public void SetupLeaderboard(int _currentscore)
	{
		m_CurrentScore = _currentscore;
		m_CurrentPos = LeaderboardManager.Instance.GetPosInLeaderboard(_currentscore);
		if (m_CurrentPos == 0)
		{
			GameManager.highScore = m_CurrentScore;
		}
		if (m_CurrentPos < m_PosYHighlight.Length + 1)
		{
			m_Highlight.GetComponent<Renderer>().enabled = true;
			Vector3 localPosition = m_Highlight.localPosition;
			localPosition.y = m_PosYHighlight[m_CurrentPos];
			m_Highlight.localPosition = localPosition;
		}
		else
		{
			m_Highlight.GetComponent<Renderer>().enabled = false;
		}
		UpdateLeaderboard();
	}

	public void UpdateName(string _name)
	{
		int num = _name.IndexOf('_');
		if (num != -1)
		{
			_name = _name.Remove(num);
		}
		m_CurrentName = _name;
		UpdateLeaderboard();
	}

	public void MakeSave()
	{
		m_CurrentName = m_CurrentName.Replace("_", string.Empty);
		if (m_CurrentPos < m_PosYHighlight.Length)
		{
			LeaderboardManager.Instance.SaveNewEntry(m_CurrentPos, m_CurrentName, m_CurrentScore);
		}
	}

	private void Update()
	{
		if (OperatorMenu.Instance.m_CreditsPerGame <= 0 && FingerGestures.InputFinger.IsDown)
		{
			MakeSave();
			ResultsController.Instance.RestartGame();
			ApplicationManager.Instance.m_CountdownObj.Activate(false);
			TransitionController.Instance.StartTransitionFromFrontEnd();
			base.gameObject.SetActive(false);
		}
	}

	private void OnTokenInserted()
	{
		if (base.gameObject.activeInHierarchy)
		{
			CheckCoinCount();
		}
	}

	private void CheckCoinCount()
	{
		if (ProgressionManager.Instance.m_CoinsInserted > 0)
		{
			ApplicationManager.Instance.m_CountdownObj.Activate(false);
		}
		if (base.gameObject.activeInHierarchy && ProgressionManager.Instance.m_CoinsInserted >= OperatorMenu.Instance.m_CreditsPerPlayAgain)
		{
			MakeSave();
			ResultsController.Instance.RestartGame();
			ApplicationManager.Instance.m_CountdownObj.Activate(false);
			ProgressionManager.Instance.m_CoinsInserted -= OperatorMenu.Instance.m_CreditsPerPlayAgain;
			TransitionController.Instance.StartTransitionFromFrontEnd();
			base.gameObject.SetActive(false);
		}
	}
}
