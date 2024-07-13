using UnityEngine;

public class ElementOfTheDayPopup : MonoBehaviour
{
	public ElementIcon elementIcon;

	private void Start()
	{
		UpdateGraphics();
	}

	public void UpdateGraphics()
	{
		elementIcon.ElementType = Elements.Type.Air;
	}
}
