using UnityEngine;

public class GUIPanel : MonoBehaviour
{
	public enum AutoAdjustScaleMode
	{
		None = 0,
		Stretch = 1,
		StretchHorizontal = 2,
		StretchVertical = 3,
		StretchMaxToFitAspect = 4,
		StretchMinToFitAspect = 5,
		StretchAverageToFitAspect = 6
	}

	public enum AutoAdjustPosition
	{
		TopLeft = 0,
		TopCenter = 1,
		TopRight = 2,
		CenterLeft = 3,
		Center = 4,
		CenterRight = 5,
		BottomLeft = 6,
		BottomCenter = 7,
		BottomRight = 8
	}

	public AutoAdjustScaleMode _autoAdjustScaleMode;

	public AutoAdjustPosition _autoAdjustPosition = AutoAdjustPosition.Center;

	private GUISystem _guiSystem;

	private void Start()
	{
		_guiSystem = GUISystem.Instance;
		AdjustPosition();
		AdjustScale();
		_guiSystem.guiCamera.resolutionChangedEvt += OnResolutionChanged;
	}

	private void Update()
	{
	}

	private void AdjustPosition()
	{
		Vector3 localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		float offsetHoriz = _guiSystem.guiCamera.offsetHoriz;
		float offsetVert = _guiSystem.guiCamera.offsetVert;
		switch (_autoAdjustPosition)
		{
		case AutoAdjustPosition.TopLeft:
			localPosition.x = 0f - offsetHoriz;
			localPosition.y = offsetVert;
			break;
		case AutoAdjustPosition.TopCenter:
			localPosition.y = offsetVert;
			break;
		case AutoAdjustPosition.TopRight:
			localPosition.x = offsetHoriz;
			localPosition.y = offsetVert;
			break;
		case AutoAdjustPosition.CenterLeft:
			localPosition.x = 0f - offsetHoriz;
			break;
		case AutoAdjustPosition.CenterRight:
			localPosition.x = offsetHoriz;
			break;
		case AutoAdjustPosition.BottomLeft:
			localPosition.x = 0f - offsetHoriz;
			localPosition.y = 0f - offsetVert;
			break;
		case AutoAdjustPosition.BottomCenter:
			localPosition.y = 0f - offsetVert;
			break;
		case AutoAdjustPosition.BottomRight:
			localPosition.x = offsetHoriz;
			localPosition.y = 0f - offsetVert;
			break;
		}
		base.transform.localPosition = localPosition;
	}

	private void AdjustScale()
	{
		base.transform.localScale = _guiSystem.guiCamera.autoAdjustScales[(int)_autoAdjustScaleMode];
	}

	private void OnResolutionChanged()
	{
		AdjustPosition();
		AdjustScale();
	}

	private void OnDestroy()
	{
		if (_guiSystem != null)
		{
			_guiSystem.guiCamera.resolutionChangedEvt -= OnResolutionChanged;
		}
	}
}
