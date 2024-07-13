using System;
using System.Collections;
using UnityEngine;

public class TimedSlider : MonoBehaviour
{
	public GameObject gameObjectToMove;

	public Transform restingPosition;

	public float restTime = 2f;

	public bool restForever;

	public float slideSpeedIn = 2500f;

	public float slideSpeedOut = 50f;

	public iTween.EaseType easeInType = iTween.EaseType.easeOutExpo;

	public iTween.EaseType easeOutType = iTween.EaseType.easeOutExpo;

	private Vector3 _originalPosition;

	private bool _itemInLingerPosition;

	private float _pollUpdateTime = 0.1f;

	private float _remainingLingerTime;

	private bool _initialized;

	public bool obscuresScore;

	private bool _isGoingToSlideBackAfterDelay;

	public bool IsCurrentlySliding { get; private set; }

	public static event EventHandler SlideInBegin;

	public static event EventHandler<CancellableEventArgs> SlideOutBegin;

	public static event EventHandler SlideInOutComplete;

	private void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!_initialized)
		{
			_originalPosition = gameObjectToMove.transform.position;
			gameObjectToMove.transform.position = restingPosition.position;
			_initialized = true;
		}
	}

	public void SlideIn()
	{
		SlideIn(0f);
	}

	public void SlideIn(float verticalOffset)
	{
		Initialize();
		if (!(gameObjectToMove != null))
		{
			return;
		}
		IsCurrentlySliding = true;
		if (_itemInLingerPosition)
		{
			if (!_isGoingToSlideBackAfterDelay && !restForever)
			{
				HideHeaderAfterDelay();
			}
			_remainingLingerTime = restTime;
			return;
		}
		Vector3 vector = new Vector3(_originalPosition.x, _originalPosition.y + verticalOffset, _originalPosition.z);
		iTween.MoveTo(gameObjectToMove, iTween.Hash("position", vector, "speed", slideSpeedIn, "easetype", easeInType, "oncomplete", "HideHeaderAfterDelay", "oncompletetarget", base.gameObject));
		OnSlideInBegin();
	}

	public void SlideOutImmediate()
	{
		StopAnimatingAndWaiting();
		CancellableEventArgs cancellableEventArgs = new CancellableEventArgs();
		OnSlideOutBegin(cancellableEventArgs);
		if (cancellableEventArgs.IsCancelled)
		{
			Debug.Log("Slide-out (immediate) was cancelled.");
			return;
		}
		gameObjectToMove.transform.position = restingPosition.position;
		_itemInLingerPosition = false;
		IsCurrentlySliding = false;
		OnSlideInOutComplete();
	}

	public void SlideOutAnimated()
	{
		StopAnimatingAndWaiting();
		CancellableEventArgs cancellableEventArgs = new CancellableEventArgs();
		OnSlideOutBegin(cancellableEventArgs);
		if (!cancellableEventArgs.IsCancelled)
		{
			_itemInLingerPosition = false;
			if (gameObjectToMove != null)
			{
				iTween.MoveTo(gameObjectToMove, iTween.Hash("position", restingPosition.position, "speed", slideSpeedOut, "easetype", easeOutType, "oncomplete", "SetStartingState", "oncompletetarget", base.gameObject));
			}
		}
	}

	private void StopAnimatingAndWaiting()
	{
		StopAllCoroutines();
		iTween.Stop(base.gameObject);
		_isGoingToSlideBackAfterDelay = false;
	}

	private void SetStartingState()
	{
		IsCurrentlySliding = false;
		OnSlideInOutComplete();
	}

	private void HideHeaderAfterDelay()
	{
		if (!restForever)
		{
			_itemInLingerPosition = true;
			_remainingLingerTime = restTime;
			StartCoroutine(SlideBackAfterDelayCoroutine());
		}
	}

	private IEnumerator SlideBackAfterDelayCoroutine()
	{
		for (_isGoingToSlideBackAfterDelay = true; _remainingLingerTime > 0f; _remainingLingerTime -= _pollUpdateTime)
		{
			yield return new WaitForSeconds(_pollUpdateTime);
		}
		_isGoingToSlideBackAfterDelay = false;
		if (!restForever)
		{
			SlideOutAnimated();
		}
	}

	private void OnSlideInBegin()
	{
		if (TimedSlider.SlideInBegin != null)
		{
			TimedSlider.SlideInBegin(this, new EventArgs());
		}
	}

	private void OnSlideOutBegin(CancellableEventArgs args)
	{
		if (TimedSlider.SlideOutBegin != null)
		{
			TimedSlider.SlideOutBegin(this, args);
		}
	}

	private void OnSlideInOutComplete()
	{
		if (TimedSlider.SlideInOutComplete != null)
		{
			TimedSlider.SlideInOutComplete(this, new EventArgs());
		}
	}
}
