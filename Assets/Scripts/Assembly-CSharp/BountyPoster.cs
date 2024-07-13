using UnityEngine;

public class BountyPoster : MonoBehaviour
{
	public SpriteText txtTitle;

	public PackedSprite background;

	public PackedSprite completedStamp;

	public bool hideCompletedStampOnStart;

	public bool hideAtStart;

	private bool _isStarted;

	public PackedSprite[] stars;

	private Bounty _bounty;

	public Bounty Bounty
	{
		get
		{
			return _bounty;
		}
		set
		{
			_bounty = value;
			UpdateGraphic();
		}
	}

	private void Start()
	{
		_isStarted = true;
		if (Bounty != null)
		{
			UpdateGraphic();
		}
		Hide(hideAtStart);
	}

	private void UpdateGraphic()
	{
		if (!_isStarted || _bounty == null)
		{
			return;
		}
		completedStamp.Hide(!_bounty.IsComplete);
		if (hideCompletedStampOnStart)
		{
			completedStamp.GetComponent<Renderer>().enabled = false;
		}
		if (Bounty.bountyData.Reward == 1)
		{
			stars[0].Hide(true);
			stars[1].Hide(true);
			stars[2].Hide(false);
			stars[3].Hide(true);
			stars[4].Hide(true);
		}
		else if (Bounty.bountyData.Reward == 2)
		{
			stars[0].Hide(true);
			stars[1].Hide(false);
			stars[2].Hide(true);
			stars[3].Hide(false);
			stars[4].Hide(true);
		}
		else
		{
			stars[0].Hide(false);
			stars[1].Hide(true);
			stars[2].Hide(false);
			stars[3].Hide(true);
			stars[4].Hide(false);
		}
		string text = "Active";
		if (Bounty.IsComplete)
		{
			text = "Inactive";
		}
		for (int i = 0; i < stars.Length; i++)
		{
			if (stars[i].GetAnim(text) != null)
			{
				stars[i].PlayAnim(text);
			}
		}
	}

	public void ActivateStar(int starNumber)
	{
		int num = 0;
		PackedSprite[] array = stars;
		foreach (PackedSprite packedSprite in array)
		{
			if (!packedSprite.IsHidden())
			{
				if (num == starNumber)
				{
					packedSprite.PlayAnim("Active");
				}
				num++;
			}
		}
	}

	public void SetCompletedStampVisibility(bool state)
	{
		completedStamp.GetComponent<Renderer>().enabled = state;
	}

	public void Hide(bool shouldHide)
	{
		background.Hide(shouldHide);
		bool tf = shouldHide;
		if (!hideCompletedStampOnStart)
		{
			if (_bounty != null && !shouldHide)
			{
				tf = !_bounty.IsComplete;
			}
			completedStamp.Hide(tf);
		}
	}
}
