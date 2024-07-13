using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class InterfaceRequestManager : MonoBehaviour
{
	public class CallRequest
	{
		public string m_Request;

		public Hashtable m_HashInfo;

		public List<string> m_ListValidation;

		public Callback<int, object> m_CallbackAnswer;

		public CallRequest()
		{
			m_Request = string.Empty;
			m_HashInfo = new Hashtable();
			m_ListValidation = new List<string>();
			m_CallbackAnswer = null;
		}

		public CallRequest(string i_Request, Hashtable i_HashInfo, List<string> i_ListValidation, Callback<int, object> i_Callback)
		{
			m_Request = i_Request;
			m_HashInfo = i_HashInfo;
			m_ListValidation = i_ListValidation;
			m_CallbackAnswer = i_Callback;
		}
	}

	private class SvrError
	{
		public uint Id { get; set; }

		public string Info { get; set; }

		public bool Silent { get; set; }
	}

	private bool m_SessionGet;

	private static InterfaceRequestManager m_Instance;

	private int m_Counter;

	private bool m_CounterUsed;

	private bool m_CoroutineValidation;

	private List<CallRequest> m_ListRequestValidation;

	private Callback m_CallbackSession;

	private Callback<bool> m_CallbackLogin;

	private byte[] l_idHashed;

	private byte[] l_idByte;

	private string l_idString;

	public static readonly string s_SecretHMac = "z6qop/QZFYDz89o3KrVZ5934pSmR5eC3iUjINykGFGwaOWFigwfOynb6CRqZTLYFzx0ZSX/bzXn8QvUuJkyZxQ";

	private Queue<SvrError> m_Errors;

	private object m_ErrorQueueLock = new object();

	public static InterfaceRequestManager Instance
	{
		get
		{
			return m_Instance;
		}
	}

	public bool SessionGet
	{
		get
		{
			return m_SessionGet;
		}
	}

	public static event Callback<int, Hashtable> SyncCallback
	{
		add
		{
			InterfaceRequestManager.m_CallbackSync = (Callback<int, Hashtable>)Delegate.Combine(InterfaceRequestManager.m_CallbackSync, value);
		}
		remove
		{
			InterfaceRequestManager.m_CallbackSync = (Callback<int, Hashtable>)Delegate.Remove(InterfaceRequestManager.m_CallbackSync, value);
		}
	}

	private static event Callback<int, Hashtable> m_CallbackSync;

	private void Awake()
	{
		m_Instance = this;
		m_Counter = 0;
		m_ListRequestValidation = new List<CallRequest>();
		m_Errors = new Queue<SvrError>();
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		m_Instance = null;
		m_ListRequestValidation = null;
		m_CallbackSession = null;
		m_CallbackLogin = null;
	}

	public void SvrGetSession(Callback i_Callback)
	{
		m_CallbackSession = i_Callback;
		ServerRequestManager.Instance.SendSecuredRequest("startsession", null, Callback_SvrGetSession);
	}

	private void Callback_SvrGetSession(int i_ResultCode, object i_Results)
	{
		Hashtable hashtable = i_Results as Hashtable;
		if (i_ResultCode == ServerRequestManager.SUCCESS)
		{
			string text = hashtable["session_id"].PEToString();
			if (!string.IsNullOrEmpty(text))
			{
				ServerRequestManager.Instance.SessionId = text;
				CurrentUserManager.Instance.SessionId = text;
				m_SessionGet = true;
			}
		}
		else
		{
			SvrError svrError = HandleSvrError(hashtable, true);
			Debug.Log("SvrGetSessionClbk(): FAILURE! error_id: " + svrError.Id.ToString("X8") + "; error_info: " + svrError.Info);
		}
		if (m_CallbackSession != null)
		{
			m_CallbackSession();
		}
	}

	public void Login(string i_UserName, Callback<bool> i_CallbackLogin, Callback<int, object> i_CallbackInterfaceLogin = null)
	{
		m_CallbackLogin = i_CallbackLogin;
		List<string> list = new List<string>();
		Hashtable hashtable = new Hashtable();
		hashtable.Add("username", i_UserName);
		if (!string.IsNullOrEmpty(CurrentUserManager.Instance.Hashkey))
		{
			hashtable.Add("version", 1);
			hashtable.Add("hk", CurrentUserManager.Instance.Hashkey);
			hashtable.Add("id", CurrentUserManager.Instance.UserId.ToString());
			list.Add(CurrentUserManager.Instance.UserId.ToString());
		}
		else
		{
			list.Add(string.Empty);
		}
		list.Add(CurrentUserManager.Instance.Hashkey);
		list.Add(i_UserName);
		if (i_CallbackInterfaceLogin == null)
		{
			SendRequestValidation("login", hashtable, list, Callback_Login);
		}
		else
		{
			SendRequestValidation("login", hashtable, list, i_CallbackInterfaceLogin);
		}
	}

	public void Callback_Login(int i_Result, object i_Data)
	{
		if (i_Result == ServerRequestManager.SUCCESS)
		{
			LoginResult(i_Result, i_Data);
		}
	}

	public void Callback_RetryLogin(int i_Result, object i_Data)
	{
		LoginResult(i_Result, i_Data);
	}

	private void LoginResult(int i_Result, object i_Data)
	{
		bool arg = false;
		Hashtable hashtable = i_Data as Hashtable;
		if (i_Result == ServerRequestManager.SUCCESS)
		{
			arg = true;
			if (hashtable.ContainsKey("user"))
			{
				Hashtable i_BaseInfo = hashtable["user"] as Hashtable;
				BaseLoginInformation(i_BaseInfo);
			}
		}
		else
		{
			SvrError svrError = HandleSvrError(hashtable, true);
			Debug.Log("SvrGetSessionClbk(): FAILURE! error_id: " + svrError.Id.ToString("X8") + "; error_info: " + svrError.Info);
		}
		if (m_CallbackLogin != null)
		{
			m_CallbackLogin(arg);
		}
	}

	private void BaseLoginInformation(Hashtable i_BaseInfo)
	{
		CurrentUserManager.Instance.UserName = i_BaseInfo["username"].PEToString();
		CurrentUserManager.Instance.Hashkey = i_BaseInfo["hashkey"].PEToString();
		CurrentUserManager.Instance.UserId = i_BaseInfo["id"].PEToUint();
		CurrentUserManager.Instance.RowVersion = i_BaseInfo["row_version"].PEToUint();
	}

	private void EndCounterUsed(bool i_FreeCounter)
	{
		if (i_FreeCounter)
		{
			m_Counter++;
			m_ListRequestValidation.RemoveAt(0);
		}
		m_CounterUsed = !i_FreeCounter;
	}

	private void SendRequestValidation(string i_Resquest, Hashtable i_HashInfo, List<string> i_ListValidation, Callback<int, object> i_Callback)
	{
		CallRequest item = new CallRequest(i_Resquest, i_HashInfo, i_ListValidation, i_Callback);
		m_ListRequestValidation.Add(item);
		if (!m_CoroutineValidation)
		{
			StartCoroutine("UpdateValidationRequest");
		}
	}

	private void SendRequestCompleted(string i_Resquest, Hashtable i_HashInfo, List<string> i_ListValidation, Callback<int, object> i_Callback)
	{
		Hashtable hashtable = null;
		hashtable = ((i_HashInfo == null) ? new Hashtable() : i_HashInfo);
		SetValidation(ref hashtable, i_ListValidation);
		ServerRequestManager.Instance.SendRequest(i_Resquest, hashtable, i_Callback);
	}

	private IEnumerator UpdateValidationRequest()
	{
		m_CoroutineValidation = true;
		while (m_ListRequestValidation.Count > 0)
		{
			while (m_CounterUsed)
			{
				yield return 0;
			}
			if (m_ListRequestValidation.Count > 0)
			{
				SendRequestCompleted(m_ListRequestValidation[0].m_Request, m_ListRequestValidation[0].m_HashInfo, m_ListRequestValidation[0].m_ListValidation, Callback_ValidationRequest);
			}
		}
		m_CoroutineValidation = false;
	}

	private void Callback_ValidationRequest(int i_Result, object i_Data)
	{
		if (m_ListRequestValidation[0].m_CallbackAnswer != null)
		{
			m_ListRequestValidation[0].m_CallbackAnswer(i_Result, i_Data);
		}
		EndCounterUsed(true);
	}

	private SvrError HandleSvrError(Hashtable i_Results, bool i_Silent)
	{
		lock (m_ErrorQueueLock)
		{
			SvrError svrError = new SvrError();
			svrError.Silent = i_Silent;
			if (i_Results != null)
			{
				if (i_Results.ContainsKey("error_id") && i_Results["error_id"] != null)
				{
					svrError.Id = Convert.ToUInt32(i_Results["error_id"]);
				}
				else
				{
					svrError.Id = ServerRequestManager.CLT_ERROR_NO_ERROR_ID_FROM_SVR;
				}
				if (i_Results.ContainsKey("error_info") && i_Results["error_info"] != null)
				{
					svrError.Info = i_Results["error_info"].ToString();
				}
				else
				{
					svrError.Info = "(no error info from server)";
				}
			}
			else
			{
				svrError.Id = ServerRequestManager.CLT_ERROR_NO_ERROR_BLOCK_FROM_SVR;
				svrError.Info = "(no error block from server)";
			}
			m_Errors.Enqueue(svrError);
			return svrError;
		}
	}

	private bool ShowError(Callback i_OnClose)
	{
		lock (m_ErrorQueueLock)
		{
			bool result = false;
			if (m_Errors != null && m_Errors.Count > 0)
			{
				result = true;
				SvrError svrError = m_Errors.Dequeue();
				if ((!UIManager.Exists() || svrError.Silent) && i_OnClose != null)
				{
					i_OnClose();
				}
			}
			return result;
		}
	}

	private void SetValidation(ref Hashtable i_HashInfo, List<string> i_ListValidation)
	{
		string empty = string.Empty;
		empty = ServerRequestManager.Instance.SessionId + "-";
		empty = empty + ServerRequestManager.Instance.DeviceId + "-";
		foreach (string item in i_ListValidation)
		{
			empty = empty + item + "-";
		}
		empty += m_Counter;
		Debug.Log(empty);
		i_HashInfo.Add("validation", CreateValidation(empty));
		i_HashInfo.Add("onetimecounter", m_Counter.ToString());
		m_Counter++;
		m_CounterUsed = true;
	}

	private string CreateValidation(string i_Data)
	{
		int num = 64;
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		SHA256 sHA = new SHA256Managed();
		byte[] array = uTF8Encoding.GetBytes(s_SecretHMac);
		byte[] array2 = new byte[num];
		byte[] array3 = new byte[num];
		if (array.Length > num)
		{
			array = sHA.ComputeHash(array);
		}
		if (array.Length < num)
		{
			byte[] array4 = new byte[num - array.Length];
			byte[] array5 = new byte[array4.Length + array.Length];
			for (int i = 0; i < num - array.Length; i++)
			{
				array4[i] = 0;
			}
			Buffer.BlockCopy(array, 0, array5, 0, array.Length);
			Buffer.BlockCopy(array4, 0, array5, array.Length, array4.Length);
			array = array5;
		}
		for (int j = 0; j < num; j++)
		{
			array2[j] = 92;
			array3[j] = 54;
		}
		for (int k = 0; k < num; k++)
		{
			array2[k] ^= array[k];
			array3[k] ^= array[k];
		}
		byte[] bytes = uTF8Encoding.GetBytes(i_Data);
		byte[] array6 = new byte[array3.Length + bytes.Length];
		Buffer.BlockCopy(array3, 0, array6, 0, array3.Length);
		Buffer.BlockCopy(bytes, 0, array6, array3.Length, bytes.Length);
		array6 = sHA.ComputeHash(array6);
		bytes = new byte[array2.Length + array6.Length];
		Buffer.BlockCopy(array2, 0, bytes, 0, array2.Length);
		Buffer.BlockCopy(array6, 0, bytes, array2.Length, array6.Length);
		return GetStringFromByteArray(sHA.ComputeHash(bytes));
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
