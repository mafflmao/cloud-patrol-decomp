using UnityEngine;

public class DifficultyUpCutscene : MonoBehaviour
{
	public SpriteText spriteText;

	public void UpdateText()
	{
		spriteText.Text = "Difficulty " + (int)(DifficultyManager.Instance.currentGlobalDifficulty / 0.1f) + "!";
	}
}
