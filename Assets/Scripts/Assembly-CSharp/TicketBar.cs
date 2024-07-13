using System.Collections.Generic;
using UnityEngine;

public class TicketBar : MonoBehaviour
{
	public UIProgressBar m_Visual;

	public List<RoomTicket> m_RoomTicketData;

	public Transform m_BarStart;

	public Transform m_BarEnd;

	public Transform m_BarFrame;

	public GameObject m_TicketSpliter;

	public GameObject m_TicketWinFX;

	private MeshRenderer[] m_Render;

	private int m_MaxRoom;

	private float m_CurRoom;

	private float m_RealRoomValue;

	private int m_TicketEarned;

	private int m_CurRoomTicketIndex;

	private List<TicketSpliter> spliterScripts = new List<TicketSpliter>();

	private static TicketBar m_Instance;

	public static TicketBar Instance
	{
		get
		{
			return m_Instance;
		}
	}

	public float RealRoom
	{
		get
		{
			return m_RealRoomValue;
		}
		set
		{
			m_RealRoomValue = value;
			m_RealRoomValue = Mathf.Clamp(m_RealRoomValue, 0f, m_MaxRoom);
		}
	}

	public int TicketEarned
	{
		get
		{
			return m_TicketEarned;
		}
		set
		{
			m_TicketEarned = value;
		}
	}

	private void Show()
	{
		for (int i = 0; i < m_Render.Length; i++)
		{
			if (!(m_Render[i] == null))
			{
				m_Render[i].enabled = true;
			}
		}
		for (int j = 0; j < spliterScripts.Count; j++)
		{
			spliterScripts[j].gameObject.SetActive(true);
		}
	}

	private void Hide()
	{
		for (int i = 0; i < m_Render.Length; i++)
		{
			if (!(m_Render[i] == null))
			{
				m_Render[i].enabled = false;
			}
		}
		for (int j = 0; j < spliterScripts.Count; j++)
		{
			spliterScripts[j].gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		LevelManager.RoomClear += HandleLevelManagerRoomClear;
	}

	private void OnDisable()
	{
		LevelManager.RoomClear -= HandleLevelManagerRoomClear;
	}

	private void HandleLevelManagerRoomClear(object sender, LevelManager.RoomClearEventArgs e)
	{
		if (LevelManager.Instance.FirstRoomPassed)
		{
			RealRoom += 1f;
			if (RealRoom >= (float)m_RoomTicketData[m_CurRoomTicketIndex].m_RoomCount)
			{
				PlayTicketWinAnim();
				m_CurRoomTicketIndex++;
			}
		}
	}

	private void PlayTicketWinAnim()
	{
	}

	private void Awake()
	{
		m_Instance = this;
		m_CurRoom = 0f;
		m_MaxRoom = 25;
	}

	private void Start()
	{
		m_Render = GetComponentsInChildren<MeshRenderer>();
		if (m_RoomTicketData.Count == 0)
		{
			Debug.LogError("m_MaxHealth in HealthBar Object is 0. This will lead to a division by 0. HealthBar is going to be destroy to avoid crash.");
			Object.Destroy(base.gameObject);
			return;
		}
		m_MaxRoom = m_RoomTicketData[m_RoomTicketData.Count - 1].m_RoomCount;
		float num = Mathf.Abs(m_BarStart.localPosition.x - m_BarEnd.localPosition.x);
		float x = m_BarStart.position.x;
		for (int i = 0; i < m_RoomTicketData.Count; i++)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(m_TicketSpliter);
			gameObject.transform.parent = m_BarFrame;
			float num2 = (float)m_RoomTicketData[i].m_RoomCount / (float)m_MaxRoom * num;
			float x2 = m_BarStart.localPosition.x + num2;
			gameObject.transform.localPosition = new Vector3(x2, 0f, -10f);
			float num3 = Mathf.Abs(gameObject.transform.position.x - x) / 3.5f;
			TicketSpliter component = gameObject.GetComponent<TicketSpliter>();
			component.SetRoomTicket(m_RoomTicketData[i]);
			component.SetLvlPos(x + num3);
			spliterScripts.Add(component);
			x = gameObject.transform.position.x;
		}
		m_CurRoomTicketIndex = 0;
		Hide();
	}

	private void Update()
	{
		m_CurRoom = Mathf.Lerp(m_CurRoom, RealRoom, 0.15f);
		m_Visual.Value = m_CurRoom / (float)m_MaxRoom;
	}
}
