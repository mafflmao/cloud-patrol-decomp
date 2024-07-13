using System.Collections;
using UnityEngine;

public class AwardStarsScreen : ScreenSequenceScreen
{
	private const float offscreenAmount = 1324f;

	private const float OnboardingScreenZOffset = -80f;

	public BountyStarContainer starContainer;

	public BountyController bountyController;

	public GameObject awardStarPrefab;

	public SpriteText levelNumberText;

	public AwardStarsLevelUpScreen levelUpScreenPrefab;

	public SoundEventData uiSlideRightSFX;

	public SoundEventData uiSlideLeftSFX;

	public GameObject starFX;

	public GameObject badge;

	public Color highLevelBadgeColor = new Color(0.89f, 0.89f, 0.38f, 1f);

	private bool _allowSkipping;

	public SoundEventData[] starRewardSounds;

	private GameObject _animatingStar;

	protected override void Start()
	{
		base.Start();
		RankAndStars currentRank = RankDataManager.Instance.CurrentRank;
		SetRankDisplay(currentRank);
		_allowSkipping = true;
		StartCoroutine(AwardSequence());
	}

	private void OnDestroy()
	{
		EnsureBountiesInCorrectStateAfterScreen();
	}

	public static void EnsureBountiesInCorrectStateAfterScreen()
	{
	}

	protected override void AnimateIn()
	{
		bountyController = ((AwardScreen)base.Owner).BountyController;
		if (bountyController == null)
		{
			Debug.LogError("Unable to get bounty controll from award screen manager. Is it set yet?");
		}
		AwardScreen.MoveFrom(base.gameObject, new Vector3(base.gameObject.transform.position.x + 1324f, base.gameObject.transform.position.y, base.gameObject.transform.position.z), 0.333f);
		SoundEventManager.Instance.Play(uiSlideRightSFX, base.gameObject);
	}

	protected override void AnimateOut()
	{
		StopAllCoroutines();
		if (_animatingStar != null)
		{
			Object.Destroy(_animatingStar);
		}
		AwardScreen.MoveTo(base.gameObject, new Vector3(base.gameObject.transform.position.x + 1324f, base.gameObject.transform.position.y, base.gameObject.transform.position.z), 0.333f);
		SoundEventManager.Instance.Play(uiSlideRightSFX, base.gameObject);
		for (int i = 0; i < 3; i++)
		{
			BountyBar bountyBar = bountyController.GetBountyBar(i);
			if (bountyBar.Bounty.IsComplete)
			{
				RankDataManager.Instance.IncreaseStars(bountyBar.Bounty.bountyData.Reward);
				bountyBar.SwitchBounty(-1);
			}
			else if (!bountyBar.Bounty.RememberProgress)
			{
				bountyBar.Bounty.ResetProgress();
			}
		}
		Suicide(0.333f);
	}

	protected IEnumerator AwardSequence()
	{
		yield return new WaitForSeconds(0.333f);
		yield return new WaitForSeconds(0.333f);
		int bountyCompleteSound = 0;
		for (int bountyNumber = 0; bountyNumber < BountyChooser.Instance.ActiveBounties.Length; bountyNumber++)
		{
			Bounty bounty = BountyChooser.Instance.ActiveBounties[bountyNumber];
			if (!bounty.IsComplete)
			{
				continue;
			}
			BountyBar bountyBar = bountyController.GetBountyBar(bountyNumber);
			PackedSprite[] bountyBarStarSprites = bountyBar.GetEnabledStars();
			for (int bountyBarStarNumber = 0; bountyBarStarNumber < bounty.bountyData.Reward; bountyBarStarNumber++)
			{
				RankAndStars currentRankAndStars3 = RankDataManager.Instance.CurrentRank;
				PackedSprite bountyBarStarSprite = bountyBarStarSprites[bountyBarStarNumber];
				int playerStarsInLevelBeforeAward = currentRankAndStars3.Stars;
				Vector3 starStartPosition = bountyBarStarSprite.gameObject.transform.position;
				Vector3 starEndPosition = starContainer.GetStar(playerStarsInLevelBeforeAward).transform.position;
				bountyBarStarSprite.Hide(true);
				yield return StartCoroutine(MoveStar(starStartPosition, starEndPosition, bountyBarStarNumber));
				starContainer.SetStarsEnabled(playerStarsInLevelBeforeAward + 1);
				if (RankDataManager.Instance.IsFinalRank(currentRankAndStars3.Rank) && currentRankAndStars3.Stars == currentRankAndStars3.Rank.StarsForNextRank - 1)
				{
					RankDataManager.Instance.IncreaseStars(1);
					currentRankAndStars3 = RankDataManager.Instance.CurrentRank;
					yield return StartCoroutine(PlayLevelUpSequence(SwrveEconomy.RankGemsAwarded));
					SetRankDisplay(currentRankAndStars3);
					continue;
				}
				RankAndStars previousRankAndStars = currentRankAndStars3;
				RankDataManager.Instance.IncreaseStars(1);
				currentRankAndStars3 = RankDataManager.Instance.CurrentRank;
				if (previousRankAndStars.Rank.RankNumber != currentRankAndStars3.Rank.RankNumber)
				{
					yield return StartCoroutine(PlayLevelUpSequence(SwrveEconomy.RankGemsAwarded));
					SetRankDisplay(currentRankAndStars3);
				}
			}
			yield return new WaitForSeconds(0.666f);
			bountyBar.SwitchBounty(bountyCompleteSound);
			bountyCompleteSound++;
			yield return new WaitForSeconds(0.666f);
		}
		_allowSkipping = true;
		StartTimeout(0.333f);
	}

	private void SetRankDisplay(RankAndStars rankAndStarData)
	{
		starContainer.SetNumberOfStars(rankAndStarData.Rank.StarsForNextRank);
		starContainer.SetStarsEnabled(rankAndStarData.Stars);
		levelNumberText.Text = rankAndStarData.Rank.RankNumber.ToString();
		if (rankAndStarData.Rank.RankNumber > 50)
		{
			badge.GetComponent<Renderer>().material.color = highLevelBadgeColor;
		}
	}

	private IEnumerator MoveStar(Vector3 startPosition, Vector3 endPosition, int starNumber)
	{
		_animatingStar = (GameObject)Object.Instantiate(awardStarPrefab);
		_animatingStar.GetComponent<TrailRenderer>().enabled = true;
		BountyStar bountyStarScript = _animatingStar.GetComponent<BountyStar>();
		bountyStarScript.IsEnabled = true;
		_animatingStar.transform.position = startPosition;
		Vector3 midPoint = startPosition + (endPosition - startPosition) / 2f;
		midPoint.y += (endPosition - startPosition).magnitude * 0.2f;
		float moveTime = 0.222f;
		Vector3[] path = new Vector3[3] { startPosition, midPoint, endPosition };
		iTween.MoveTo(args: iTween.Hash("position", endPosition, "path", path, "easeType", iTween.EaseType.easeInSine, "movetopath", false, "time", moveTime), target: _animatingStar);
		iTween.RotateBy(_animatingStar, new Vector3(0f, 0f, 3f), moveTime);
		yield return new WaitForSeconds(moveTime);
		GameObject fxInstance = (GameObject)Object.Instantiate(starFX, endPosition, Quaternion.identity);
		fxInstance.transform.parent = starContainer.transform;
		if (starNumber <= starRewardSounds.Length)
		{
			SoundEventManager.Instance.Play(starRewardSounds[starNumber], _animatingStar);
		}
		else
		{
			SoundEventManager.Instance.Play(starRewardSounds[starRewardSounds.Length - 1], _animatingStar);
		}
		yield return new WaitForSeconds(0.111f);
		Object.Destroy(_animatingStar, 0.333f);
	}

	private IEnumerator PlayLevelUpSequence(int gemsToAward)
	{
		_allowSkipping = false;
		GameObject levelUpScreenInstance = (GameObject)Object.Instantiate(levelUpScreenPrefab.gameObject);
		levelUpScreenInstance.transform.parent = base.transform;
		AwardStarsLevelUpScreen levelUpScreen = levelUpScreenInstance.GetComponent<AwardStarsLevelUpScreen>();
		yield return StartCoroutine(levelUpScreen.AnimateIn(gemsToAward));
		Object.Destroy(levelUpScreenInstance);
		_allowSkipping = true;
	}

	public override bool AllowAdvanceToNextScreenFromUserPress()
	{
		return _allowSkipping;
	}
}
