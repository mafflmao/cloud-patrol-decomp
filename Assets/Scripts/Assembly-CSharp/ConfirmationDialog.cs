using System;
using System.Collections;
using UnityEngine;

public class ConfirmationDialog : MonoBehaviour
{
	public const float ScaleInOutTime = 0.333f;

	public SpriteText spriteText;

	public GameObject destroyParticle;

	public GameObject visibleStuff;

	public bool BlockInputOnConfirm;

	public SoundEventData transitionInSFX;

	private bool? userInput;

	private bool cancelOnEvents;

	private Action cancelAction;

	public SoundEventData OnYesDismissedSound { get; set; }

	public SoundEventData OnNoDismissedSound { get; set; }

	private void OnEnable()
	{
		StateManager.StateDeactivated += HandleStateDeactivated;
		Bedrock.UnlockContentChanged += HandleUnlockContentChanged;
	}

	private void OnDisable()
	{
		StateManager.StateDeactivated -= HandleStateDeactivated;
		Bedrock.UnlockContentChanged -= HandleUnlockContentChanged;
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			HandleUnlockContentChanged(this, null);
		}
	}

	private void HandleStateDeactivated(object sender, StateEventArgs e)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void HandleUnlockContentChanged(object sender, EventArgs e)
	{
		if (cancelOnEvents)
		{
			if (cancelAction != null)
			{
				cancelAction();
			}
			iTween.ScaleTo(visibleStuff.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.333f, "ignoretimescale", true, "oncomplete", "Close", "oncompletetarget", base.gameObject));
		}
	}

	public IEnumerator Display(string message, Action onYesClicked, Action onNoClicked, bool dismissOnEvents = false)
	{
		cancelAction = onNoClicked;
		cancelOnEvents = dismissOnEvents;
		Debug.Log("Display Confirmation Dialog");
		if (message != string.Empty)
		{
			spriteText.Text = message;
		}
		iTween.ScaleFrom(visibleStuff.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.333f, "ignoretimescale", true));
		SoundEventManager.Instance.Play2D(transitionInSFX);
		UIManager.instance.blockInput = true;
		yield return new WaitForEndOfFrame();
		UIManager.instance.blockInput = false;
		while (!userInput.HasValue)
		{
			yield return new WaitForEndOfFrame();
		}
		UIManager.instance.blockInput = true;
		if (userInput.Value)
		{
			if (onYesClicked != null)
			{
				onYesClicked();
			}
		}
		else if (onNoClicked != null)
		{
			onNoClicked();
		}
		float startTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < startTime + 0.25f)
		{
			yield return new WaitForEndOfFrame();
		}
		if (userInput.Value && (bool)OnYesDismissedSound)
		{
			SoundEventManager.Instance.Play2D(OnYesDismissedSound);
		}
		else if (!userInput.Value && (bool)OnNoDismissedSound)
		{
			SoundEventManager.Instance.Play2D(OnNoDismissedSound);
		}
		iTween.ScaleTo(visibleStuff.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.333f, "ignoretimescale", true, "oncomplete", "Close", "oncompletetarget", base.gameObject));
	}

	private void Close()
	{
		if (Time.timeScale != 0f)
		{
			UnityEngine.Object.Instantiate(destroyParticle, base.transform.position, Quaternion.identity);
		}
		UIManager.instance.blockInput = userInput.HasValue && userInput.Value && BlockInputOnConfirm;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void noBtnHit()
	{
		userInput = false;
	}

	private void yesBtnHit()
	{
		userInput = true;
	}
}
