using System.Runtime.InteropServices;

namespace KaboomTestDLLCSharp
{
	public class KaboomCommWrapper
	{
		public enum KABOOM_DLL_ERROR : byte
		{
			NO_ERROR = 0,
			SERIAL_COMMUNICATION_INITIALIZATION_ERROR = 1,
			SERIAL_COMMUNICATION_INITIALIZATION_ERROR_1 = 2,
			SERIAL_COMMUNICATION_INITIALIZATION_ERROR_2 = 3,
			SERIAL_COMMUNICATION_INITIALIZATION_ERROR_3 = 4,
			SERIAL_COMMUNICATION_INITIALIZATION_ERROR_ALREADY_OPEN = 5,
			SERIAL_COMMUNICATION_INITIALIZATION_ERROR_SERIAL_CREATE_FILE_FAILED = 6,
			SERIAL_COMMUNICATION_NOT_INITIALIZED = 7,
			SERIAL_COMMUNICATION_ERROR = 8,
			DLL_IO_CARD_INCOMPATIBILITY = 9
		}

		public struct KaboomKeypad
		{
			public ushort un8UpButtonPressCount;

			public ushort un8DownButtonPressCount;

			public ushort un8MenuButtonPressCount;

			public ushort un8SelectButtonPressCount;
		}

		private const string DLL_PATH = "KaboomComDll";

		[DllImport("KaboomComDll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern KABOOM_DLL_ERROR InitLib(int nSerialPort);

		[DllImport("KaboomComDll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern void CloseLib();

		[DllImport("KaboomComDll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern KABOOM_DLL_ERROR CheckComm();

		[DllImport("KaboomComDll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern KABOOM_DLL_ERROR GetKeypadCount(ref KaboomKeypad pKeypadCount);

		[DllImport("KaboomComDll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern KABOOM_DLL_ERROR GetTicketFeederTicketToFeedCount(ref ushort pun16TicketToFeedCount);

		[DllImport("KaboomComDll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern KABOOM_DLL_ERROR SetTicketFeederTicketToFeedCount(ushort un16TicketToFeedCount);

		[DllImport("KaboomComDll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern KABOOM_DLL_ERROR GetTicketFeederCount(ref ushort pun16FeederCount, ref ushort pun16IsEmpty);

		[DllImport("KaboomComDll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern KABOOM_DLL_ERROR ResetTicketFeeder();

		[DllImport("KaboomComDll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern KABOOM_DLL_ERROR ResetEmptyTicketFeeder(ref ushort pun16NumReset);

		[DllImport("KaboomComDll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern KABOOM_DLL_ERROR GetCurrentCoinCount(ref ushort pun16CoinCounter1, ref ushort pun16CoinCounter2);

		[DllImport("KaboomComDll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern KABOOM_DLL_ERROR ResetCoinCount(ref ushort pun16CoinCounter1, ref ushort pun16CoinCounter2);
	}
}
