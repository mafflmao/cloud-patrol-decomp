using System;
using UnityEngine;

[Serializable]
public class JavascriptSkeleton : MonoBehaviour
{
	public virtual void OnEnable()
	{
		FingerGestures.OnFingerDown += FingerGestures_OnFingerDown;
		FingerGestures.OnFingerStationaryBegin += FingerGestures_OnFingerStationaryBegin;
		FingerGestures.OnFingerStationary += FingerGestures_OnFingerStationary;
		FingerGestures.OnFingerStationaryEnd += FingerGestures_OnFingerStationaryEnd;
		FingerGestures.OnFingerMoveBegin += FingerGestures_OnFingerMoveBegin;
		FingerGestures.OnFingerMove += FingerGestures_OnFingerMove;
		FingerGestures.OnFingerMoveEnd += FingerGestures_OnFingerMoveEnd;
		FingerGestures.OnFingerUp += FingerGestures_OnFingerUp;
		FingerGestures.OnFingerLongPress += FingerGestures_OnFingerLongPress;
		FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
		FingerGestures.OnFingerDoubleTap += FingerGestures_OnFingerDoubleTap;
		FingerGestures.OnFingerSwipe += FingerGestures_OnFingerSwipe;
		FingerGestures.OnFingerDragBegin += FingerGestures_OnFingerDragBegin;
		FingerGestures.OnFingerDragMove += FingerGestures_OnFingerDragMove;
		FingerGestures.OnFingerDragEnd += FingerGestures_OnFingerDragEnd;
		FingerGestures.OnLongPress += FingerGestures_OnLongPress;
		FingerGestures.OnTap += FingerGestures_OnTap;
		FingerGestures.OnDoubleTap += FingerGestures_OnDoubleTap;
		FingerGestures.OnSwipe += FingerGestures_OnSwipe;
		FingerGestures.OnDragBegin += FingerGestures_OnDragBegin;
		FingerGestures.OnDragMove += FingerGestures_OnDragMove;
		FingerGestures.OnDragEnd += FingerGestures_OnDragEnd;
		FingerGestures.OnPinchBegin += FingerGestures_OnPinchBegin;
		FingerGestures.OnPinchMove += FingerGestures_OnPinchMove;
		FingerGestures.OnPinchEnd += FingerGestures_OnPinchEnd;
		FingerGestures.OnRotationBegin += FingerGestures_OnRotationBegin;
		FingerGestures.OnRotationMove += FingerGestures_OnRotationMove;
		FingerGestures.OnRotationEnd += FingerGestures_OnRotationEnd;
		FingerGestures.OnTwoFingerLongPress += FingerGestures_OnTwoFingerLongPress;
		FingerGestures.OnTwoFingerTap += FingerGestures_OnTwoFingerTap;
		FingerGestures.OnTwoFingerSwipe += FingerGestures_OnTwoFingerSwipe;
		FingerGestures.OnTwoFingerDragBegin += FingerGestures_OnTwoFingerDragBegin;
		FingerGestures.OnTwoFingerDragMove += FingerGestures_OnTwoFingerDragMove;
		FingerGestures.OnTwoFingerDragEnd += FingerGestures_OnTwoFingerDragEnd;
	}

	public virtual void OnDisable()
	{
		FingerGestures.OnFingerDown -= FingerGestures_OnFingerDown;
		FingerGestures.OnFingerStationaryBegin -= FingerGestures_OnFingerStationaryBegin;
		FingerGestures.OnFingerStationary -= FingerGestures_OnFingerStationary;
		FingerGestures.OnFingerStationaryEnd -= FingerGestures_OnFingerStationaryEnd;
		FingerGestures.OnFingerMoveBegin -= FingerGestures_OnFingerMoveBegin;
		FingerGestures.OnFingerMove -= FingerGestures_OnFingerMove;
		FingerGestures.OnFingerMoveEnd -= FingerGestures_OnFingerMoveEnd;
		FingerGestures.OnFingerUp -= FingerGestures_OnFingerUp;
		FingerGestures.OnFingerLongPress -= FingerGestures_OnFingerLongPress;
		FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
		FingerGestures.OnFingerDoubleTap -= FingerGestures_OnFingerDoubleTap;
		FingerGestures.OnFingerSwipe -= FingerGestures_OnFingerSwipe;
		FingerGestures.OnFingerDragBegin -= FingerGestures_OnFingerDragBegin;
		FingerGestures.OnFingerDragMove -= FingerGestures_OnFingerDragMove;
		FingerGestures.OnFingerDragEnd -= FingerGestures_OnFingerDragEnd;
		FingerGestures.OnLongPress -= FingerGestures_OnLongPress;
		FingerGestures.OnTap -= FingerGestures_OnTap;
		FingerGestures.OnDoubleTap -= FingerGestures_OnDoubleTap;
		FingerGestures.OnSwipe -= FingerGestures_OnSwipe;
		FingerGestures.OnDragBegin -= FingerGestures_OnDragBegin;
		FingerGestures.OnDragMove -= FingerGestures_OnDragMove;
		FingerGestures.OnDragEnd -= FingerGestures_OnDragEnd;
		FingerGestures.OnPinchBegin -= FingerGestures_OnPinchBegin;
		FingerGestures.OnPinchMove -= FingerGestures_OnPinchMove;
		FingerGestures.OnPinchEnd -= FingerGestures_OnPinchEnd;
		FingerGestures.OnRotationBegin -= FingerGestures_OnRotationBegin;
		FingerGestures.OnRotationMove -= FingerGestures_OnRotationMove;
		FingerGestures.OnRotationEnd -= FingerGestures_OnRotationEnd;
		FingerGestures.OnTwoFingerLongPress -= FingerGestures_OnTwoFingerLongPress;
		FingerGestures.OnTwoFingerTap -= FingerGestures_OnTwoFingerTap;
		FingerGestures.OnTwoFingerSwipe -= FingerGestures_OnTwoFingerSwipe;
		FingerGestures.OnTwoFingerDragBegin -= FingerGestures_OnTwoFingerDragBegin;
		FingerGestures.OnTwoFingerDragMove -= FingerGestures_OnTwoFingerDragMove;
		FingerGestures.OnTwoFingerDragEnd -= FingerGestures_OnTwoFingerDragEnd;
	}

	public virtual void FingerGestures_OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
	{
	}

	public virtual void FingerGestures_OnFingerMoveBegin(int fingerIndex, Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnFingerMove(int fingerIndex, Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnFingerMoveEnd(int fingerIndex, Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnFingerStationaryBegin(int fingerIndex, Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnFingerStationary(int fingerIndex, Vector2 fingerPos, float elapsedTime)
	{
	}

	public virtual void FingerGestures_OnFingerStationaryEnd(int fingerIndex, Vector2 fingerPos, float elapsedTime)
	{
	}

	public virtual void FingerGestures_OnFingerLongPress(int fingerIndex, Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnFingerDoubleTap(int fingerIndex, Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnFingerSwipe(int fingerIndex, Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity)
	{
	}

	public virtual void FingerGestures_OnFingerDragBegin(int fingerIndex, Vector2 fingerPos, Vector2 startPos)
	{
	}

	public virtual void FingerGestures_OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
	}

	public virtual void FingerGestures_OnFingerDragEnd(int fingerIndex, Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnLongPress(Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnTap(Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnDoubleTap(Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity)
	{
	}

	public virtual void FingerGestures_OnDragBegin(Vector2 fingerPos, Vector2 startPos)
	{
	}

	public virtual void FingerGestures_OnDragMove(Vector2 fingerPos, Vector2 delta)
	{
	}

	public virtual void FingerGestures_OnDragEnd(Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnPinchBegin(Vector2 fingerPos1, Vector2 fingerPos2)
	{
	}

	public virtual void FingerGestures_OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
	{
	}

	public virtual void FingerGestures_OnPinchEnd(Vector2 fingerPos1, Vector2 fingerPos2)
	{
	}

	public virtual void FingerGestures_OnRotationBegin(Vector2 fingerPos1, Vector2 fingerPos2)
	{
	}

	public virtual void FingerGestures_OnRotationMove(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta)
	{
	}

	public virtual void FingerGestures_OnRotationEnd(Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle)
	{
	}

	public virtual void FingerGestures_OnTwoFingerLongPress(Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnTwoFingerTap(Vector2 fingerPos)
	{
	}

	public virtual void FingerGestures_OnTwoFingerSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity)
	{
	}

	public virtual void FingerGestures_OnTwoFingerDragBegin(Vector2 fingerPos, Vector2 startPos)
	{
	}

	public virtual void FingerGestures_OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta)
	{
	}

	public virtual void FingerGestures_OnTwoFingerDragEnd(Vector2 fingerPos)
	{
	}

	public virtual void Main()
	{
	}
}
