using System;
using System.Runtime.InteropServices;
using PQ_SDK_MultiTouch;
using UnityEngine;

public class PqmtScreenGestures : FingerGestures
{
	public static PqmtScreenGestures m_instancePQMT;

	public static IntVector2 m_TouchScreenResolution;

	public static ScreenOrigin m_TouchScreenOrigin = ScreenOrigin.UpperLeft;

	public static PositiveAxisSystemDirection m_TouchScreenAxisPositiveDirections = PositiveAxisSystemDirection.XrightYdown;

	public static Action<string> OnDebugTrace = null;

	public static Action<string> OnDebugTraceWarning = null;

	public static Action<string> OnDebugTraceError = null;

	private static bool mReady = false;

	public int mMaxFingers = 6;

	public static int mStaticMaxFingers = 6;

	private static PQMTClientImport.PFuncOnReceivePointFrame cur_rf_func = OnReceivePointFrame;

	private static PQMTClientImport.PFuncOnServerBreak cur_svr_break = OnServerBreak;

	private static PQMTClientImport.PFuncOnReceiveError cur_rcv_err_func = OnReceiveError;

	private static PQMTClientImport.PFuncOnGetServerResolution cur_get_resolution = OnGetServerResolution;

	private static PQMTClientImport.PFuncOnGetDeviceInfo cur_fn_get_dev_info = OnGetDeviceInfo;

	private static readonly PqmtTouch NULL_TOUCH = new PqmtTouch(FingerPhase.None);

	public override int MaxFingers
	{
		get
		{
			return mMaxFingers;
		}
	}

	public static PqmtScreenGestures InstancePqmt
	{
		get
		{
			return m_instancePQMT;
		}
	}

	private static void DebugTrace(string msg)
	{
		if (OnDebugTrace != null)
		{
			OnDebugTrace(msg);
		}
	}

	private static void DebugTraceWarning(string msg)
	{
		if (OnDebugTraceWarning != null)
		{
			OnDebugTraceWarning(msg);
		}
	}

	private static void DebugTraceError(string msg)
	{
		if (OnDebugTraceError != null)
		{
			OnDebugTraceError(msg);
		}
	}

	protected override void Awake()
	{
		m_instancePQMT = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
		if (!mReady)
		{
			try
			{
				mStaticMaxFingers = mMaxFingers;
				InputSynch.Initialize(MaxFingers);
				InitAndConnectToServer();
				mReady = true;
			}
			catch (Exception ex)
			{
				DebugTraceError(ex.Message);
			}
		}
		base.Awake();
	}

	private void OnDestroy()
	{
		FinalizeAndDisconnectFromServer();
	}

	private void OnApplicationQuit()
	{
		FinalizeAndDisconnectFromServer();
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		InputSynch.FixPqmtInputs();
		base.Update();
	}

	public static void InitAndConnectToServer()
	{
		int num = 0;
		SetFuncsOnReceiveProc();
		string ip = "127.0.0.1";
		if ((num = PQMTClientImport.ConnectServer(ip, PQMTClientImport.PQMT_DEFAULT_CLIENT_PORT)) != 0)
		{
			throw new PqmtException("Connection to server failed.", (PQMTClientImport.EnumPQErrorType)num);
		}
		PQMTClientImport.TouchClientRequest request = default(PQMTClientImport.TouchClientRequest);
		request.type = 2;
		if ((num = PQMTClientImport.SendRequest(ref request)) != 0)
		{
			throw new PqmtException("Sending request failed.", (PQMTClientImport.EnumPQErrorType)num);
		}
		if ((num = PQMTClientImport.GetServerResolution(cur_get_resolution, IntPtr.Zero)) != 0)
		{
			throw new PqmtException("Getting server resolution failed.", (PQMTClientImport.EnumPQErrorType)num);
		}
	}

	public static void FinalizeAndDisconnectFromServer()
	{
		if (!mReady)
		{
			return;
		}
		try
		{
			PQMTClientImport.SetOnReceivePointFrame(null, IntPtr.Zero);
			PQMTClientImport.SetOnServerBreak(null, IntPtr.Zero);
			PQMTClientImport.SetOnReceiveError(null, IntPtr.Zero);
			PQMTClientImport.SetOnGetDeviceInfo(null, IntPtr.Zero);
			cur_rf_func = null;
			cur_svr_break = null;
			cur_rcv_err_func = null;
			cur_fn_get_dev_info = null;
			int num = PQMTClientImport.DisconnectServer();
			if (num != 0)
			{
				DebugTraceError("Error while disconnecting from PQMT server." + (PQMTClientImport.EnumPQErrorType)num);
			}
			mReady = false;
		}
		catch (Exception ex)
		{
			DebugTraceError(ex.Message);
		}
	}

	public void Reset()
	{
		try
		{
			PQMTClientImport.SetOnReceivePointFrame(null, IntPtr.Zero);
			PQMTClientImport.SetOnServerBreak(null, IntPtr.Zero);
			PQMTClientImport.SetOnReceiveError(null, IntPtr.Zero);
			PQMTClientImport.SetOnGetDeviceInfo(null, IntPtr.Zero);
			int num = PQMTClientImport.DisconnectServer();
			if (num != 0)
			{
				DebugTraceError("Error while disconnecting from PQMT server." + (PQMTClientImport.EnumPQErrorType)num);
			}
			mReady = false;
		}
		catch (Exception ex)
		{
			DebugTraceError(ex.Message);
		}
		try
		{
			mStaticMaxFingers = mMaxFingers;
			InputSynch.Initialize(MaxFingers);
			InitAndConnectToServer();
			mReady = true;
		}
		catch (Exception ex2)
		{
			DebugTraceError(ex2.Message);
		}
	}

	private static void SetFuncsOnReceiveProc()
	{
		PQMTClientImport.SetOnReceivePointFrame(cur_rf_func, IntPtr.Zero);
		PQMTClientImport.SetOnServerBreak(cur_svr_break, IntPtr.Zero);
		PQMTClientImport.SetOnReceiveError(cur_rcv_err_func, IntPtr.Zero);
		PQMTClientImport.SetOnGetDeviceInfo(cur_fn_get_dev_info, IntPtr.Zero);
	}

	private static void OnReceivePointFrame(int frame_id, int time_stamp, int moving_point_count, IntPtr moving_point_array, IntPtr call_back_object)
	{
		PQMTClientImport.TouchPoint[] array = new PQMTClientImport.TouchPoint[moving_point_count];
		for (int i = 0; i < moving_point_count; i++)
		{
			IntPtr ptr = (IntPtr)(moving_point_array.ToInt64() + i * Marshal.SizeOf(typeof(PQMTClientImport.TouchPoint)));
			array[i] = (PQMTClientImport.TouchPoint)Marshal.PtrToStructure(ptr, typeof(PQMTClientImport.TouchPoint));
		}
		InputSynch.ProcessTouchPoints(array);
	}

	private static void OnServerBreak(IntPtr param, IntPtr call_back_object)
	{
		FinalizeAndDisconnectFromServer();
	}

	private static void OnReceiveError(int err_code, IntPtr call_back_object)
	{
		switch ((PQMTClientImport.EnumPQErrorType)err_code)
		{
		case PQMTClientImport.EnumPQErrorType.PQMTE_RCV_INVALIDATE_DATA:
			break;
		case PQMTClientImport.EnumPQErrorType.PQMTE_SERVER_VERSION_OLD:
			break;
		case PQMTClientImport.EnumPQErrorType.PQMTE_EXCEPTION_FROM_CALLBACKFUNCTION:
			break;
		}
	}

	private static void OnGetServerResolution(int x, int y, IntPtr call_back_object)
	{
		switch (m_TouchScreenAxisPositiveDirections)
		{
		case PositiveAxisSystemDirection.XdownYright:
		case PositiveAxisSystemDirection.XdownYleft:
		case PositiveAxisSystemDirection.XupYright:
		case PositiveAxisSystemDirection.XupYleft:
			m_TouchScreenResolution.x = y;
			m_TouchScreenResolution.y = x;
			break;
		default:
			m_TouchScreenResolution.x = x;
			m_TouchScreenResolution.y = y;
			break;
		}
	}

	private static void OnGetDeviceInfo(ref PQMTClientImport.TouchDeviceInfo deviceInfo, IntPtr call_back_object)
	{
	}

	protected override FingerPhase GetPhase(Finger finger)
	{
		if (HasValidTouch(finger))
		{
			return GetTouch(finger).Phase;
		}
		return FingerPhase.None;
	}

	protected override Vector2 GetPosition(Finger finger)
	{
		return Pqmt2Unity(GetTouch(finger).Position);
	}

	private Vector2 Pqmt2Unity(IntVector2 aPt)
	{
		IntVector2 zero = IntVector2.zero;
		switch (m_TouchScreenAxisPositiveDirections)
		{
		case PositiveAxisSystemDirection.XdownYleft:
			zero.x = -aPt.y;
			zero.y = -aPt.x;
			break;
		case PositiveAxisSystemDirection.XdownYright:
			zero.x = aPt.y;
			zero.y = -aPt.x;
			break;
		case PositiveAxisSystemDirection.XleftYdown:
			zero.x = -aPt.x;
			zero.y = -aPt.y;
			break;
		case PositiveAxisSystemDirection.XleftYup:
			zero.x = -aPt.x;
			zero.y = aPt.y;
			break;
		case PositiveAxisSystemDirection.XrightYdown:
			zero.x = aPt.x;
			zero.y = -aPt.y;
			break;
		case PositiveAxisSystemDirection.XrightYup:
			zero.x = aPt.x;
			zero.y = aPt.y;
			break;
		case PositiveAxisSystemDirection.XupYleft:
			zero.x = -aPt.y;
			zero.y = aPt.x;
			break;
		case PositiveAxisSystemDirection.XupYright:
			zero.x = aPt.y;
			zero.y = aPt.x;
			break;
		}
		switch (m_TouchScreenOrigin)
		{
		case ScreenOrigin.LowerRight:
			zero.x += m_TouchScreenResolution.x;
			break;
		case ScreenOrigin.UpperLeft:
			zero.y += m_TouchScreenResolution.y;
			break;
		case ScreenOrigin.UpperRight:
			zero.x += m_TouchScreenResolution.x;
			zero.y += m_TouchScreenResolution.y;
			break;
		}
		Resolution currentResolution = Screen.currentResolution;
		Vector2 result = zero;
		result.x *= currentResolution.width / m_TouchScreenResolution.x;
		result.y *= currentResolution.height / m_TouchScreenResolution.y;
		return result;
	}

	private bool HasValidTouch(Finger finger)
	{
		if (0 <= finger.Index && finger.Index < MaxFingers)
		{
			return InputSynch.Touches[finger.Index].Phase != FingerPhase.None;
		}
		return false;
	}

	private PqmtTouch GetTouch(Finger finger)
	{
		if (HasValidTouch(finger))
		{
			if (0 <= finger.Index && finger.Index < MaxFingers)
			{
				return InputSynch.Touches[finger.Index];
			}
			return NULL_TOUCH;
		}
		return NULL_TOUCH;
	}
}
