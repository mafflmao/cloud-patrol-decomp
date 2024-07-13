using System;
using System.Runtime.InteropServices;

namespace PQ_SDK_MultiTouch
{
	public class PQMTClientImport
	{
		public enum EnumPQErrorType
		{
			PQMTE_SUCCESS = 0,
			PQMTE_RCV_INVALIDATE_DATA = 822083585,
			PQMTE_SERVER_VERSION_OLD = 822083586,
			PQMTE_EXCEPTION_FROM_CALLBACKFUNCTION = 822083587
		}

		public enum EnumPQTouchPointType : ushort
		{
			TP_DOWN = 0,
			TP_MOVE = 1,
			TP_UP = 2
		}

		public enum EnumPQTouchGestureType : ushort
		{
			TG_TOUCH_START = 0,
			TG_DOWN = 1,
			TG_MOVE = 6,
			TG_UP = 7,
			TG_CLICK = 8,
			TG_DB_CLICK = 9,
			TG_BIG_DOWN = 10,
			TG_BIG_MOVE = 11,
			TG_BIG_UP = 12,
			TG_MOVE_RIGHT = 17,
			TG_MOVE_UP = 18,
			TG_MOVE_LEFT = 19,
			TG_MOVE_DOWN = 20,
			TG_SECOND_DOWN = 25,
			TG_SECOND_UP = 26,
			TG_SECOND_CLICK = 27,
			TG_SECOND_DB_CLICK = 28,
			TG_SPLIT_START = 32,
			TG_SPLIT_APART = 33,
			TG_SPLIT_CLOSE = 34,
			TG_SPLIT_END = 35,
			TG_ROTATE_START = 36,
			TG_ROTATE_ANTICLOCK = 37,
			TG_ROTATE_CLOCK = 38,
			TG_ROTATE_END = 39,
			TG_NEAR_PARREL_DOWN = 40,
			TG_NEAR_PARREL_MOVE = 45,
			TG_NEAR_PARREL_UP = 46,
			TG_NEAR_PARREL_CLICK = 47,
			TG_NEAR_PARREL_DB_CLICK = 48,
			TG_NEAR_PARREL_MOVE_RIGHT = 49,
			TG_NEAR_PARREL_MOVE_UP = 50,
			TG_NEAR_PARREL_MOVE_LEFT = 51,
			TG_NEAR_PARREL_MOVE_DOWN = 52,
			TG_MULTI_DOWN = 53,
			TG_MULTI_MOVE = 58,
			TG_MULTI_UP = 59,
			TG_MULTI_MOVE_RIGHT = 60,
			TG_MULTI_MOVE_UP = 61,
			TG_MULTI_MOVE_LEFT = 62,
			TG_MULTI_MOVE_DOWN = 63,
			TG_TOUCH_END = 128,
			TG_NO_ACTION = ushort.MaxValue
		}

		public enum EnumTouchClientRequestType : byte
		{
			RQST_RAWDATA_INSIDE_ONLY = 0,
			RQST_RAWDATA_INSIDE = 1,
			RQST_RAWDATA_ALL = 2,
			RQST_GESTURE_INSIDE = 4,
			RQST_GESTURE_ALL = 8,
			RQST_TRANSLATOR_CONFIG = 0x10
		}

		public struct TouchPoint
		{
			public ushort point_event;

			public ushort id;

			public int x;

			public int y;

			public ushort dx;

			public ushort dy;
		}

		public struct TouchClientRequest
		{
			public ushort type;

			public Guid app_id;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string param;
		}

		public struct TouchGesture
		{
			public ushort type;

			public ushort param_size;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
			public double[] param_s;
		}

		public struct TouchDeviceInfo
		{
			public int screen_width;

			public int screen_height;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string serial_number;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void PFuncOnReceivePointFrame([In] int frame_id, [In] int time_stamp, [In] int moving_point_count, [In] IntPtr moving_point_array, [In] IntPtr call_back_object);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void PFuncOnReceiveGesture([In] ref TouchGesture gesture, [In] IntPtr call_back_object);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void PFuncOnGetDeviceInfo([In] ref TouchDeviceInfo deviceInfo, [In] IntPtr call_back_object);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void PFuncOnServerBreak([In] IntPtr param, [In] IntPtr call_back_object);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void PFuncOnReceiveError([In] int error_code, [In] IntPtr call_back_object);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void PFuncOnGetServerResolution([In] int max_x, [In] int max_y, [In] IntPtr call_back_object);

		private const int MAX_TG_PARAM_SIZE = 6;

		public static int PQ_MT_CLIENT_VERSION = 259;

		public static int PQMT_DEFAULT_CLIENT_PORT = 21555;

		[DllImport("PQMTClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ConnectServer([In] string ip, int port);

		[DllImport("PQMTClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int SendRequest([In] ref TouchClientRequest request);

		[DllImport("PQMTClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int SendThreshold([In] int move_threshold);

		[DllImport("PQMTClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetServerResolution([In] PFuncOnGetServerResolution pFnCallback, [In] IntPtr call_back_object);

		[DllImport("PQMTClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int DisconnectServer();

		[DllImport("PQMTClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PFuncOnReceivePointFrame SetOnReceivePointFrame([In] PFuncOnReceivePointFrame pf_on_rcv_frame, [In] IntPtr call_back_object);

		[DllImport("PQMTClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PFuncOnReceiveGesture SetOnReceiveGesture([In] PFuncOnReceiveGesture pf_on_rcv_gesture, [In] IntPtr call_back_object);

		[DllImport("PQMTClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PFuncOnServerBreak SetOnServerBreak([In] PFuncOnServerBreak pf_on_svr_break, [In] IntPtr call_back_object);

		[DllImport("PQMTClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PFuncOnReceiveError SetOnReceiveError([In] PFuncOnReceiveError pf_on_rcv_error, [In] IntPtr call_back_object);

		[DllImport("PQMTClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int SetRawDataResolution(int max_x, int max_y);

		[DllImport("PQMTClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern PFuncOnGetDeviceInfo SetOnGetDeviceInfo([In] PFuncOnGetDeviceInfo pf_on_get_device_info, IntPtr call_back_object);

		[DllImport("PQMTClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr GetGestureName([In] ref TouchGesture tg);

		[DllImport("PQMTClient.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern Guid GetTrialAppID();
	}
}
