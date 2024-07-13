using UnityEngine;

[AddComponentMenu("FingerGestures/Gesture Recognizers/Tolerant Multi-Finger Drag")]
public class TolerantMultifingerDragGestureRecognizer : AveragedGestureRecognizer
{
	public FingerGestures.IFingerList m_touches;

	public int MinimumFingerCount = 1;

	public int MaximumFingerCount = 5;

	public float MoveTolerance = 5f;

	public bool m_noAverage;

	public Vector2[] m_multiPosition;

	private Vector2 delta = Vector2.zero;

	private Vector2 lastPos = Vector2.zero;

	public Vector2 MoveDelta
	{
		get
		{
			return delta;
		}
		private set
		{
			delta = value;
		}
	}

	protected bool ValidateNbTouchingFingers(int aTouchCount)
	{
		return aTouchCount >= MinimumFingerCount && aTouchCount <= MaximumFingerCount;
	}

	protected int GetNbFingersDown(FingerGestures.IFingerList aTouches)
	{
		int num = 0;
		for (int i = 0; i < aTouches.Count; i++)
		{
			if (aTouches[i].IsDown)
			{
				num++;
			}
		}
		return num;
	}

	protected override bool CanBegin(FingerGestures.IFingerList touches)
	{
		if (!base.CanBegin(touches) && !ValidateNbTouchingFingers(GetNbFingersDown(touches)))
		{
			return false;
		}
		if (touches.GetAverageDistanceFromStart() < MoveTolerance)
		{
			return false;
		}
		return true;
	}

	protected override void OnBegin(FingerGestures.IFingerList touches)
	{
		m_touches = touches;
		if (m_noAverage)
		{
			m_multiPosition = new Vector2[touches.Count];
			for (int i = 0; i < touches.Count; i++)
			{
				m_multiPosition[i] = touches[i].Position;
			}
		}
		base.Position = touches.GetAveragePosition();
		base.StartPosition = base.Position;
		MoveDelta = Vector2.zero;
		lastPos = base.Position;
	}

	protected override GestureState OnActive(FingerGestures.IFingerList touches)
	{
		m_touches = touches;
		int nbFingersDown = GetNbFingersDown(touches);
		if (!ValidateNbTouchingFingers(nbFingersDown))
		{
			if (nbFingersDown < MinimumFingerCount)
			{
				return GestureState.Recognized;
			}
			return GestureState.Failed;
		}
		base.Position = touches.GetAveragePosition();
		MoveDelta = base.Position - lastPos;
		if (MoveDelta.sqrMagnitude > 0f)
		{
			lastPos = base.Position;
		}
		return GestureState.InProgress;
	}
}
