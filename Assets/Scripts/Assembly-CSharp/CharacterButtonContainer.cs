using UnityEngine;

public class CharacterButtonContainer : MonoBehaviour
{
	public CharacterButton[] buttons;

	public void UpdateGraphics()
	{
		CharacterButton[] array = buttons;
		foreach (CharacterButton characterButton in array)
		{
			characterButton.UpdateGraphics();
		}
	}
}
