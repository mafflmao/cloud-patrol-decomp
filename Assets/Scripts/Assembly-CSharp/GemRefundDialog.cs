using System;
using System.Collections;
using UnityEngine;

public class GemRefundDialog : MonoBehaviour
{
	public SpriteText messageText;

	public SpriteText titleText;

	public UIButton3D continueButton;

	public GameObject destroyParticle;

	public GameObject visibleStuff;

	public SoundEventData transitionInSfx;

	public SoundEventData transitionOutSfx;

	public SoundEventData continueButtonPressedSfx;

	public SoundEventData cancelButtonPressedSfx;

	private Action _cancelPressedAction;

	private Action _confirmPressedAction;

	private void OnEnable()
	{
		StateManager.StateDeactivated += HandleStateDeactivated;
	}

	private void OnDisable()
	{
		StateManager.StateDeactivated -= HandleStateDeactivated;
	}

	private void HandleStateDeactivated(object sender, StateEventArgs e)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Display(string message, string title, Action onConfirmPressed, Action onCancelPressed)
	{
		messageText.Text = message;
		titleText.Text = title;
		_confirmPressedAction = onConfirmPressed;
		_cancelPressedAction = onCancelPressed;
		iTween.ScaleFrom(visibleStuff.gameObject, Vector3.zero, 0.333f);
		SoundEventManager.Instance.Play2D(transitionInSfx);
		UIManager.instance.blockInput = false;
	}

	public void Display(string message, Action onConfirmPressed, Action onCancelPressed)
	{
		Display(message, titleText.Text, onConfirmPressed, onCancelPressed);
	}

	private void CancelButtonPressed()
	{
		UIManager.instance.blockInput = true;
		SoundEventManager.Instance.Play2D(cancelButtonPressedSfx);
		StartCoroutine(DismissCoroutine(false));
	}

	private void ContinueButtonPressed()
	{
		UIManager.instance.blockInput = true;
		SoundEventManager.Instance.Play2D(continueButtonPressedSfx);
		StartCoroutine(DismissCoroutine(true));
	}

	private IEnumerator DismissCoroutine(bool confirmPressed)
	{
		yield return new WaitForSeconds(0.5f);
		if (confirmPressed && _confirmPressedAction != null)
		{
			_confirmPressedAction();
		}
		else if (!confirmPressed && _cancelPressedAction != null)
		{
			_cancelPressedAction();
		}
		iTween.ScaleTo(visibleStuff.gameObject, Vector3.zero, 0.333f);
		SoundEventManager.Instance.Play2D(transitionOutSfx);
		yield return new WaitForSeconds(0.3f);
		UnityEngine.Object.Instantiate(destroyParticle, base.transform.position, Quaternion.identity);
		UnityEngine.Object.Destroy(base.gameObject);
		UIManager.instance.blockInput = false;
	}
}
