using System;
using UnityEngine;

[RequireComponent(typeof(SimpleSprite))]
public class ElementIcon : MonoBehaviour
{
	public const float IconSize = 59f;

	public SimpleSprite sprite;

	public UIButton eotdNoMatchAlert;

	public Elements.Type elementType;

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
		elementType = Elements.Type.Air;
		if (sprite == null)
		{
			return;
		}
		Vector2 lowerLeftPixel = sprite.lowerLeftPixel;
		switch (elementType)
		{
		case Elements.Type.Water:
			lowerLeftPixel = new Vector2(0f, 59f);
			break;
		case Elements.Type.Life:
			lowerLeftPixel = new Vector2(59f, 59f);
			break;
		case Elements.Type.Magic:
			lowerLeftPixel = new Vector2(118f, 59f);
			break;
		case Elements.Type.Undead:
			lowerLeftPixel = new Vector2(177f, 59f);
			break;
		case Elements.Type.Air:
			lowerLeftPixel = new Vector2(0f, 118f);
			break;
		case Elements.Type.Tech:
			lowerLeftPixel = new Vector2(59f, 118f);
			break;
		case Elements.Type.Fire:
			lowerLeftPixel = new Vector2(118f, 118f);
			break;
		case Elements.Type.Earth:
			lowerLeftPixel = new Vector2(177f, 118f);
			break;
		}
		sprite.SetLowerLeftPixel(lowerLeftPixel);
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
