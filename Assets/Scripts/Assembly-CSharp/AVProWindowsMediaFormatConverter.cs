using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AVProWindowsMediaFormatConverter : IDisposable
{
	private int _movieHandle;

	private Texture2D _texture;

	private RenderTexture _target;

	private Material _conversionMaterial;

	private GCHandle _frameHandle;

	private Color32[] _frameData;

	private int _width;

	private int _height;

	private bool _flipX;

	private bool _flipY;

	private AVProWindowsMediaPlugin.VideoFrameFormat _sourceVideoFormat;

	private bool _useBT709;

	public Texture OutputTexture
	{
		get
		{
			return _target;
		}
	}

	public bool ValidPicture { get; private set; }

	public bool Build(int movieHandle, int width, int height, AVProWindowsMediaPlugin.VideoFrameFormat format, bool useBT709, bool flipX, bool flipY)
	{
		ValidPicture = false;
		_movieHandle = movieHandle;
		_width = width;
		_height = height;
		_sourceVideoFormat = format;
		_flipX = flipX;
		_flipY = flipY;
		_conversionMaterial = AVProWindowsMediaManager.Instance.GetConversionMaterial(_sourceVideoFormat, useBT709);
		if (_conversionMaterial == null)
		{
			return false;
		}
		CreateTexture();
		CreateRenderTexture();
		CreateBuffer();
		return true;
	}

	public bool Update()
	{
		bool flag = UpdateTexture();
		if (flag)
		{
			DoFormatConversion();
		}
		return flag;
	}

	private bool UpdateTexture()
	{
		bool flag = false;
		if (AVProWindowsMediaManager.Instance.IsOpenGL)
		{
			flag = AVProWindowsMediaPlugin.UpdateTextureGL(_movieHandle, _texture.GetNativeTextureID());
			GL.InvalidateState();
		}
		else
		{
			flag = AVProWindowsMediaPlugin.GetFramePixels(_movieHandle, _frameHandle.AddrOfPinnedObject(), _texture.width, _texture.height);
			if (flag)
			{
				_texture.SetPixels32(_frameData);
				_texture.Apply(false, false);
			}
		}
		return flag;
	}

	public void Dispose()
	{
		_width = (_height = 0);
		if (_conversionMaterial != null)
		{
			_conversionMaterial.SetTexture("_MainTex", null);
			_conversionMaterial = null;
		}
		if (_target != null)
		{
			_target.Release();
			UnityEngine.Object.Destroy(_target);
			_target = null;
		}
		if (_texture != null)
		{
			UnityEngine.Object.Destroy(_texture);
			_texture = null;
		}
		if (_frameHandle.IsAllocated)
		{
			_frameHandle.Free();
			_frameData = null;
		}
	}

	private void CreateTexture()
	{
		int num = _width;
		int num2 = _height;
		bool flag = _sourceVideoFormat != AVProWindowsMediaPlugin.VideoFrameFormat.RAW_BGRA32;
		if (flag)
		{
			num /= 2;
		}
		if ((!Mathf.IsPowerOfTwo(_width) || !Mathf.IsPowerOfTwo(_height)) && (!AVProWindowsMediaManager.Instance.IsOpenGL || flag))
		{
			num = Mathf.NextPowerOfTwo(num);
			num2 = Mathf.NextPowerOfTwo(num2);
		}
		if (_texture != null && (_texture.width != num || _texture.height != num2))
		{
			UnityEngine.Object.Destroy(_texture);
			_texture = null;
		}
		if (_texture == null)
		{
			_texture = new Texture2D(num, num2, TextureFormat.ARGB32, false);
			_texture.hideFlags = HideFlags.HideAndDontSave;
			_texture.wrapMode = TextureWrapMode.Clamp;
			_texture.filterMode = FilterMode.Point;
			_texture.anisoLevel = 0;
			_texture.Apply(false, false);
		}
	}

	private void CreateRenderTexture()
	{
		if (_target != null && (_target.width != _width || _target.height != _height))
		{
			_target.Release();
			UnityEngine.Object.Destroy(_target);
			_target = null;
		}
		if (_target == null)
		{
			ValidPicture = false;
			_target = new RenderTexture(_width, _height, 0);
			_target.wrapMode = TextureWrapMode.Clamp;
			_target.useMipMap = false;
			_target.hideFlags = HideFlags.HideAndDontSave;
			_target.format = RenderTextureFormat.ARGB32;
			_target.filterMode = FilterMode.Bilinear;
			_target.Create();
		}
	}

	private void CreateBuffer()
	{
		if (!AVProWindowsMediaManager.Instance.IsOpenGL)
		{
			if (_frameHandle.IsAllocated && _frameData != null && _frameData.Length < _texture.width * _texture.height)
			{
				_frameHandle.Free();
				_frameData = null;
			}
			if (_frameData == null)
			{
				_frameData = new Color32[_texture.width * _texture.height];
				_frameHandle = GCHandle.Alloc(_frameData, GCHandleType.Pinned);
			}
		}
	}

	private void DoFormatConversion()
	{
		if (!(_texture == null))
		{
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = _target;
			if (_sourceVideoFormat != 0)
			{
				_conversionMaterial.SetFloat("_TextureWidth", _texture.width);
			}
			_conversionMaterial.SetTexture("_MainTex", _texture);
			_conversionMaterial.SetPass(0);
			GL.PushMatrix();
			GL.LoadOrtho();
			DrawQuad(_flipX, _flipY);
			GL.PopMatrix();
			RenderTexture.active = active;
			ValidPicture = true;
		}
	}

	private void DrawQuad(bool invertX, bool invertY)
	{
		GL.Begin(7);
		float num;
		float num2;
		if (invertX)
		{
			num = 1f;
			num2 = 0f;
		}
		else
		{
			num = 0f;
			num2 = 1f;
		}
		float num3;
		float num4;
		if (invertY)
		{
			num3 = 1f;
			num4 = 0f;
		}
		else
		{
			num3 = 0f;
			num4 = 1f;
		}
		if (_width != _texture.width)
		{
			float num5 = (float)_width / (float)_texture.width;
			num *= num5;
			num2 *= num5;
		}
		if (_height != _texture.height)
		{
			float num6 = (float)_height / (float)_texture.height;
			num3 *= num6;
			num4 *= num6;
		}
		GL.TexCoord2(num, num3);
		GL.Vertex3(0f, 0f, 0.1f);
		GL.TexCoord2(num2, num3);
		GL.Vertex3(1f, 0f, 0.1f);
		GL.TexCoord2(num2, num4);
		GL.Vertex3(1f, 1f, 0.1f);
		GL.TexCoord2(num, num4);
		GL.Vertex3(0f, 1f, 0.1f);
		GL.End();
	}
}
