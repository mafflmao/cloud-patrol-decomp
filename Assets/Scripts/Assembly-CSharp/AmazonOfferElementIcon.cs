using UnityEngine;

public class AmazonOfferElementIcon : MonoBehaviour
{
	public PackedSprite sprite;

	public Elements.Type elementType;

	public void UpdateGraphic(ElementData myElement)
	{
		string text = string.Empty;
		switch (myElement.elementType)
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
	}
}
