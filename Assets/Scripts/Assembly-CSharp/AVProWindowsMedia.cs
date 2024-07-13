using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AVProWindowsMedia : IDisposable
{
	private int _movieHandle = -1;

	private AVProWindowsMediaFormatConverter _formatConverter;

	private float _volume = 1f;

	public string Filename { get; private set; }

	public int Width { get; private set; }

	public int Height { get; private set; }

	public float AspectRatio
	{
		get
		{
			return (float)Width / (float)Height;
		}
	}

	public float FrameRate { get; private set; }

	public float DurationSeconds { get; private set; }

	public uint DurationFrames { get; private set; }

	public bool IsPlaying { get; private set; }

	public bool Loop
	{
		get
		{
			return AVProWindowsMediaPlugin.IsLooping(_movieHandle);
		}
		set
		{
			AVProWindowsMediaPlugin.SetLooping(_movieHandle, value);
		}
	}

	public float Volume
	{
		get
		{
			return _volume;
		}
		set
		{
			_volume = value;
			AVProWindowsMediaPlugin.SetVolume(_movieHandle, _volume);
		}
	}

	public float PlaybackRate
	{
		get
		{
			return AVProWindowsMediaPlugin.GetPlaybackRate(_movieHandle);
		}
		set
		{
			AVProWindowsMediaPlugin.SetPlaybackRate(_movieHandle, value);
		}
	}

	public float PositionSeconds
	{
		get
		{
			return AVProWindowsMediaPlugin.GetCurrentPositionSeconds(_movieHandle);
		}
		set
		{
			AVProWindowsMediaPlugin.SeekSeconds(_movieHandle, value);
		}
	}

	public uint PositionFrames
	{
		get
		{
			return AVProWindowsMediaPlugin.GetCurrentPositionFrames(_movieHandle);
		}
		set
		{
			AVProWindowsMediaPlugin.SeekFrames(_movieHandle, value);
		}
	}

	public float AudioBalance
	{
		get
		{
			return AVProWindowsMediaPlugin.GetAudioBalance(_movieHandle);
		}
		set
		{
			AVProWindowsMediaPlugin.SetAudioBalance(_movieHandle, value);
		}
	}

	public Texture OutputTexture
	{
		get
		{
			if (_formatConverter != null && _formatConverter.ValidPicture)
			{
				return _formatConverter.OutputTexture;
			}
			return null;
		}
	}

	public bool StartVideo(string filename, bool loop, bool allowNativeFormat, bool useBT709)
	{
		Filename = filename;
		if (!string.IsNullOrEmpty(Filename))
		{
			if (_movieHandle < 0)
			{
				_movieHandle = AVProWindowsMediaPlugin.GetInstanceHandle();
			}
			if (_formatConverter == null)
			{
				_formatConverter = new AVProWindowsMediaFormatConverter();
			}
			IntPtr intPtr = Marshal.StringToHGlobalUni(Filename);
			if (AVProWindowsMediaPlugin.LoadMovie(_movieHandle, intPtr, loop, allowNativeFormat))
			{
				Volume = _volume;
				Width = AVProWindowsMediaPlugin.GetWidth(_movieHandle);
				Height = AVProWindowsMediaPlugin.GetHeight(_movieHandle);
				FrameRate = AVProWindowsMediaPlugin.GetFrameRate(_movieHandle);
				DurationSeconds = AVProWindowsMediaPlugin.GetDurationSeconds(_movieHandle);
				DurationFrames = AVProWindowsMediaPlugin.GetDurationFrames(_movieHandle);
				AVProWindowsMediaPlugin.VideoFrameFormat format = (AVProWindowsMediaPlugin.VideoFrameFormat)AVProWindowsMediaPlugin.GetFormat(_movieHandle);
				if (Width < 0 || Width > 4096 || Height < 0 || Height > 4096)
				{
					Debug.LogWarning("[AVProWindowsMedia] invalid width or height");
					int width = (Height = 0);
					Width = width;
					if (_formatConverter != null)
					{
						_formatConverter.Dispose();
						_formatConverter = null;
					}
				}
				else
				{
					bool flipY = AVProWindowsMediaPlugin.IsOrientedTopDown(_movieHandle);
					if (!_formatConverter.Build(_movieHandle, Width, Height, format, useBT709, false, flipY))
					{
						Debug.LogWarning("[AVProWindowsMedia] unable to convert video format");
						int width = (Height = 0);
						Width = width;
						if (_formatConverter != null)
						{
							_formatConverter.Dispose();
							_formatConverter = null;
						}
					}
					else
					{
						PreRoll();
					}
				}
			}
			else
			{
				Debug.LogWarning("[AVProWindowsMedia] Movie failed to load");
				Close();
			}
			Marshal.FreeHGlobal(intPtr);
		}
		else
		{
			Debug.LogWarning("[AVProWindowsMedia] No movie file specified");
			Close();
		}
		return _movieHandle >= 0;
	}

	public bool StartAudio(string filename, bool loop)
	{
		Filename = filename;
		int width = (Height = 0);
		Width = width;
		if (!string.IsNullOrEmpty(Filename))
		{
			if (_movieHandle < 0)
			{
				_movieHandle = AVProWindowsMediaPlugin.GetInstanceHandle();
			}
			if (_formatConverter != null)
			{
				_formatConverter.Dispose();
				_formatConverter = null;
			}
			IntPtr intPtr = Marshal.StringToHGlobalUni(Filename);
			if (AVProWindowsMediaPlugin.LoadAudio(_movieHandle, intPtr, loop))
			{
				Volume = _volume;
				DurationSeconds = AVProWindowsMediaPlugin.GetDurationSeconds(_movieHandle);
				Debug.Log("[AVProWindowsMedia] Loaded audio " + Filename + " " + DurationSeconds.ToString("F2") + " sec");
			}
			else
			{
				Debug.LogWarning("[AVProWindowsMedia] Movie failed to load");
				Close();
			}
			Marshal.FreeHGlobal(intPtr);
		}
		else
		{
			Debug.LogWarning("[AVProWindowsMedia] No movie file specified");
			Close();
		}
		return _movieHandle >= 0;
	}

	private void PreRoll()
	{
		if (_movieHandle >= 0)
		{
			float volume = Volume;
			Volume = 0f;
			Play();
			DateTime now = DateTime.Now;
			while (!AVProWindowsMediaPlugin.IsNextFrameReadyForGrab(_movieHandle) && !((DateTime.Now - now).TotalSeconds > 3.0))
			{
			}
			Update(true);
			Pause();
			AVProWindowsMediaPlugin.SeekFrames(_movieHandle, 0u);
			Volume = volume;
		}
	}

	public void PlayFor1Frame()
	{
		if (_movieHandle >= 0)
		{
			Play();
			DateTime now = DateTime.Now;
			while (!AVProWindowsMediaPlugin.IsNextFrameReadyForGrab(_movieHandle) && !((DateTime.Now - now).TotalSeconds > 3.0))
			{
			}
			Update(true);
			Pause();
		}
	}

	public bool Update(bool force)
	{
		bool result = false;
		if (_movieHandle >= 0 && IsPlaying)
		{
			AVProWindowsMediaPlugin.Update(_movieHandle);
			bool flag = true;
			if (!force)
			{
				flag = AVProWindowsMediaPlugin.IsNextFrameReadyForGrab(_movieHandle);
			}
			if (flag && _formatConverter != null)
			{
				_formatConverter.Update();
				result = true;
			}
		}
		return result;
	}

	public void Play()
	{
		if (_movieHandle >= 0)
		{
			AVProWindowsMediaPlugin.Play(_movieHandle);
			IsPlaying = true;
		}
	}

	public void Pause()
	{
		if (_movieHandle >= 0)
		{
			AVProWindowsMediaPlugin.Pause(_movieHandle);
			IsPlaying = false;
		}
	}

	public void Rewind()
	{
		if (_movieHandle >= 0)
		{
			PositionSeconds = 0f;
		}
	}

	public void Dispose()
	{
		Close();
		if (_formatConverter != null)
		{
			_formatConverter.Dispose();
			_formatConverter = null;
		}
	}

	private void Close()
	{
		Pause();
		AVProWindowsMediaPlugin.Stop(_movieHandle);
		int width = (Height = 0);
		Width = width;
		if (_movieHandle >= 0)
		{
			AVProWindowsMediaPlugin.FreeInstanceHandle(_movieHandle);
			_movieHandle = -1;
		}
	}
}
