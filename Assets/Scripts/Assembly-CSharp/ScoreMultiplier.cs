public class ScoreMultiplier : Powerup
{
	protected override void HandleCutsceneComplete()
	{
		ScoreKeeper.Instance.AddPermanentScoreMultiplier(base.PowerupData.level1_scoreModifier);
		DestroyAndFinish(false);
	}
}
