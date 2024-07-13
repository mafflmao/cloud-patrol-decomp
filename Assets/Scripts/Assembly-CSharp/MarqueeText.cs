using UnityEngine;

public class MarqueeText : MonoBehaviour
{
	public SpriteText text;

	public float pixelsPerSecond = 400f;

	private float _textWidth;

	private float _screenWidth;

	private string _pendingText;

	private GUISystem _guiSystem;

	public string ImmediateText
	{
		get
		{
			return _pendingText;
		}
		set
		{
			_pendingText = value;
			UpdateText();
		}
	}

	public string Text
	{
		get
		{
			return _pendingText;
		}
		set
		{
			_pendingText = value;
			if (text.transform.position == RightEdgeOfScreen)
			{
				UpdateText();
			}
		}
	}

	private Vector3 RightEdgeOfScreen
	{
		get
		{
			return new Vector3(_screenWidth / 2f, text.transform.position.y, text.transform.position.z);
		}
	}

	private void Start()
	{
		_guiSystem = GUISystem.Instance;
		_guiSystem.guiCamera.resolutionChangedEvt += OnResolutionChanged;
		_pendingText = text.Text;
		_textWidth = text.GetWidth(text.Text);
		OnResolutionChanged();
		UpdateText();
		MoveTextToRightEdgeOfScreen();
	}

	private void OnDestroy()
	{
		if (_guiSystem != null)
		{
			_guiSystem.guiCamera.resolutionChangedEvt -= OnResolutionChanged;
		}
	}

	private void OnResolutionChanged()
	{
		_screenWidth = GUISystem.ReferenceWidth * _guiSystem.guiCamera.autoAdjustScales[1].x;
		Debug.Log("Updated screen width: " + _screenWidth);
	}

	private void MoveTextToRightEdgeOfScreen()
	{
		text.transform.position = RightEdgeOfScreen;
	}

	private void UpdateText()
	{
		if (_pendingText != text.Text)
		{
			text.Text = _pendingText;
			_textWidth = text.GetWidth(text.Text);
		}
	}

	private void Update()
	{
		if (text.transform.position.x + _textWidth < (0f - _screenWidth) / 2f)
		{
			UpdateText();
			MoveTextToRightEdgeOfScreen();
		}
		else
		{
			text.transform.position -= new Vector3(pixelsPerSecond * Time.deltaTime, 0f, 0f);
		}
	}
}
