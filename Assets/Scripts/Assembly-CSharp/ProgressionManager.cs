using System.IO;
using UnityEngine;
using XmlTool;

public class ProgressionManager : BaseManager, IGameData
{
	public delegate void OnCoinInsertedEventHandler();

	public ProfileManager.ExecutionOrder m_ExecutionOrder;

	public int m_ContinueExtraTickets = 2;

	public float m_ComboInterval = 0.3f;

	private bool mOperatorUp;

	private float m_GameDuration = -1f;

	private float m_NoGameDuration = -1f;

	private bool m_SameUser;

	[HideInInspector]
	public int m_CoinsInserted;

	private static ProgressionManager m_Instance;

	public SoundEventData CoinInsertedSound;

	public float GameDuration
	{
		get
		{
			return m_GameDuration;
		}
		set
		{
			m_GameDuration = value;
		}
	}

	public float NoGameDuration
	{
		get
		{
			return m_NoGameDuration;
		}
		set
		{
			m_NoGameDuration = value;
		}
	}

	public bool SameUser
	{
		get
		{
			return m_SameUser;
		}
		set
		{
			m_SameUser = value;
		}
	}

	public static ProgressionManager Instance
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

	public static event OnCoinInsertedEventHandler OnCoinInserted;

	protected override void Awake()
	{
		base.Awake();
		if (m_Instance == null)
		{
			m_Instance = this;
			return;
		}
		Debug.Log("More than one instance of ProgressionManager.", this);
		Object.Destroy(this);
	}

	private void Start()
	{
		m_CoinsInserted = 0;
		mOperatorUp = false;
		Register();
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
		i_Writer.WriteLine("\t<SaveGame>");
		i_Writer.WriteLine("\t\t<SessionId>" + CurrentUserManager.Instance.SessionId + "</SessionId>");
		i_Writer.WriteLine("\t\t<RowVersion>" + CurrentUserManager.Instance.RowVersion + "</RowVersion>");
		i_Writer.WriteLine("\t\t<UserId>" + CurrentUserManager.Instance.UserId + "</UserId>");
		i_Writer.WriteLine("\t\t<Hashkey>" + CurrentUserManager.Instance.Hashkey + "</Hashkey>");
		i_Writer.WriteLine("\t</SaveGame>");
	}

	public void LoadGame(XmlNode i_RootNode)
	{
		XmlNode child = i_RootNode.GetChild("SaveGame");
		if (child != null)
		{
			CurrentUserManager.Instance.SessionId = child.GetChild("SessionId").GetElement();
			CurrentUserManager.Instance.RowVersion = ulong.Parse(child.GetChild("RowVersion").GetElement());
			CurrentUserManager.Instance.UserId = ulong.Parse(child.GetChild("UserId").GetElement());
			CurrentUserManager.Instance.Hashkey = child.GetChild("Hashkey").GetElement();
		}
	}

	public void ResetData()
	{
	}

	private void Update()
	{
		if (GameDuration >= 0f)
		{
			GameDuration += Time.deltaTime;
		}
		if (NoGameDuration >= 0f)
		{
			NoGameDuration += Time.deltaTime;
		}
		bool flag = false;
		KaboomMgr.CoinCountEvent aEvt;
		while (KaboomMgr.Instance.GetCoinEvent(out aEvt))
		{
			if (aEvt.Count1 != 0 || aEvt.Count2 != 0)
			{
				CoinInserted();
				flag = true;
			}
		}
		if (flag)
		{
			ushort aUn16CoinCount = 0;
			ushort aUn16CoinCount2 = 0;
			KaboomMgr.Instance.ResetCoinCount(ref aUn16CoinCount, ref aUn16CoinCount2);
		}
		KaboomMgr.KeyPadEvent aEvt2;
		if (KaboomMgr.Instance.GetKeyboardEvent(out aEvt2))
		{
			OnOperatorBtnClicked();
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			CoinInserted();
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			OnOperatorBtnClicked();
		}
	}

	public void OnOperatorBtnClicked()
	{
		if (!OperatorMenu.Instance.m_InGame)
		{
			string empty = string.Empty;
			if (!mOperatorUp)
			{
				mOperatorUp = true;
				empty = "OperatorScene";
			}
			else
			{
				mOperatorUp = false;
				empty = "Splash";
			}
			if (OperatorMenu.Instance.m_CreditsPerGame <= 0)
			{
				m_CoinsInserted = 0;
			}
			Application.LoadLevel(empty);
		}
	}

	private void CoinInserted()
	{
		if (OperatorMenu.Instance.m_CreditsPerGame > 0)
		{
			SoundEventManager.Instance.Play2D(CoinInsertedSound);
			m_CoinsInserted++;
			OperatorMenu.Instance.AddTotalCredits();
			if (ProgressionManager.OnCoinInserted != null)
			{
				ProgressionManager.OnCoinInserted();
			}
		}
	}
}
