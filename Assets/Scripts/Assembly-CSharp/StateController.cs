using System.Collections;
using UnityEngine;

public class StateController : MonoBehaviour
{
	public const float defaultAnimateInTime = 0.5f;

	public const float defaultAnimateOutTime = 0.5f;

	protected bool mActive;

	protected string mLastState = string.Empty;

	protected bool mFirstRun = true;

	public float animateInTime = 0.5f;

	public float animateOutTime = 0.5f;

	public GameObject[] objectToDeactivate;

	protected bool IsShowing { get; private set; }

	protected virtual void OnStateLoaded()
	{
		if (StateManager.Instance.StateCount > 1)
		{
			HideState();
		}
	}

	protected virtual void OnStateActivate(string oldState)
	{
		for (int i = 0; i < objectToDeactivate.Length; i++)
		{
			objectToDeactivate[i].SetActive(false);
		}
		mActive = true;
		mLastState = oldState;
		StartCoroutine(AnimateStateInInternal());
	}

	protected virtual void OnStateDeactivate(string oldState)
	{
		mActive = false;
		StartCoroutine(AnimateStateOut());
	}

	protected virtual void OnStateBack()
	{
		StateManager.Instance.LoadAndActivateState(mLastState);
	}

	protected virtual void HideState()
	{
		IsShowing = false;
		base.transform.position = new Vector3(0f, 10000f, 0f);
	}

	private IEnumerator AnimateStateInInternal()
	{
		yield return StartCoroutine(AnimateStateIn());
		if (LoadingPanel.InstanceNoAutocreate != null && LoadingPanel.InstanceAutoCreate.DismissOnStateChange)
		{
			LoadingPanel.InstanceNoAutocreate.Dismiss();
		}
		UIManager.instance.blockInput = false;
	}

	protected virtual IEnumerator AnimateStateIn()
	{
		HideState();
		yield return new WaitForEndOfFrame();
		while (StateManager.Instance.Loading)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.1f);
		ShowState();
	}

	protected virtual IEnumerator AnimateStateOut()
	{
		UIManager.instance.blockInput = true;
		HideState();
		yield return new WaitForSeconds(0.1f);
	}

	protected virtual void ShowState()
	{
		IsShowing = true;
		base.transform.position = Vector3.zero;
	}

	public virtual void AnimateOutAll()
	{
		HeaderUI.Instance.visible = false;
		FooterUI.Instance.visible = false;
	}
}
