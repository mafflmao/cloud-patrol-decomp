using System;
using UnityEngine;

public class LoadoutElementIcon : MonoBehaviour
{
	public enum Mode
	{
		activeSkylander = 0,
		elementOfTheDay = 1
	}

	public PackedSprite sprite;

	public Elements.Type elementType;

	public Mode mode;

	public UIButton eotdNoMatchAlert;

	[HideInInspector]
	public Elements.Type ElementType
	{
		get
		{
			return elementType;
		}
		set
		{
			elementType = value;
			UpdateGraphic();
		}
	}

	public void Start()
	{
		UpdateGraphic();
	}

	public void OnEnable()
	{
		StartGameSettings.ActiveSkylanderChanged += HandleStartGameSettingsActiveSkylanderChanged;
	}

	private void HandleStartGameSettingsActiveSkylanderChanged(object sender, EventArgs e)
	{
		UpdateGraphic();
	}

	public void OnDisable()
	{
		StartGameSettings.ActiveSkylanderChanged -= HandleStartGameSettingsActiveSkylanderChanged;
	}

	public void UpdateGraphic()
	{
		if (sprite == null)
		{
			return;
		}
		if (mode == Mode.activeSkylander)
		{
			elementType = StartGameSettings.Instance.activeSkylander.elementData.elementType;
		}
		else
		{
			elementType = Elements.Type.Air;
		}
		string text = string.Empty;
		switch (elementType)
		{
		case Elements.Type.Water:
			text = "Water";
			break;
		case Elements.Type.Life:
			text = "Life";
			break;
		case Elements.Type.Magic:
			text = "Magic";
			break;
		case Elements.Type.Undead:
			text = "Undead";
			break;
		case Elements.Type.Air:
			text = "Air";
			break;
		case Elements.Type.Tech:
			text = "Tech";
			break;
		case Elements.Type.Fire:
			text = "Fire";
			break;
		case Elements.Type.Earth:
			text = "Earth";
			break;
		}
		if (sprite.GetAnim(text) != null)
		{
			sprite.PlayAnim(text);
		}
		if (eotdNoMatchAlert != null)
		{
			if (elementType != StartGameSettings.Instance.activeSkylander.elementData.elementType)
			{
				eotdNoMatchAlert.Hide(false);
			}
			else
			{
				eotdNoMatchAlert.Hide(true);
			}
		}
	}
}
