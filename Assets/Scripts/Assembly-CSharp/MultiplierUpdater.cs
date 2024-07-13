using UnityEngine;

public class MultiplierUpdater : MonoBehaviour
{
	public ScoreKeeper scoreKeeper;

	public void CombineMultiplier()
	{
		scoreKeeper.UpdateCombinedMultiplier();
	}
}
