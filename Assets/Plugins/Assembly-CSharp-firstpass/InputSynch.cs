using PQ_SDK_MultiTouch;
using UnityEngine;

public static class InputSynch
{
	private static object mLock = new object();

	private static PqmtTouch[] mPqmtTouches;

	private static PqmtTouch[] mFixedPqmtTouches;

	private static int mUnityFrameCount = 0;

	private static int mMaxTouches;

	public static PqmtTouch[] Touches
	{
		get
		{
			lock (mLock)
			{
				return mFixedPqmtTouches;
			}
		}
	}

	public static void Initialize(int aMaxTouches)
	{
		mMaxTouches = aMaxTouches;
		mPqmtTouches = new PqmtTouch[aMaxTouches];
		mFixedPqmtTouches = new PqmtTouch[aMaxTouches];
	}

	public static void FixPqmtInputs()
	{
		lock (mLock)
		{
			if (mUnityFrameCount == Time.frameCount)
			{
				return;
			}
			mUnityFrameCount = Time.frameCount;
			for (int i = 0; i < mMaxTouches; i++)
			{
				mFixedPqmtTouches[i] = mPqmtTouches[i];
				mPqmtTouches[i].PhasePinned = false;
				mPqmtTouches[i].NbTaps = 0;
				switch (mPqmtTouches[i].Phase)
				{
				case FingerGestures.FingerPhase.Began:
					mFixedPqmtTouches[i].Position = mFixedPqmtTouches[i].BeginPosition;
					if (mFixedPqmtTouches[i].NbTaps > 0)
					{
						mPqmtTouches[i].Phase = FingerGestures.FingerPhase.Ended;
						mPqmtTouches[i].PhasePinned = true;
					}
					else if (mFixedPqmtTouches[i].BeginPosition != mFixedPqmtTouches[i].Position)
					{
						mPqmtTouches[i].Phase = FingerGestures.FingerPhase.Moved;
					}
					else
					{
						mPqmtTouches[i].Phase = FingerGestures.FingerPhase.Stationary;
					}
					mPqmtTouches[i].BeginPosition = IntVector2.zero;
					break;
				case FingerGestures.FingerPhase.Moved:
					mPqmtTouches[i].Phase = FingerGestures.FingerPhase.Stationary;
					break;
				case FingerGestures.FingerPhase.Ended:
					mFixedPqmtTouches[i].Position = mFixedPqmtTouches[i].EndPosition;
					if (mFixedPqmtTouches[i].NbTaps < 0)
					{
						mPqmtTouches[i].Phase = FingerGestures.FingerPhase.Began;
						mPqmtTouches[i].PhasePinned = true;
					}
					else
					{
						mPqmtTouches[i].Phase = FingerGestures.FingerPhase.None;
					}
					break;
				}
				mFixedPqmtTouches[i].PhasePinned = false;
				mFixedPqmtTouches[i].NbTaps = 0;
			}
		}
	}

	public static void ProcessTouchPoints(PQMTClientImport.TouchPoint[] aTouchPoints)
	{
		lock (mLock)
		{
			bool[] array = new bool[mPqmtTouches.Length];
			for (int i = 0; i < aTouchPoints.Length; i++)
			{
				int id = aTouchPoints[i].id;
				if (id >= mMaxTouches)
				{
					continue;
				}
				IntVector2 intVector = new IntVector2(aTouchPoints[i].x, aTouchPoints[i].y);
				array[id] = true;
				switch ((PQMTClientImport.EnumPQTouchPointType)aTouchPoints[i].point_event)
				{
				case PQMTClientImport.EnumPQTouchPointType.TP_DOWN:
					switch (mPqmtTouches[id].Phase)
					{
					case FingerGestures.FingerPhase.Ended:
						mPqmtTouches[id].NbTaps--;
						if (!mPqmtTouches[id].PhasePinned)
						{
							if (mPqmtTouches[id].Position != intVector)
							{
								mPqmtTouches[id].Phase = FingerGestures.FingerPhase.Moved;
							}
							else
							{
								mPqmtTouches[id].Phase = FingerGestures.FingerPhase.Stationary;
							}
						}
						mPqmtTouches[id].BeginPosition = intVector;
						break;
					default:
						mPqmtTouches[id].Phase = FingerGestures.FingerPhase.Began;
						mPqmtTouches[id].BeginPosition = intVector;
						break;
					case FingerGestures.FingerPhase.Began:
					case FingerGestures.FingerPhase.Moved:
					case FingerGestures.FingerPhase.Stationary:
						break;
					}
					mPqmtTouches[id].Position = intVector;
					break;
				case PQMTClientImport.EnumPQTouchPointType.TP_MOVE:
					switch (mPqmtTouches[id].Phase)
					{
					case FingerGestures.FingerPhase.Stationary:
						if (mPqmtTouches[id].Position != intVector)
						{
							mPqmtTouches[id].Phase = FingerGestures.FingerPhase.Moved;
						}
						break;
					}
					mPqmtTouches[id].Position = intVector;
					break;
				case PQMTClientImport.EnumPQTouchPointType.TP_UP:
					switch (mPqmtTouches[id].Phase)
					{
					case FingerGestures.FingerPhase.Began:
						mPqmtTouches[id].NbTaps++;
						if (!mPqmtTouches[id].PhasePinned)
						{
							mPqmtTouches[id].Phase = FingerGestures.FingerPhase.None;
						}
						mPqmtTouches[id].EndPosition = intVector;
						break;
					case FingerGestures.FingerPhase.Moved:
						mPqmtTouches[id].Phase = FingerGestures.FingerPhase.Ended;
						mPqmtTouches[id].EndPosition = intVector;
						break;
					case FingerGestures.FingerPhase.Stationary:
						mPqmtTouches[id].Phase = FingerGestures.FingerPhase.Ended;
						mPqmtTouches[id].EndPosition = intVector;
						break;
					}
					mPqmtTouches[id].Position = intVector;
					break;
				}
			}
			int num = mPqmtTouches.Length;
			for (int j = 0; j < num; j++)
			{
				if (mPqmtTouches[j].Phase != 0 && !array[j])
				{
					mPqmtTouches[j].Phase = FingerGestures.FingerPhase.Ended;
				}
			}
		}
	}
}
