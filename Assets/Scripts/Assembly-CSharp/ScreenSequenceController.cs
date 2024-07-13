using System;
using System.Collections;
using UnityEngine;

public abstract class ScreenSequenceController : MonoBehaviour
{
	public const float ScreenTimeout = 2f;

	private ILogger _log = LogBuilder.Instance.GetLogger(typeof(SplashScreenController), LogLevel.Warning);

	public bool AdvanceToFirstScreenOnStart = true;

	public bool AdvanceOnScreenTimeout = true;

	public PrefabPlaceholder BackgroundPrefab;

	protected int CurrentScreenNumber { get; set; }

	protected ScreenSequenceScreen CurrentScreen { get; set; }

	public static event EventHandler<EventArgs> SequenceComplete;

	protected virtual void Start()
	{
		if (AdvanceToFirstScreenOnStart)
		{
			AdvanceToNextScreen();
		}
		UIButton componentInChildren = BackgroundPrefab.InstantiatePrefab().GetComponentInChildren<UIButton>();
		if (componentInChildren != null)
		{
			UIManager.AddAction<ScreenSequenceController>(componentInChildren, base.gameObject, "HandleScreenPressed", POINTER_INFO.INPUT_EVENT.RELEASE);
		}
	}

	public void HandleScreenPressed()
	{
		if (CurrentScreen == null || CurrentScreen.AllowAdvanceToNextScreenFromUserPress())
		{
			AdvanceToNextScreen();
		}
	}

	public void ScreenTimedOut(ScreenSequenceScreen timedOutScreen)
	{
		if (AdvanceOnScreenTimeout)
		{
			if (timedOutScreen == CurrentScreen)
			{
				AdvanceToNextScreen();
			}
			else
			{
				Debug.LogError("We were informed that '" + timedOutScreen.name + "' timed out, but it wasn't the current screen. Probably race-condition. Ignored.");
			}
		}
	}

	protected void SetNextScreenWithAnimateOutAsync(ScreenSequenceScreen nextScreenPrefab, float animateOutWait)
	{
		StopAllCoroutines();
		StartCoroutine(SetNextScreenWithAnimateOut(nextScreenPrefab, animateOutWait));
	}

	private IEnumerator SetNextScreenWithAnimateOut(ScreenSequenceScreen nextScreenPrefab, float animateOutWait)
	{
		if (CurrentScreen != null)
		{
			CurrentScreen.StartAnimateOut();
			yield return new WaitForSeconds(animateOutWait);
		}
		UIManager.instance.enabled = true;
		CurrentScreen = InstantiateScreen(nextScreenPrefab);
	}

	protected virtual ScreenSequenceScreen InstantiateScreen(ScreenSequenceScreen screenSequenceScreenPrefab)
	{
		if (screenSequenceScreenPrefab == null)
		{
			Debug.LogError("Cannot instantiate next screen in sequence - prefab is null.");
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(screenSequenceScreenPrefab.gameObject);
		ScreenSequenceScreen component = gameObject.GetComponent<ScreenSequenceScreen>();
		gameObject.transform.parent = base.transform;
		component.Owner = this;
		return component;
	}

	public abstract void AdvanceToNextScreen();

	protected virtual void OnSequenceComplete(float animateOutTime)
	{
		_log.LogDebug("OnSequenceComplete({0})", animateOutTime);
		if (ScreenSequenceController.SequenceComplete != null)
		{
			ScreenSequenceController.SequenceComplete(this, new EventArgs());
		}
		StartCoroutine(SequenceCompleteCoroutine(animateOutTime));
	}

	protected IEnumerator SequenceCompleteCoroutine(float animateOutTime)
	{
		if (CurrentScreen != null)
		{
			CurrentScreen.StartAnimateOut();
			yield return new WaitForSeconds(animateOutTime);
		}
		UIManager.instance.enabled = true;
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
