using UnityEngine;

public class MessageTray : MonoBehaviour
{
	public GameObject background;

	public Color textColor = Color.white;

	public Color textDropColor = Color.black;

	public Color spriteColor = Color.white;

	public Vector3 fadeScale;

	public float fadeAlpha;

	public float fadeTime;

	public void FadeInImmediate()
	{
		GetComponent<UIPanel>().BringIn();
		base.transform.localScale = Vector3.one;
	}

	public void FadeOut()
	{
		GetComponent<UIPanel>().Dismiss();
		iTween.ScaleTo(base.gameObject, iTween.Hash("scale", fadeScale, "time", fadeTime, "easeType", iTween.EaseType.easeOutQuad));
	}

	public void FadeIn()
	{
		GetComponent<UIPanel>().BringIn();
		iTween.ScaleTo(base.gameObject, iTween.Hash("scale", Vector3.one, "time", fadeTime, "easeTime", iTween.EaseType.easeInQuad));
	}
}
