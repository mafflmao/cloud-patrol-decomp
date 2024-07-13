using UnityEngine;

[AddComponentMenu("AVPro Windows Media/Manager (required)")]
public class AVProWindowsMediaManager : MonoBehaviour
{
	private static AVProWindowsMediaManager _instance;

	private bool _isOpenGL;

	private Material _materialBGRA32;

	private Material _materialYUY2;

	private Material _materialYUY2_709;

	private Material _materialUYVY;

	private Material _materialYVYU;

	private Material _materialHDYC;

	private Shader _shaderBGRA32;

	private Shader _shaderYUY2;

	private Shader _shaderYUY2_709;

	private Shader _shaderUYVY;

	private Shader _shaderYVYU;

	private Shader _shaderHDYC;

	public static AVProWindowsMediaManager Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			Debug.LogError("[AVProWindowsMedia] Trying to use component before it has started or after it has been destroyed.");
			return null;
		}
	}

	public bool IsOpenGL
	{
		get
		{
			return _isOpenGL;
		}
	}

	private void Start()
	{
		if (_instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		_instance = this;
		if (!Init())
		{
			Deinit();
			base.enabled = false;
		}
	}

	private void OnDestroy()
	{
		Deinit();
	}

	protected bool Init()
	{
		if (!AVProWindowsMediaPlugin.Init())
		{
			Debug.LogError("[AVProWindowsMedia] failed to initialise.");
			return false;
		}
		if (!LoadShader("Hidden/AVProWindowsMedia_CompositeBGRA2RGBA", out _shaderBGRA32))
		{
			return false;
		}
		if (!LoadShader("Hidden/AVProWindowsMedia_CompositeYUY22RGBA", out _shaderYUY2))
		{
			return false;
		}
		if (!LoadShader("Hidden/AVProWindowsMedia_CompositeYUY27092RGBA", out _shaderYUY2_709))
		{
			return false;
		}
		if (!LoadShader("Hidden/AVProWindowsMedia_CompositeUYVY2RGBA", out _shaderUYVY))
		{
			return false;
		}
		if (!LoadShader("Hidden/AVProWindowsMedia_CompositeYVYU2RGBA", out _shaderYVYU))
		{
			return false;
		}
		if (!LoadShader("Hidden/AVProWindowsMedia_CompositeHDYC2RGBA", out _shaderHDYC))
		{
			return false;
		}
		_materialYUY2 = new Material(_shaderYUY2);
		_materialYUY2_709 = new Material(_shaderYUY2_709);
		_materialUYVY = new Material(_shaderUYVY);
		_materialYVYU = new Material(_shaderYVYU);
		_materialHDYC = new Material(_shaderHDYC);
		_materialBGRA32 = new Material(_shaderBGRA32);
		_isOpenGL = SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL");
		return true;
	}

	public void Deinit()
	{
		Object.Destroy(_materialYUY2);
		Object.Destroy(_materialYUY2_709);
		Object.Destroy(_materialUYVY);
		Object.Destroy(_materialYVYU);
		Object.Destroy(_materialHDYC);
		Object.Destroy(_materialBGRA32);
		_materialYUY2 = null;
		_materialYUY2_709 = null;
		_materialUYVY = null;
		_materialYVYU = null;
		_materialHDYC = null;
		_materialBGRA32 = null;
		_shaderBGRA32 = null;
		_shaderYUY2 = null;
		_shaderYUY2_709 = null;
		_shaderUYVY = null;
		_shaderYVYU = null;
		_shaderHDYC = null;
		AVProWindowsMediaMovie[] array = (AVProWindowsMediaMovie[])Object.FindObjectsOfType(typeof(AVProWindowsMediaMovie));
		if (array != null && array.Length > 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UnloadMovie();
			}
		}
		if (_instance == this)
		{
			_instance = null;
		}
		AVProWindowsMediaPlugin.Deinit();
	}

	public Material GetConversionMaterial(AVProWindowsMediaPlugin.VideoFrameFormat format, bool useBT709)
	{
		Material result = null;
		switch (format)
		{
		case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_YUY2:
			result = _materialYUY2;
			if (useBT709)
			{
				result = _materialYUY2_709;
			}
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_UYVY:
			result = _materialUYVY;
			if (useBT709)
			{
				result = _materialHDYC;
			}
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_YVYU:
			result = _materialYVYU;
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_HDYC:
			result = _materialHDYC;
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.RAW_BGRA32:
			result = _materialBGRA32;
			break;
		default:
			Debug.LogError("AVProWindowsMedia] Unknown video format '" + format);
			break;
		}
		return result;
	}

	private static bool LoadShader(string name, out Shader result)
	{
		result = Shader.Find(name);
		if (!result || !result.isSupported)
		{
			result = null;
			Debug.LogError("[AVProWindowsMedia] shader '" + name + "' not found or supported");
		}
		return result != null;
	}
}
