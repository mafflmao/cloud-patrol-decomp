using UnityEngine;

public class BountyStar : MonoBehaviour
{
	private PackedSprite _icon;

	private bool _enabled;

	public bool IsEnabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			string text = ((!value) ? "disabled" : "enabled");
			if (_icon == null)
			{
				_icon = GetComponent<PackedSprite>();
			}
			if (_icon != null)
			{
				_icon.PlayAnim(text);
			}
			_enabled = value;
		}
	}

	private void Start()
	{
		IsEnabled = _enabled;
	}
}
