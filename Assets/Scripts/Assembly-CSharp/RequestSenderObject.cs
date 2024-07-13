using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public class RequestSenderObject : MonoBehaviour
{
	private ServerRequestManager._Params m_Request;

	private WWW m_WebRequest;

	private static int m_IdErrorRetryStart = 1000000;

	public void StartRequest(ServerRequestManager._Params i_Request)
	{
		Debug.Log("Request : " + i_Request.m_Action, this);
		m_Request = i_Request;
		StartCoroutine("SendRequest");
	}

	private void PoolMe()
	{
		StopCoroutine("SendRequest");
		StopCoroutine("TimeOut");
		Debug.Log("Released : " + m_Request.m_Action, this);
		m_Request = null;
		if (m_WebRequest != null)
		{
			m_WebRequest.Dispose();
			m_WebRequest = null;
		}
		ServerRequestManager.Instance.PoolSender(this);
	}


	private byte[] Int32ToByteArray(int i_Int32)
	{
		byte[] bytes = BitConverter.GetBytes(i_Int32);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(bytes);
		}
		return bytes;
	}

	private int ByteArrayToInt32(byte[] i_Bytes)
	{
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(i_Bytes);
		}
		int result = BitConverter.ToInt32(i_Bytes, 0);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(i_Bytes);
		}
		return result;
	}

	private IEnumerator TimeOut()
	{
		yield return new WaitForSeconds(ServerRequestManager.Instance.m_TimeOutTime);
		Hashtable oData = new Hashtable();
		oData["error_id"] = ServerRequestManager.CLT_ERROR_SERVER_TIMEOUT;
		oData.Add("error_info", "Server timeout");
		ErrorNeedInternet(oData);
		PoolMe();
	}

	private void ErrorAction(Hashtable i_Error)
	{
		if (m_Request.m_CounterError > 0)
		{
			ServerRequestManager.Instance.RetryRequest(m_Request);
		}
		else
		{
			ErrorProcess(i_Error);
		}
	}

	private void ErrorActionRetrySession(Hashtable i_Error)
	{
		if (m_Request.m_CounterError > 0)
		{
			ServerRequestManager.Instance.AskForNewSession(m_Request);
		}
		else
		{
			ErrorProcess(i_Error);
		}
	}

	private void ErrorNeedInternet(Hashtable i_Error)
	{
		ErrorProcess(i_Error);
		if (ServerRequestManager.Instance.m_ErrorInternetConnection != null)
		{
			ServerRequestManager.Instance.m_ErrorInternetConnection();
		}
	}

	private void ErrorProcess(Hashtable i_Error)
	{
		if (m_Request.m_Callback != null)
		{
			try
			{
				m_Request.m_Callback(-1, i_Error);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Exception during callback: {0}: {1}", ex, ex.Message));
				throw ex;
			}
		}
	}
}
