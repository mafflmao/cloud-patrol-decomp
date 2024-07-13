using System.Collections;
using System.Collections.Generic;
using System.Text;
using MXAPI;
using UnityEngine;

public class ServerRequestManager : MonoBehaviour
{
	public class _Params
	{
		public bool m_Secured;

		public bool m_Binary;

		public bool m_EncryptBinPart;

		public bool m_DecryptBinPart;

		public string m_Action;

		public object m_Request;

		public int m_CounterError;

		public Callback<int, object> m_Callback;

		public _Params(bool i_Secured, bool i_EncryptBinPart, bool i_DecryptBinPart, string i_Action, object i_Request, Callback<int, object> i_Callback)
		{
			m_Secured = i_Secured;
			m_Binary = false;
			m_EncryptBinPart = i_EncryptBinPart;
			m_DecryptBinPart = i_DecryptBinPart;
			m_Action = i_Action;
			m_Request = i_Request;
			m_Callback = i_Callback;
			m_CounterError = 3;
		}

		public _Params(_Params i_Params)
		{
			m_Secured = i_Params.m_Secured;
			m_Binary = i_Params.m_Binary;
			m_EncryptBinPart = i_Params.m_EncryptBinPart;
			m_DecryptBinPart = i_Params.m_DecryptBinPart;
			m_Action = i_Params.m_Action;
			m_Request = i_Params.m_Request;
			m_Callback = i_Params.m_Callback;
			m_CounterError = i_Params.m_CounterError;
		}
	}

	private const int m_CountMaxError = 3;

	public int m_TimeOutTime = 15;

	public Callback m_RestratLoginClbk;

	public Callback m_ErrorInternetConnection;

	public static readonly int SUCCESS = 1;

	public static readonly int ERROR = -1;

	public static readonly uint wip = 1u;

	public static readonly uint CLT_ERROR_NO_NET_ACCESS = 65536u;

	public static readonly uint CLT_ERROR_SERVER_TIMEOUT = 131072u;

	public static readonly uint CLT_ERROR_WEB_EXCEPTION = 196608u;

	public static readonly uint CLT_ERROR_NO_ERROR_ID_FROM_SVR = 1879113728u;

	public static readonly uint CLT_ERROR_NO_ERROR_BLOCK_FROM_SVR = 1879179264u;

	public static readonly uint SVR_ERROR_NO_USER = 32u;

	public static readonly uint SVR_ERROR_SESSION = 41u;

	public static readonly uint SVR_ERROR_SESSION_WRONG = 42u;

	public static readonly uint SVR_ERROR_MISSINGUSERNAME = 59u;

	public static readonly uint SVR_ERROR_MISSINGBLOXXID = 60u;

	public static readonly uint SVR_ERROR_MISSINGFACEID = 61u;

	public static readonly uint SVR_ERROR_MISSINGFACEFRIEND = 62u;

	public static readonly uint SVR_ERROR_INVALID_REQUEST = 64u;

	public static readonly uint SVR_ERROR_INVALID_DATA = 65u;

	public static readonly uint SVR_ERROR_DEFAULT_ERROR = uint.MaxValue;

	public static readonly string m_HostName = "m.sarbakan.com";

	public static readonly string m_Folder = "skylander";

	public static readonly string m_URL = m_HostName + "/" + m_Folder + "/main.php";

	public static readonly string m_BloxxIcon = string.Empty;

	public static readonly bool s_HandleSecure = false;

	public static readonly string KEY_BIN_PART = "binary_part";

	private static ServerRequestManager m_Instance = null;

	private List<RequestSenderObject> m_SenderObjectPool = new List<RequestSenderObject>();

	private List<_Params> m_RequestNeededRelogin = new List<_Params>();

	private bool m_IsTryingToRelog;

	public static ServerRequestManager Instance
	{
		get
		{
			return m_Instance;
		}
	}

	public static string IconURL
	{
		get
		{
			return "http://" + m_HostName + "/" + m_Folder + "/icon_share.png";
		}
	}

	public string SessionId { get; set; }

	public string DeviceId
	{
		get
		{
			return Matrix.GetUserId().ToString();
		}
	}

	public bool NetIsAccessible
	{
		get 
		{
            return true;
        }
	}

	public void Awake()
	{
		if (m_Instance == null)
		{
			m_Instance = this;
		}
		SessionId = string.Empty;
		m_SenderObjectPool = new List<RequestSenderObject>();
		m_RequestNeededRelogin = new List<_Params>();
		m_IsTryingToRelog = false;
	}

	private void OnDestroy()
	{
	}

	private void Start()
	{
	}

	public void SendRequest(string i_Action, object i_Request, Callback<int, object> i_Callback, bool i_EncryptRequest = true, bool i_DecryptResponse = true)
	{
		_Params sender = new _Params(s_HandleSecure, i_EncryptRequest, i_DecryptResponse, i_Action, i_Request, i_Callback);
		SetSender(sender);
	}

	public void SendRequest(_Params i_Param)
	{
		SetSender(i_Param);
	}

	public void SendSecuredRequest(string i_Action, object i_Request, Callback<int, object> i_Callback, bool i_EncryptRequest = true, bool i_DecryptResponse = true)
	{
		_Params sender = new _Params(s_HandleSecure, i_EncryptRequest, i_DecryptResponse, i_Action, i_Request, i_Callback);
		SetSender(sender);
	}

	public void PoolSender(RequestSenderObject i_ObjectToPool)
	{
		m_SenderObjectPool.Add(i_ObjectToPool);
	}

	public void SetSender(_Params i_Params)
	{
		RequestSenderObject requestSenderObject = null;
		if (m_SenderObjectPool.Count > 0)
		{
			requestSenderObject = m_SenderObjectPool[0];
			requestSenderObject.gameObject.SetActive(true);
			m_SenderObjectPool.RemoveAt(0);
		}
		else
		{
			GameObject gameObject = new GameObject("RequestSenderObject");
			requestSenderObject = gameObject.AddComponent<RequestSenderObject>();
			requestSenderObject.transform.parent = base.transform;
		}
		requestSenderObject.StartRequest(i_Params);
		requestSenderObject = null;
	}

	public void AskForNewSession(_Params i_Params)
	{
		m_RequestNeededRelogin.Add(new _Params(i_Params));
		if (!m_IsTryingToRelog)
		{
			m_IsTryingToRelog = true;
			if (Instance.m_RestratLoginClbk != null)
			{
				Instance.m_RestratLoginClbk();
			}
		}
	}

	public void RetryAfterRelogin(int i_Result, Hashtable i_Data)
	{
		DebugScreen.Log(" ON SYNC  SERVERREQUEST START");
		InterfaceRequestManager.SyncCallback -= RetryAfterRelogin;
		if (i_Result == 1)
		{
			foreach (_Params item in m_RequestNeededRelogin)
			{
				SendRequest(item.m_Action, item.m_Request, item.m_Callback);
			}
		}
		else
		{
			foreach (_Params item2 in m_RequestNeededRelogin)
			{
				if (item2.m_Callback != null)
				{
					Hashtable hashtable = new Hashtable();
					hashtable.Add("error_id", SVR_ERROR_SESSION_WRONG);
					hashtable.Add("error_info", "Error from server");
					item2.m_Callback(i_Result, hashtable);
				}
			}
		}
		m_IsTryingToRelog = false;
		m_RequestNeededRelogin.Clear();
		DebugScreen.Log(" ON SYNC  SERVERREQUEST END");
	}

	public void RetryRequest(_Params i_Params)
	{
		i_Params.m_CounterError--;
		SendRequest(i_Params);
	}

	private void printToBoth(string s)
	{
		MonoBehaviour.print(s);
		DebugScreen.Log(s);
	}

	public string GetStringFromByteArray(byte[] i_Data)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < i_Data.Length; i++)
		{
			stringBuilder.Append(i_Data[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}
}
