using UnityEngine;

public class GUICamera : MonoBehaviour
{
	public delegate void ResolutionChangedHandler();

	private Camera _guiCam;

	private float _width;

	private float _height;

	private float _widthNormal;

	private float _heightNormal;

	private float _widthMinAspect;

	private float _heightMinAspect;

	private float _screenAr;

	private float _offsetHoriz;

	private float _offsetVert;

	private Vector3[] _autoAdjustScales;

	public float width
	{
		get
		{
			return _width;
		}
	}

	public float height
	{
		get
		{
			return _height;
		}
	}

	public Vector3 sizeMinAspect
	{
		get
		{
			return new Vector3(_widthMinAspect, _heightMinAspect, 1f);
		}
	}

	public Vector3[] autoAdjustScales
	{
		get
		{
			return _autoAdjustScales;
		}
	}

	public float offsetHoriz
	{
		get
		{
			return _offsetHoriz;
		}
	}

	public float offsetVert
	{
		get
		{
			return _offsetVert;
		}
	}

	public event ResolutionChangedHandler resolutionChangedEvt;

	private void Awake()
	{
		_guiCam = base.gameObject.GetComponent<Camera>();
		if (_guiCam == null)
		{
			Debug.LogWarning("GUI System: Camera component could not be located in the GUICamera script");
		}
		CalculateSize();
		if (Object.FindObjectOfType(typeof(AudioListener)) == null)
		{
			base.gameObject.AddComponent<AudioListener>();
		}
	}

	private void CalculateSize()
	{
		_widthNormal = Screen.width;
		_heightNormal = Screen.height;
		float num = (float)Screen.width / (float)Screen.height;
		float num2 = GUISystem.ReferenceWidth / GUISystem.ReferenceHeight;
		if (num > num2)
		{
			_height = GUISystem.ReferenceHeight;
			_width = num * _height;
		}
		else
		{
			_width = GUISystem.ReferenceWidth;
			_height = _width / num;
		}
		ApplyScreenSizeToCamera();
		CalculatePanelAdjustments();
	}

	private void ApplyScreenSizeToCamera()
	{
		if (_guiCam != null)
		{
			_guiCam.orthographicSize = _height / 2f;
		}
		else
		{
			Debug.LogWarning("GUI System: Camera component is null when applying screen size to the camera");
		}
	}

	private void CalculatePanelAdjustments()
	{
		float num = _width - GUISystem.ReferenceWidth;
		float num2 = _height - GUISystem.ReferenceHeight;
		_offsetHoriz = num / 2f;
		_offsetVert = num2 / 2f;
		float num3 = _width / GUISystem.ReferenceWidth;
		float num4 = _height / GUISystem.ReferenceHeight;
		float num5 = (num3 + num4) / 2f;
		float num6 = ((!(num3 > num4)) ? num4 : num3);
		float num7 = ((!(num3 < num4)) ? num4 : num3);
		_autoAdjustScales = new Vector3[7];
		_autoAdjustScales[0] = new Vector3(1f, 1f, 1f);
		_autoAdjustScales[1] = new Vector3(num3, num4, 1f);
		_autoAdjustScales[2] = new Vector3(num3, 1f, 1f);
		_autoAdjustScales[3] = new Vector3(1f, num4, 1f);
		_autoAdjustScales[6] = new Vector3(num5, num5, 1f);
		_autoAdjustScales[4] = new Vector3(num6, num6, 1f);
		_autoAdjustScales[5] = new Vector3(num7, num7, 1f);
		_widthMinAspect = _width / num7;
		_heightMinAspect = _height / num7;
	}

	private void Update()
	{
	}
}
