using UnityEngine;

public class TweenPropagator : MonoBehaviour
{
	public bool lockAlpha;

	private AutoSpriteBase[] _uiElements;

	private Color _currColor = Color.white;

	public Color color
	{
		get
		{
			return _currColor;
		}
		set
		{
			_currColor = value;
			AutoSpriteBase[] uiElements = _uiElements;
			foreach (AutoSpriteBase autoSpriteBase in uiElements)
			{
				if (!(autoSpriteBase == null))
				{
					Color currColor = _currColor;
					if (lockAlpha)
					{
						currColor = autoSpriteBase.color;
						currColor.a = _currColor.a;
					}
					autoSpriteBase.SetColor(currColor);
				}
			}
		}
	}

	private void Awake()
	{
		_uiElements = base.gameObject.GetComponentsInChildren<AutoSpriteBase>();
	}
}
