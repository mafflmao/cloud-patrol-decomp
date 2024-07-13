using UnityEngine;

public class CurrentUserManager : MonoBehaviour
{
	private static CurrentUserManager m_Instance;

	private string m_UserName;

	private string m_SessionId;

	private string m_Hashkey;

	private ulong m_UserId;

	private ulong m_RowVersion;

	private int m_AvailableSlots;

	public static CurrentUserManager Instance
	{
		get
		{
			return m_Instance;
		}
	}

	public int AvailableSlots
	{
		get
		{
			return m_AvailableSlots;
		}
	}

	public string UserName
	{
		get
		{
			return m_UserName;
		}
		set
		{
			m_UserName = value;
		}
	}

	public ulong UserId
	{
		get
		{
			return m_UserId;
		}
		set
		{
			m_UserId = value;
		}
	}

	public string SessionId
	{
		get
		{
			return m_SessionId;
		}
		set
		{
			m_SessionId = value;
		}
	}

	public ulong RowVersion
	{
		get
		{
			return m_RowVersion;
		}
		set
		{
			m_RowVersion = value;
		}
	}

	public string Hashkey
	{
		get
		{
			return m_Hashkey;
		}
		set
		{
			m_Hashkey = value;
		}
	}

	public int Money
	{
		get
		{
			return 0;
		}
	}

	private void Awake()
	{
		m_Instance = this;
		m_UserName = string.Empty;
		m_SessionId = string.Empty;
		m_AvailableSlots = 2;
	}

	private void Start()
	{
	}

	public void SelectUser(string i_Name)
	{
		m_UserName = i_Name;
	}
}
