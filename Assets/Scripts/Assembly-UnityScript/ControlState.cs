using System;

[Serializable]
public enum ControlState
{
	WaitingForFirstTouch = 0,
	WaitingForSecondTouch = 1,
	MovingCharacter = 2,
	WaitingForMovement = 3,
	ZoomingCamera = 4,
	RotatingCamera = 5,
	WaitingForNoFingers = 6
}
