public class MagicItemAffinityUpgrade : CharacterUpgrade
{
	private const float _baseAffinity = 0.75f;

	private const float _superAffinity = 0.9f;

	public PowerupData powerup;

	public bool evenMoreAffinity;

	public float percentAffinity
	{
		get
		{
			if (evenMoreAffinity)
			{
				return 0.9f;
			}
			return 0.75f;
		}
	}
}
