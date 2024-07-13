using System;
using System.Collections;
using UnityEngine;

public class ErrorDialog : MonoBehaviour
{
	public SpriteText messageSpriteText;

	public SpriteText titleSpriteText;

	public SpriteText buttonSpriteText;

	public UIButton3D button;

	public GameObject destroyParticle;

	public GameObject visibleStuff;

	public SoundEventData transitionInSfx;

	public SoundEventData transitionOutSfx;

	public SoundEventData buttonPressSfx;

	private Action _dismissAction;

	public void Display(string titleText, string messageText, string buttonText, Action dismissAction)
	{
		_dismissAction = dismissAction;
		titleSpriteText.Text = titleText;
		messageSpriteText.Text = messageText;
		buttonSpriteText.Text = buttonText;
		iTween.ScaleFrom(visibleStuff.gameObject, Vector3.zero, 0.333f);
		SoundEventManager.Instance.Play2D(transitionInSfx);
	}

	private void ButtonPressed()
	{
		UIManager.instance.blockInput = true;
		SoundEventManager.Instance.Play2D(buttonPressSfx);
		StartCoroutine(Dismiss());
	}

	private IEnumerator Dismiss()
	{
		if (_dismissAction != null)
		{
			_dismissAction();
		}
		yield return new WaitForSeconds(0.5f);
		iTween.ScaleTo(visibleStuff.gameObject, Vector3.zero, 0.333f);
		SoundEventManager.Instance.Play2D(transitionOutSfx);
		yield return new WaitForSeconds(0.3f);
		UnityEngine.Object.Instantiate(destroyParticle, base.transform.position, Quaternion.identity);
		UnityEngine.Object.Destroy(base.gameObject);
		UIManager.instance.blockInput = false;
	}
}
