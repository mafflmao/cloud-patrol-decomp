public class GlobalSoundEventData : SingletonMonoBehaviour
{
	public SoundEventData UI_Scene_Clear;

	public SoundEventData UI_Scene_Transition_Start;

	public SoundEventData UI_Scene_Transition_End;

	public SoundEventData Coin_SFX_Burst_small;

	public SoundEventData Coin_SFX_Burst_medium;

	public SoundEventData Coin_SFX_Burst_large;

	public SoundEventData Boss_Intro_Stinger1;

	public SoundEventData Boss_Outro_Stinger1;

	public SoundEventData Boss_Outro_Stinger2;

	public SoundEventData Boss_Coin_Fountain;

	public SoundEventData Boss_Intro_SFX_Tally;

	public SoundEventData Boss_Outro_Explode;

	public SoundEventData Boss_Castle_Explode;

	public SoundEventData Boss_Warning_SFX;

	public SoundEventData Bonus_Intro_Tally_Total;

	public SoundEventData Bonus_Results_Neutral;

	public SoundEventData Bonus_Results_Perfect;

	public SoundEventData Bonus_Intro_Stinger;

	public SoundEventData Bonus_Outro_Stinger;

	public SoundEventData PlayerTakeHit_VO;

	public SoundEventData PlayerTakeHit_SFX;

	public SoundEventData EnemyPoof;

	public SoundEventData PauseSound;

	public SoundEventData UnPauseSound;

	public static GlobalSoundEventData Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<GlobalSoundEventData>();
		}
	}
}
