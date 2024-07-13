using System;

public class DestructibleScoreBonusUpgrade : CharacterUpgrade
{
	public Destructible.Type _destructibleType;

	private void OnEnable()
	{
		Health.Killed += HandleHealthKilled;
	}

	private void OnDisable()
	{
		Health.Killed -= HandleHealthKilled;
	}

	private void HandleHealthKilled(object sender, EventArgs args)
	{
		Health health = (Health)sender;
		if (IsDestructibleModifier.IsHealthAttachedToDestructible(health, _destructibleType))
		{
			ScoreKeeper.Instance.AddScore(ScoreData.ScoreType.DESTRUCTIBLE_BONUS, health.transform.position, true);
		}
	}
}
