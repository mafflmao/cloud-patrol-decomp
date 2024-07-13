using System;
using UnityEngine;

public class Achievements : ScriptableObject
{
	[Serializable]
	public class AchievementData
	{
		public string id = string.Empty;

		public string name = string.Empty;

		public int stepCount = 1;

		[NonSerialized]
		private int _step;

		[NonSerialized]
		private bool _isDirty;

		public string GameCenterIdentifier
		{
			get
			{
				return ITunesConnectNameManager.GetIdentifierForAchievement(id);
			}
		}

		public int step
		{
			get
			{
				return _step;
			}
			set
			{
				_step = value;
				_isDirty = true;
			}
		}

		public float Progress
		{
			get
			{
				return (_step < stepCount) ? (100f * (float)step / (float)stepCount) : 100f;
			}
		}

		public bool Dirty
		{
			get
			{
				return _isDirty;
			}
		}

		public void LoadProgressFromPersistentStorage()
		{
			_step = PlayerPrefs.GetInt("player.achievement." + id);
		}

		public void SaveProgressToPersistentStorage()
		{
			PlayerPrefs.SetInt("player.achievement." + id, _step);
		}

		public void NotDirty()
		{
			_isDirty = false;
		}
	}

	public AchievementData[] achievements;

	public static readonly string CoinsEarn = ITunesConnectNameManager.GetIdentifierForAchievement("coinsEarn");

	public static readonly string CoinsSpend = ITunesConnectNameManager.GetIdentifierForAchievement("coinsSpend");

	public static readonly string BountyWeek = ITunesConnectNameManager.GetIdentifierForAchievement("bountyWeek");

	public static readonly string BountyElements = ITunesConnectNameManager.GetIdentifierForAchievement("bountyElements");

	public static readonly string ClearSmall = ITunesConnectNameManager.GetIdentifierForAchievement("clearSmall");

	public static readonly string ClearMedium = ITunesConnectNameManager.GetIdentifierForAchievement("clearMedium");

	public static readonly string ClearLarge = ITunesConnectNameManager.GetIdentifierForAchievement("clearLarge");

	public static readonly string ComboNone = ITunesConnectNameManager.GetIdentifierForAchievement("comboNone");

	public static readonly string CoinsMedium = ITunesConnectNameManager.GetIdentifierForAchievement("coinsMedium");

	public static readonly string CoinsLarge = ITunesConnectNameManager.GetIdentifierForAchievement("coinsLarge");

	public static readonly string DieRoom = ITunesConnectNameManager.GetIdentifierForAchievement("dieRoom");

	public static readonly string ComboMissiles = ITunesConnectNameManager.GetIdentifierForAchievement("comboMissiles");

	public static readonly string ViewCredits = ITunesConnectNameManager.GetIdentifierForAchievement("viewCredits");

	public static readonly string BuyMagicUpgrade = ITunesConnectNameManager.GetIdentifierForAchievement("buyMagicUpgrade");

	public static readonly string BuyAll = ITunesConnectNameManager.GetIdentifierForAchievement("buyAll");

	public static readonly string BountySmall = ITunesConnectNameManager.GetIdentifierForAchievement("bountySmall");

	public static readonly string BountyMedium = ITunesConnectNameManager.GetIdentifierForAchievement("bountyMedium");

	public static readonly string BountyLarge = ITunesConnectNameManager.GetIdentifierForAchievement("bountyLarge");

	public static readonly string BossSmall = ITunesConnectNameManager.GetIdentifierForAchievement("bossSmall");

	public static readonly string BossLarge = ITunesConnectNameManager.GetIdentifierForAchievement("bossLarge");

	public static readonly string DestroyObjects = ITunesConnectNameManager.GetIdentifierForAchievement("destroyObjects");

	public static readonly string ShootSheep = ITunesConnectNameManager.GetIdentifierForAchievement("shootSheep");

	public static readonly string ShootMissiles = ITunesConnectNameManager.GetIdentifierForAchievement("shootMissiles");

	public static readonly string ComboMaxSmall = ITunesConnectNameManager.GetIdentifierForAchievement("comboMaxSmall");

	public static readonly string ComboMaxLarge = ITunesConnectNameManager.GetIdentifierForAchievement("comboMaxLarge");

	public static readonly string DieBombs = ITunesConnectNameManager.GetIdentifierForAchievement("dieBombs");

	public static readonly string BountyChange = ITunesConnectNameManager.GetIdentifierForAchievement("bountyChange");

	public static readonly string BountyChangeMore = ITunesConnectNameManager.GetIdentifierForAchievement("bountyChangemore");

	public static readonly string BuyMagic = ITunesConnectNameManager.GetIdentifierForAchievement("buyMagic");

	public static readonly string BuySkylander = ITunesConnectNameManager.GetIdentifierForAchievement("buySkylander");

	public static readonly string BadgeUpgrade = ITunesConnectNameManager.GetIdentifierForAchievement("badgeUpgrade");
}
