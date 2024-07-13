using UnityEngine;
using UnityEngine.UI;

public class HighScoreBackground : MonoBehaviour
{
	private Image guiFlash;

	private Texture2D flashTexture;

	public Color bgColor;

	private void Start()
	{
		flashTexture = new Texture2D(1, 1);
		flashTexture.SetPixel(0, 0, Color.white);
		flashTexture.Apply();
	}
}
