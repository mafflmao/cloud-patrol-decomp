public struct PqmtTouch
{
	public FingerGestures.FingerPhase Phase;

	public IntVector2 Position;

	public IntVector2 BeginPosition;

	public IntVector2 EndPosition;

	public int NbTaps;

	public bool PhasePinned;

	public PqmtTouch(FingerGestures.FingerPhase aPhase)
	{
		Phase = aPhase;
		Position = IntVector2.zero;
		BeginPosition = IntVector2.zero;
		EndPosition = IntVector2.zero;
		NbTaps = 0;
		PhasePinned = false;
	}
}
