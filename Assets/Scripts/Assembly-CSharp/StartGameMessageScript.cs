using UnityEngine;

public class StartGameMessageScript : MonoBehaviour
{
	public float fadeInTime = 3f;

	public float fadeOutTime = 0.5f;

	public float lingerTime = 6f;

	public PackedSprite _sprite;

	private void Start()
	{
		iTween.FadeFrom(_sprite.gameObject, iTween.Hash("alpha", 0f, "time", fadeInTime, "oncomplete", "DoneFadeIn", "oncompletetarget", base.gameObject));
	}

	private void DoneFadeIn()
	{
		iTween.FadeTo(_sprite.gameObject, iTween.Hash("alpha", 0f, "time", fadeOutTime, "delay", lingerTime, "oncomplete", "Done", "oncompletetarget", base.gameObject));
	}

	private void Done()
	{
		Object.Destroy(base.gameObject);
	}
}
