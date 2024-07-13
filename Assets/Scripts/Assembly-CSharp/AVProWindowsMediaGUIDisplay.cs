using UnityEngine;

[AddComponentMenu("AVPro Windows Media/GUI Display")]
public class AVProWindowsMediaGUIDisplay : MonoBehaviour
{
	public AVProWindowsMediaMovie _movie;

	public ScaleMode _scaleMode = ScaleMode.ScaleToFit;

	public bool _alphaBlend;

	public bool _fullScreen = true;

	public int _depth;

	public float _x;

	public float _y;

	public float _width = 1f;

	public float _height = 1f;

	public void OnGUI()
	{
		if (!(_movie == null))
		{
			_x = Mathf.Clamp01(_x);
			_y = Mathf.Clamp01(_y);
			_width = Mathf.Clamp01(_width);
			_height = Mathf.Clamp01(_height);
			if (_movie.OutputTexture != null)
			{
				GUI.depth = _depth;
				Rect position = ((!_fullScreen) ? new Rect(_x * (float)(Screen.width - 1), _y * (float)(Screen.height - 1), _width * (float)Screen.width, _height * (float)Screen.height) : new Rect(0f, 0f, Screen.width, Screen.height));
				GUI.DrawTexture(position, _movie.OutputTexture, _scaleMode, _alphaBlend);
			}
		}
	}
}
