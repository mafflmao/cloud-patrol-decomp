using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
	public enum METRICNAMES
	{
		METRIC_DEVICEINFO = 0,
		METRIC_PLAYSESSION = 1,
		METRIC_GAMEEND = 2,
		Count = 3
	}

	private static ApplicationManager m_Instance;

	public Camera m_MovieCamera;

	public Countdown m_CountdownObj;

	public GUIStyle m_InsertCoinStyle;

	public GUIStyle m_InsertCoinStyleBack;

	public Vector2 m_InsertCoinPos;

	public Vector2 m_InsertCoinPosBack;

	public static ApplicationManager Instance
	{
		get
		{
			return m_Instance;
		}
	}

	private void Awake()
	{
		if (m_Instance == null)
		{
			m_Instance = this;
		}
		else if (m_Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		Screen.SetResolution(1920, 1080, true);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	public void DrawInsertCoins(bool i_PlayAgain = false, bool i_SkylanderPubMessage = false)
	{
		GUI.depth = 0;
		if (i_SkylanderPubMessage)
		{
			GUI.Label(new Rect(m_InsertCoinPos.x, m_InsertCoinPos.y, 400f, 500f), new GUIContent("Available at retail locations"), m_InsertCoinStyle);
			GUI.Label(new Rect(m_InsertCoinPosBack.x, m_InsertCoinPosBack.y, 400f, 500f), new GUIContent("Available at retail locations"), m_InsertCoinStyleBack);
		}
		else if (OperatorMenu.Instance.m_CreditsPerGame == 0)
		{
			GUI.Label(new Rect(m_InsertCoinPos.x, m_InsertCoinPos.y, 400f, 500f), new GUIContent("FreePlay"), m_InsertCoinStyle);
			GUI.Label(new Rect(m_InsertCoinPosBack.x, m_InsertCoinPosBack.y, 400f, 500f), new GUIContent("FreePlay"), m_InsertCoinStyleBack);
		}
		else if (i_PlayAgain)
		{
			GUI.Label(new Rect(m_InsertCoinPos.x, m_InsertCoinPos.y, 400f, 600f), new GUIContent("PLAY AGAIN? " + ProgressionManager.Instance.m_CoinsInserted + "/" + OperatorMenu.Instance.m_CreditsPerPlayAgain), m_InsertCoinStyle);
			GUI.Label(new Rect(m_InsertCoinPosBack.x, m_InsertCoinPosBack.y, 400f, 600f), new GUIContent("PLAY AGAIN? " + ProgressionManager.Instance.m_CoinsInserted + "/" + OperatorMenu.Instance.m_CreditsPerPlayAgain), m_InsertCoinStyleBack);
		}
		else if (OperatorMenu.Instance.m_PaymentType == 1)
		{
			GUI.Label(new Rect(m_InsertCoinPos.x, m_InsertCoinPos.y, 400f, 600f), new GUIContent("INSERT COINS " + ProgressionManager.Instance.m_CoinsInserted + "/" + OperatorMenu.Instance.m_CreditsPerGame), m_InsertCoinStyle);
			GUI.Label(new Rect(m_InsertCoinPosBack.x, m_InsertCoinPosBack.y, 400f, 600f), new GUIContent("INSERT COINS " + ProgressionManager.Instance.m_CoinsInserted + "/" + OperatorMenu.Instance.m_CreditsPerGame), m_InsertCoinStyleBack);
		}
		else
		{
			GUI.Label(new Rect(m_InsertCoinPos.x, m_InsertCoinPos.y, 400f, 600f), new GUIContent("SWIPE CARD " + ProgressionManager.Instance.m_CoinsInserted + "/" + OperatorMenu.Instance.m_CreditsPerGame), m_InsertCoinStyle);
			GUI.Label(new Rect(m_InsertCoinPosBack.x, m_InsertCoinPosBack.y, 400f, 600f), new GUIContent("SWIPE CARD " + ProgressionManager.Instance.m_CoinsInserted + "/" + OperatorMenu.Instance.m_CreditsPerGame), m_InsertCoinStyleBack);
		}
	}
}
