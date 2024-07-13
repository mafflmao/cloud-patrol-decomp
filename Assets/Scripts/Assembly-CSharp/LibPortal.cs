using System.Runtime.InteropServices;
using UnityEngine;

public class LibPortal : MonoBehaviour
{
	private const string ImportLib = "PortalController";

	[DllImport("PortalController")]
	public static extern void pcSet_Enabled(bool enabled);

	[DllImport("PortalController")]
	public static extern bool pcGet_Enabled();

	[DllImport("PortalController")]
	public static extern void pcSet_Connect(bool connect);

	[DllImport("PortalController")]
	public static extern bool pcGet_Connect();

	[DllImport("PortalController")]
	public static extern void pcSet_AntennaEnabled(bool enabled);

	[DllImport("PortalController")]
	public static extern bool pcGet_AntennaEnabled();

	[DllImport("PortalController")]
	public static extern void pcSet_Verbose(bool verbose);

	[DllImport("PortalController")]
	public static extern bool pcGet_Verbose();

	[DllImport("PortalController")]
	public static extern void pcSet_Color(byte r, byte g, byte b);

	[DllImport("PortalController")]
	public static extern void pcGet_Color(ref byte red, ref byte green, ref byte blue);

	[DllImport("PortalController")]
	public static extern bool pcGet_Connected();

	[DllImport("PortalController")]
	public static extern bool pcGet_ConnectionState();

	[DllImport("PortalController")]
	public static extern bool pcGet_AntennaState();

	[DllImport("PortalController")]
	public static extern float pcGet_BatteryRemaining();

	[DllImport("PortalController")]
	public static extern PortalControllerBatteryStatus pcGet_BatteryStatus();

	[DllImport("PortalController")]
	public static extern void pcUpdate(float deltaTime);

	[DllImport("PortalController")]
	public static extern void pcTickle();

	[DllImport("PortalController")]
	public static extern int pcGet_PortalTagCount();

	[DllImport("PortalController")]
	public static extern PortalTagPresence ptGet_Presence(int tagIndex);

	[DllImport("PortalController")]
	public static extern bool ptGet_DoneReading(int tagIndex);

	[DllImport("PortalController")]
	public static extern bool ptGet_DoneWriting(int tagIndex);

	[DllImport("PortalController")]
	public static extern PortalTagError ptGet_Error(int tagIndex);

	[DllImport("PortalController")]
	public static extern void ptReadTagData(int tagIndex);

	[DllImport("PortalController")]
	public static extern void ptReadMagicMoment(int tagIndex);

	[DllImport("PortalController")]
	public static extern void ptReadData(int tagIndex);

	[DllImport("PortalController")]
	public static extern void ptWrite(int tagIndex);

	[DllImport("PortalController")]
	public static extern void ptReset(int tagIndex);

	[DllImport("PortalController")]
	public static extern void ptFormat(int tagIndex);

	[DllImport("PortalController")]
	public static extern void ptCorrupt(int tagIndex);

	[DllImport("PortalController")]
	public static extern bool ptGet_ReadingTagData(int tagIndex);

	[DllImport("PortalController")]
	public static extern bool ptGet_ReadingMagicMoment(int tagIndex);

	[DllImport("PortalController")]
	public static extern bool ptGet_ReadingData(int tagIndex);

	[DllImport("PortalController")]
	public static extern uint ptGet_SerialNumber(int tagIndex);

	[DllImport("PortalController")]
	public static extern ushort ptGet_ToyType(int tagIndex);

	[DllImport("PortalController")]
	public static extern ushort ptGet_SubType(int tagIndex);

	[DllImport("PortalController")]
	public static extern void ptGet_WebCode(byte[] buffer, uint bufferSize, int tagIndex);

	[DllImport("PortalController")]
	public static extern void ptSet_ToyType(ushort toyType, int tagIndex);

	[DllImport("PortalController")]
	public static extern void ptSet_SubType(ushort subType, int tagIndex);

	[DllImport("PortalController")]
	public static extern int ptGet_Experience(int tagIndex);

	[DllImport("PortalController")]
	public static extern void ptSet_Experience(int experience, int tagIndex);

	[DllImport("PortalController")]
	public static extern short ptGet_Money(int tagIndex);

	[DllImport("PortalController")]
	public static extern void ptSet_Money(short money, int tagIndex);
}
