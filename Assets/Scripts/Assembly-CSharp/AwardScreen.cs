using System;
using UnityEngine;

public class AwardScreen : ScreenSequenceController
{
	private enum CurrentScreenState
	{
		AwardChallengeMedals = 0,
		AwardGems = 1,
		AwardStars = 2,
		AwardCoins = 3,
		Finishing = 4
	}

	public const float AnimateTime = 0.333f;

	public const float WaitTime = 0.333f;

	private const float bountyOffscreenAmount = 1024f;

	private static ILogger _log = LogBuilder.Instance.GetLogger(typeof(AwardScreen), LogLevel.Warning);

	public ScreenSequenceScreen awardChallengeMedalsPrefab;

	public ScreenSequenceScreen awardGameplayGemsPrefab;

	public ScreenSequenceScreen awardStarsPrefab;

	public ScreenSequenceScreen awardCoinsPrefab;

	public PrefabPlaceholder bountiesPlaceholder;

	public GameObject loadoutScreenPrefab;

	private AwardGameplayGemsScreen _awardGameplayGemsScreen;

	private AwardStarsScreen _awardStarsScreen;

	private AwardCoinsScreen _awardCoinsScreen;

	private BountyController _bountyController;

	public static bool testMode = false;

	public BountyController BountyController;

	public static event EventHandler<EventArgs> ShowBountyController;

	protected override void Start()
	{
		_log.LogDebug("Start()");
		AdvanceToFirstScreenOnStart = false;
		base.Start();
		InvokeHelper.InvokeSafe(AdvanceToNextScreen, 0.5f, this);
	}

	public static void MoveTo(GameObject go, Vector3 pos, float time)
	{
		iTween.MoveTo(go, iTween.Hash("position", pos, "time", time, "easetype", iTween.EaseType.easeInSine));
	}

	public static void MoveFrom(GameObject go, Vector3 pos, float time)
	{
		iTween.MoveFrom(go, iTween.Hash("position", pos, "time", time, "easetype", iTween.EaseType.easeOutExpo));
	}

	public override void AdvanceToNextScreen()
	{
		_log.LogDebug("AdvanceToNextScreen()");
		switch ((CurrentScreenState)base.CurrentScreenNumber)
		{
		case CurrentScreenState.AwardChallengeMedals:
			_log.LogDebug("CurrentScreenState.AwardChallengeMedals");
			base.CurrentScreenNumber = 1;
			goto case CurrentScreenState.AwardGems;
		case CurrentScreenState.AwardGems:
			HeaderUI.Instance.visible = true;
			if (testMode)
			{
				GameManager.gemsCollectedInVoyage = 3;
			}
			if (GameManager.gemsCollectedInVoyage == 0)
			{
				base.CurrentScreenNumber = 2;
				goto case CurrentScreenState.AwardStars;
			}
			SetNextScreenWithAnimateOutAsync(awardGameplayGemsPrefab, 0.333f);
			break;
		case CurrentScreenState.AwardStars:
			if (false || testMode)
			{
				SetNextScreenWithAnimateOutAsync(awardStarsPrefab, 0.333f);
				break;
			}
			AwardStarsScreen.EnsureBountiesInCorrectStateAfterScreen();
			base.CurrentScreenNumber = 3;
			goto case CurrentScreenState.AwardCoins;
		case CurrentScreenState.AwardCoins:
			_log.LogDebug("CurrentScreenState.AwardCoins");
			if ((StartGameSettings.Instance.IsBonusElementActive && GameManager.moneyAwardedForBonus > 0) || testMode)
			{
				_log.LogDebug("EOTD match - showing coin award.");
				SetNextScreenWithAnimateOutAsync(awardCoinsPrefab, 0.333f);
				break;
			}
			_log.LogDebug("EOTD mismatch - skipping coin award.");
			base.CurrentScreenNumber = 4;
			goto case CurrentScreenState.Finishing;
		case CurrentScreenState.Finishing:
			_log.LogDebug("CurrentScreenState.Finishing");
			BountyController.SetBountyButtonsEnabled(true);
			BountyController.SetTabSwitchingEnabled(true);
			BountyController.Reset();
			UIManager.instance.enabled = true;
			OnSequenceComplete(0.333f);
			break;
		}
		base.CurrentScreenNumber++;
	}

	protected void OnShowBountyController()
	{
		if (AwardScreen.ShowBountyController != null)
		{
			AwardScreen.ShowBountyController(this, new EventArgs());
		}
	}
}
