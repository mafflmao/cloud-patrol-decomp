using System;
using System.Collections;
using UnityEngine;

public class ActivateSellDialog : MonoBehaviour
{
	public GameObject destroyParticle;

	public GameObject visibleStuff;

	public SoundEventData transitionInSfx;

	public SoundEventData transitionOutSfx;

	public Action DismissAction;

	public void Display()
	{
		iTween.ScaleFrom(visibleStuff.gameObject, Vector3.zero, 0.333f);
		SoundEventManager.Instance.Play2D(transitionInSfx);
	}

	public void Update()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
		}
	}

	private void ButtonPressed()
	{
		UIManager.instance.blockInput = true;
		StartCoroutine(Dismiss());
	}

	private void JoinButtonPressed()
	{
		ButtonPressed();
		if (StateManager.Instance.CurrentStateName == "Results")
		{
			StateManager.Instance.LoadAndActivateState("LoadActivate");
		}
		else
		{
			ActivateWatcher.Instance.ReclaimMemoryThenLaunchActivate(false);
		}
	}

	protected virtual IEnumerator Dismiss()
	{
		if (DismissAction != null)
		{
			DismissAction();
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
