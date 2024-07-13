using System;
using System.Runtime.InteropServices;

public class AVProWindowsMediaPlugin
{
	public enum VideoFrameFormat
	{
		RAW_BGRA32 = 0,
		YUV_422_YUY2 = 1,
		YUV_422_UYVY = 2,
		YUV_422_YVYU = 3,
		YUV_422_HDYC = 4
	}

	[DllImport("AVProWindowsMedia")]
	public static extern bool Init();

	[DllImport("AVProWindowsMedia")]
	public static extern void Deinit();

	[DllImport("AVProWindowsMedia")]
	public static extern int GetInstanceHandle();

	[DllImport("AVProWindowsMedia")]
	public static extern void FreeInstanceHandle(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern bool LoadMovie(int handle, IntPtr filename, bool loop, bool allowNativeFormat);

	[DllImport("AVProWindowsMedia")]
	public static extern bool LoadAudio(int handle, IntPtr filename, bool loop);

	[DllImport("AVProWindowsMedia")]
	public static extern int GetWidth(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern int GetHeight(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern float GetFrameRate(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern int GetFormat(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern float GetDurationSeconds(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern uint GetDurationFrames(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern bool IsOrientedTopDown(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern void Play(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern void Pause(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern void Stop(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern void SeekUnit(int handle, float position);

	[DllImport("AVProWindowsMedia")]
	public static extern void SeekSeconds(int handle, float position);

	[DllImport("AVProWindowsMedia")]
	public static extern void SeekFrames(int handle, uint position);

	[DllImport("AVProWindowsMedia")]
	public static extern float GetCurrentPositionSeconds(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern uint GetCurrentPositionFrames(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern bool IsLooping(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern float GetPlaybackRate(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern float GetAudioBalance(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern bool IsFinishedPlaying(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern void SetVolume(int handle, float volume);

	[DllImport("AVProWindowsMedia")]
	public static extern void SetLooping(int handle, bool loop);

	[DllImport("AVProWindowsMedia")]
	public static extern void SetPlaybackRate(int handle, float rate);

	[DllImport("AVProWindowsMedia")]
	public static extern void SetAudioBalance(int handle, float balance);

	[DllImport("AVProWindowsMedia")]
	public static extern bool Update(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern bool IsNextFrameReadyForGrab(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern bool UpdateTextureGL(int handle, int textureID);

	[DllImport("AVProWindowsMedia")]
	public static extern bool GetFramePixels(int handle, IntPtr data, int bufferWidth, int bufferHeight);
}
