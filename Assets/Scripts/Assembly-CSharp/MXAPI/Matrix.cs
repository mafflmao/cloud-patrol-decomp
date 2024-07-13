using System.Runtime.InteropServices;

namespace MXAPI
{
	public class Matrix
	{
		public static int GetUserId()
		{
			short port = 85;
			short num = Init_MatrixAPI();
			if (num < 0)
			{
				return -1;
			}
			short num2 = Dongle_Count(port);
			if (num2 <= 0)
			{
				return -2;
			}
			int num3 = Dongle_ReadSerNr(48316, num2, port);
			if (num3 < 0)
			{
				return -3;
			}
			num = Release_MatrixAPI();
			return num3;
		}

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short Init_MatrixAPI();

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short Release_MatrixAPI();

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern int GetVersionAPI();

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern int GetVersionDRV();

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern int GetVersionDRV_USB();

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetVersionDRV_USB")]
		public static extern void SetW95Access(short Mode);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short GetPortAdr(short Port);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short PausePrinterActivity();

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short ResumePrinterActivity();

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short Dongle_Find();

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern int Dongle_Version(short DngNr, short Port);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern int Dongle_Model(short DngNr, short Port);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short Dongle_MemSize(short DngNr, short Port);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short Dongle_Count(short Port);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short Dongle_ReadData(int UserCode, ref int Data, short Count, short DngNr, short Port);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short Dongle_ReadDataEx(int UserCode, ref int Data, short Fpos, short Count, short DngNr, short Port);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern int Dongle_ReadSerNr(int UserCode, short DngNr, short Port);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short Dongle_WriteData(int UserCode, ref int Data, short Count, short DngNr, short Port);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short Dongle_WriteDataEx(int UserCode, ref int Data, short Fpos, short Count, short DngNr, short Port);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short Dongle_WriteKey(int UserCode, ref int KeyData, short DngNr, short Port);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short Dongle_GetKeyFlag(int UserCode, short DngNr, short Port);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short Dongle_Exit();

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern int GetConfig_MatrixNet(short Category);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short LogIn_MatrixNet(int UserCode, short AppSlot, short DngNr);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short LogOut_MatrixNet(int UserCode, short AppSlot, short DngNr);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short Dongle_EncryptData(int UserCode, ref int DataBlock, short DngNr, short Port);

		[DllImport("matrix32", CallingConvention = CallingConvention.StdCall)]
		public static extern short Dongle_DecryptData(int UserCode, ref int DataBlock, short DngNr, short Port);
	}
}
