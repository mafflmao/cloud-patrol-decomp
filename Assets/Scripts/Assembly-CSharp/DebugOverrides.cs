using UnityEngine;

public class DebugOverrides : MonoBehaviour
{
	public bool forceMagicItemSpawn;

	public PowerupData magicItemToSpawn;

	public bool forcePresentSpawn;

	public bool forcePresentType;

	public PresentBoxRewardTypes presentType;

	public bool showDummyChallengeMedalAwards;

	public bool resetAwardedGems;

	public bool forceSaleIcons;

	public string levelOverride;

	public bool forceOneEnemyInBossRooms;

	public bool useBountyOverrides;

	public int[] bountyOverrides;

	private void Awake()
	{
		if (Application.isEditor)
		{
			DebugSettingsUI.forceMagicItemSpawn = forceMagicItemSpawn;
			DebugSettingsUI.magicItemToSpawn = magicItemToSpawn.name;
			DebugSettingsUI.forcePresentSpawn = forcePresentSpawn;
			DebugSettingsUI.forcePresentType = ((!forcePresentType) ? null : new PresentBoxRewardTypes?(presentType));
			DebugSettingsUI.forceShowChallengeMedalAwards = showDummyChallengeMedalAwards;
			DebugSettingsUI.forceSaleIcons = forceSaleIcons;
			DebugSettingsUI.forceOneEnemyInBossRooms = forceOneEnemyInBossRooms;
			if (resetAwardedGems)
			{
			}
			if (useBountyOverrides)
			{
				BountyChooser.Instance.SetBounty(0, BountyChooser.Instance.GetBountyDataForId(bountyOverrides[0]));
				BountyChooser.Instance.SetBounty(1, BountyChooser.Instance.GetBountyDataForId(bountyOverrides[1]));
				BountyChooser.Instance.SetBounty(2, BountyChooser.Instance.GetBountyDataForId(bountyOverrides[2]));
			}
		}
	}

	private void OnEnable()
	{
		LevelManager.RoomClear += HandleLevelManagerRoomClear;
	}

	private void OnDisable()
	{
		LevelManager.RoomClear -= HandleLevelManagerRoomClear;
	}

	private void HandleLevelManagerRoomClear(object sender, LevelManager.RoomClearEventArgs e)
	{
		if (!string.IsNullOrEmpty(levelOverride))
		{
			LevelManager.Instance.levelOverride = levelOverride;
			levelOverride = string.Empty;
		}
	}
}
