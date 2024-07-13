using System;
using System.Collections;
using UnityEngine;

public class MetricManager : MonoBehaviour
{
	private const string m_ServerMetricActionName = "metric";

	private const int m_MinMetricToSend = 1;

	private const float m_MinTimeBetweenRequest = 20f;

	private float m_LastTimeUpdatedServer;

	private ArrayList m_MetricList;

	private bool[] m_OneTimeMetricFlagList = new bool[3];

	private bool[] m_ActivatedMetricFlagList = new bool[3];

	private static MetricManager m_Instance;

	private float m_LastActionTime;

	private float m_BeginActionTime;

	public static MetricManager Instance
	{
		get
		{
			return m_Instance;
		}
	}

	public void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (m_Instance == null)
		{
			m_Instance = this;
		}
		else
		{
			Debug.Log("More than one instance of MetricManager.", this);
		}
		m_MetricList = new ArrayList();
		m_LastTimeUpdatedServer = 0f;
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
	}

	public void Update()
	{
		AddedNewMetric(false);
	}

	public void ReceiveProviderInfo(string i_ProviderInfo)
	{
	}

	public void ReceiveDeviceInfo(string i_DeviceInfo)
	{
		Debug.Log("--> METRIC MANAGER i_OSInfo : " + i_DeviceInfo);
		string ao_Attribute_ = "Other";
		string[] array = i_DeviceInfo.Split(';');
		AddMetric(ApplicationManager.METRICNAMES.METRIC_DEVICEINFO, false, ao_Attribute_, array[1], SystemInfo.deviceModel);
	}

	public void AddDeviceAndProviderMetric()
	{
		AddMetric(ApplicationManager.METRICNAMES.METRIC_DEVICEINFO, false, "pc", "unity", SystemInfo.deviceModel);
		AddedNewMetric(true);
	}

	public void OnPlayerInput()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (realtimeSinceStartup - m_LastActionTime > 1800f)
		{
			AddMetric(ApplicationManager.METRICNAMES.METRIC_PLAYSESSION, false, m_LastActionTime - m_BeginActionTime);
			AddedNewMetric(true);
			m_BeginActionTime = realtimeSinceStartup;
		}
		m_LastActionTime = realtimeSinceStartup;
	}

	public void RegisterOldSessionTime(float i_Time, DateTime m_LastSessionDate)
	{
		AddMetric(ApplicationManager.METRICNAMES.METRIC_PLAYSESSION, false, i_Time, m_LastSessionDate.ToString("yyyy-MM-dd HH:mm:ss"));
		AddedNewMetric(true);
	}

	private void OnApplicationFocus(bool i_Focused)
	{
		if (i_Focused)
		{
			OnPlayerInput();
		}
	}

	public bool AddMetric(ApplicationManager.METRICNAMES ao_MetricName, bool a_OneTime)
	{
		if (!ValidateMetric(ao_MetricName))
		{
			return false;
		}
		m_OneTimeMetricFlagList[(int)ao_MetricName] = a_OneTime;
		Hashtable ao_Info = new Hashtable();
		FillCommonMetricAttributes(ref ao_Info);
		ao_Info.Add("type", (int)ao_MetricName);
		m_MetricList.Add(ao_Info);
		AddedNewMetric(false);
		return true;
	}

	public bool AddMetric(ApplicationManager.METRICNAMES ao_MetricName, bool a_OneTime, object ao_Attribute_01)
	{
		if (!ValidateMetric(ao_MetricName))
		{
			return false;
		}
		m_OneTimeMetricFlagList[(int)ao_MetricName] = a_OneTime;
		Hashtable ao_Info = new Hashtable();
		FillCommonMetricAttributes(ref ao_Info);
		ao_Info.Add("type", (int)ao_MetricName);
		ao_Info.Add("a0", ao_Attribute_01);
		m_MetricList.Add(ao_Info);
		AddedNewMetric(false);
		return true;
	}

	public bool AddMetric(ApplicationManager.METRICNAMES ao_MetricName, bool a_OneTime, object ao_Attribute_01, object ao_Attribute_02)
	{
		if (!ValidateMetric(ao_MetricName))
		{
			return false;
		}
		m_OneTimeMetricFlagList[(int)ao_MetricName] = a_OneTime;
		Hashtable ao_Info = new Hashtable();
		FillCommonMetricAttributes(ref ao_Info);
		ao_Info.Add("type", (int)ao_MetricName);
		ao_Info.Add("a0", ao_Attribute_01);
		ao_Info.Add("a1", ao_Attribute_02);
		m_MetricList.Add(ao_Info);
		AddedNewMetric(false);
		return true;
	}

	public bool AddMetric(ApplicationManager.METRICNAMES ao_MetricName, bool a_OneTime, object ao_Attribute_01, object ao_Attribute_02, object ao_Attribute_03)
	{
		if (!ValidateMetric(ao_MetricName))
		{
			return false;
		}
		m_OneTimeMetricFlagList[(int)ao_MetricName] = a_OneTime;
		Hashtable ao_Info = new Hashtable();
		FillCommonMetricAttributes(ref ao_Info);
		ao_Info.Add("type", (int)ao_MetricName);
		ao_Info.Add("a0", ao_Attribute_01);
		ao_Info.Add("a1", ao_Attribute_02);
		ao_Info.Add("a2", ao_Attribute_03);
		m_MetricList.Add(ao_Info);
		AddedNewMetric(false);
		return true;
	}

	public bool AddMetric(ApplicationManager.METRICNAMES ao_MetricName, bool a_OneTime, object ao_Attribute_01, object ao_Attribute_02, object ao_Attribute_03, object ao_Attribute_04)
	{
		if (!ValidateMetric(ao_MetricName))
		{
			return false;
		}
		m_OneTimeMetricFlagList[(int)ao_MetricName] = a_OneTime;
		Hashtable ao_Info = new Hashtable();
		FillCommonMetricAttributes(ref ao_Info);
		ao_Info.Add("type", (int)ao_MetricName);
		ao_Info.Add("a0", ao_Attribute_01);
		ao_Info.Add("a1", ao_Attribute_02);
		ao_Info.Add("a2", ao_Attribute_03);
		ao_Info.Add("a3", ao_Attribute_04);
		m_MetricList.Add(ao_Info);
		AddedNewMetric(false);
		return true;
	}

	public bool AddMetric(ApplicationManager.METRICNAMES ao_MetricName, bool a_OneTime, object ao_Attribute_01, object ao_Attribute_02, object ao_Attribute_03, object ao_Attribute_04, object ao_Attribute_05)
	{
		if (!ValidateMetric(ao_MetricName))
		{
			return false;
		}
		m_OneTimeMetricFlagList[(int)ao_MetricName] = a_OneTime;
		Hashtable ao_Info = new Hashtable();
		FillCommonMetricAttributes(ref ao_Info);
		ao_Info.Add("type", (int)ao_MetricName);
		ao_Info.Add("a0", ao_Attribute_01);
		ao_Info.Add("a1", ao_Attribute_02);
		ao_Info.Add("a2", ao_Attribute_03);
		ao_Info.Add("a3", ao_Attribute_04);
		ao_Info.Add("same", ao_Attribute_05);
		m_MetricList.Add(ao_Info);
		AddedNewMetric(false);
		return true;
	}

	public bool AddMetric(ApplicationManager.METRICNAMES ao_MetricName, bool a_OneTime, object ao_Attribute_01, object ao_Attribute_02, object ao_Attribute_03, object ao_Attribute_04, object ao_Attribute_05, object ao_Attribute_06)
	{
		if (!ValidateMetric(ao_MetricName))
		{
			return false;
		}
		m_OneTimeMetricFlagList[(int)ao_MetricName] = a_OneTime;
		Hashtable ao_Info = new Hashtable();
		FillCommonMetricAttributes(ref ao_Info);
		ao_Info.Add("type", (int)ao_MetricName);
		ao_Info.Add("a0", ao_Attribute_01);
		ao_Info.Add("a1", ao_Attribute_02);
		ao_Info.Add("a2", ao_Attribute_03);
		ao_Info.Add("a3", ao_Attribute_04);
		ao_Info.Add("a4", ao_Attribute_05);
		ao_Info.Add("same", ao_Attribute_06);
		m_MetricList.Add(ao_Info);
		AddedNewMetric(false);
		return true;
	}

	public void ForceSend()
	{
		AddedNewMetric(true);
	}

	public void FlushMetrics()
	{
		AddedNewMetric(true);
		ResetOneTimeMetrics();
	}

	public void ResetOneTimeMetrics()
	{
		for (int i = 0; i < m_ActivatedMetricFlagList.Length; i++)
		{
			m_ActivatedMetricFlagList[i] = false;
		}
	}

	private bool ValidateMetric(ApplicationManager.METRICNAMES ao_MetricID)
	{
		if (ao_MetricID >= ApplicationManager.METRICNAMES.Count)
		{
			return false;
		}
		if (m_OneTimeMetricFlagList[(int)ao_MetricID])
		{
			return false;
		}
		m_ActivatedMetricFlagList[(int)ao_MetricID] = true;
		return true;
	}

	private void FillCommonMetricAttributes(ref Hashtable ao_Info)
	{
	}

	private void AddedNewMetric(bool ab_ForceSending)
	{
		float num = Time.realtimeSinceStartup - m_LastTimeUpdatedServer;
		if (ab_ForceSending || (num >= 20f && m_MetricList.Count >= 1))
		{
			SendMetricData();
		}
	}

	private void SendMetricData()
	{
		if (m_MetricList.Count != 0 && !(ServerRequestManager.Instance == null))
		{
			ServerRequestManager.Instance.SendRequest("metric", m_MetricList, Callback_Metrics);
			m_MetricList.Clear();
			m_LastTimeUpdatedServer = Time.realtimeSinceStartup;
		}
	}

	private void Callback_Metrics(int i_Result, object i_Data)
	{
		if (i_Result != ServerRequestManager.SUCCESS)
		{
		}
	}
}
