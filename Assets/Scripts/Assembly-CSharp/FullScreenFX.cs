using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenFX : MonoBehaviour
{
	private Image guiFlash;

	private Texture2D flashTexture;

	private float alphaFade = 1f;

	public Color HitColor;

	private Color alphaColor;

	private float intervalTime = 0.01666f;

	private float intervalAlpha;

	private float numIntervals;

	private void Start()
	{
	}

	private IEnumerator FadeAlpha()
	{
		while (guiFlash.enabled && alphaFade > 0f)
		{
			alphaFade -= intervalAlpha;
			if (alphaFade < 0f)
			{
				alphaFade = 0f;
			}
			alphaColor = guiFlash.color;
			alphaColor.a = alphaFade;
			guiFlash.color = alphaColor;
			yield return new WaitForSeconds(intervalTime);
		}
		guiFlash.enabled = false;
	}

	public void Flash(float duration, Color _flashColor)
	{
		guiFlash.color = _flashColor;
		alphaFade = _flashColor.a;
		guiFlash.enabled = true;
		numIntervals = duration / intervalTime;
		intervalAlpha = alphaFade / numIntervals;
		StartCoroutine(FadeAlpha());
	}

	public void CancelFlash()
	{
		guiFlash.enabled = false;
	}
}
