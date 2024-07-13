using System;
using System.Collections;
using UnityEngine;

public abstract class PeekabooTroll : TrollBase
{
	private const float PeekabooTransitionTime = 0.05f;

	protected const float WaitDownTime = 2.5f;

	protected const float WaitUpTime = 2.5f;

	private static readonly Vector3 PeekOutDistance = new Vector3(0f, 0.4f, 0f);

	public bool peekABoo;

	private Vector3 _peekabooVisiblePosition;

	private Vector3 _peekabooHiddenPosition;

	private bool _blockHiding;

	private PeekabooStates _state;

	public PeekabooStates PeekabooState
	{
		get
		{
			return _state;
		}
		set
		{
			if (_state != value)
			{
				_state = value;
				OnPeekabooStateChanged();
			}
		}
	}

	public bool IsWaitingToHide
	{
		get
		{
			return _blockHiding;
		}
	}

	public static event EventHandler<CancellableEventArgs> WantsToHide;

	public static event EventHandler PeekabooStateChanged;

	protected virtual void Start()
	{
		if (peekABoo)
		{
			base.gameObject.layer = Layers.EnemiesDontTarget;
			_peekabooHiddenPosition = base.transform.localPosition;
			_peekabooVisiblePosition = base.transform.localPosition + PeekOutDistance;
		}
	}

	protected IEnumerator PeekabooMoveUp()
	{
		float startMoveTime = Time.time;
		PeekabooState = PeekabooStates.MovingUp;
		for (float percentDoneMove = 0f; percentDoneMove < 1f; percentDoneMove = (Time.time - startMoveTime) / 0.05f)
		{
			base.transform.localPosition = Vector3.Lerp(_peekabooHiddenPosition, _peekabooVisiblePosition, percentDoneMove);
			yield return new WaitForEndOfFrame();
		}
		base.gameObject.layer = Layers.Enemies;
		base.transform.localPosition = _peekabooVisiblePosition;
		PeekabooState = PeekabooStates.Up;
	}

	protected IEnumerator PeekabooMoveDown()
	{
		_blockHiding = false;
		CancellableEventArgs hidingArgs = new CancellableEventArgs();
		OnWantsToHide(hidingArgs);
		_blockHiding = hidingArgs.IsCancelled;
		while (_blockHiding)
		{
			yield return new WaitForEndOfFrame();
		}
		float startMoveTime = Time.time;
		PeekabooState = PeekabooStates.MovingDown;
		for (float percentDoneMove = 0f; percentDoneMove < 1f; percentDoneMove = (Time.time - startMoveTime) / 0.05f)
		{
			base.transform.localPosition = Vector3.Lerp(_peekabooVisiblePosition, _peekabooHiddenPosition, percentDoneMove);
			yield return new WaitForEndOfFrame();
		}
		for (int i = 0; i < ShipManager.instance.shooter.Count; i++)
		{
			ShipManager.instance.shooter[i].targetQueue.RemoveGameObject(base.gameObject);
		}
		base.gameObject.layer = Layers.EnemiesDontTarget;
		PeekabooState = PeekabooStates.Down;
		base.transform.localPosition = _peekabooHiddenPosition;
		_blockHiding = false;
	}

	public void AllowHiding()
	{
		_blockHiding = false;
	}

	public void OnWantsToHide(CancellableEventArgs args)
	{
		if (PeekabooTroll.WantsToHide != null)
		{
			PeekabooTroll.WantsToHide(this, args);
		}
	}

	public void OnPeekabooStateChanged()
	{
		if (PeekabooTroll.PeekabooStateChanged != null)
		{
			PeekabooTroll.PeekabooStateChanged(this, new EventArgs());
		}
	}

	private void OnDrawGizmos()
	{
		if (_blockHiding)
		{
			Gizmos.DrawWireCube(base.transform.position, Vector3.one);
		}
	}
}
