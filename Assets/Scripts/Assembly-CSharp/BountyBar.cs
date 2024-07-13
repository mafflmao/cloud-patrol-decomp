using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BountyBar : MonoBehaviour
{
	public SpriteText descSpriteText;

	public PackedSprite[] oneBountyStars;

	public PackedSprite[] twoBountyStars;

	public PackedSprite[] threeBountyStars;

	public SimpleSprite icon;

	[HideInInspector]
	public int index;

	public UIButton3D _button;

	public UIButtonComposite skipButton;

	public GameObject skipButtonRoot;

	public SpriteText skipButtonText;

	public PackedSprite skipButtonCoinIcon;

	public SpriteText newText;

	public SoundEventData bountySelectSFX;

	public SoundEventData bountySkipYesSFX;

	public SoundEventData bountySkipNoSFX;

	public SoundEventData bountyCompleteFirstSFX;

	public SoundEventData bountyCompleteSecondSFX;

	public SoundEventData bountyCompleteThirdSFX;

	private bool _skipButtonVisible;

	private bool _hidingSkipButton;

	private Material _iconMat;

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

	public bool ButtonEnabled
	{
		get
		{
			return base.gameObject.GetComponent<Collider>().enabled;
		}
		set
		{
			_button.gameObject.GetComponent<Collider>().enabled = value;
		}
	}

	private void Start()
	{
		HideSkipButtonAssets(true);
		if (Bounty != null)
		{
			UpdateGraphic();
		}
	}

	private void OnEnable()
	{
		BountyChooser.BountyChanged += HandleBountyChooserBountyChanged;
		Bounty.BountyProgressUpdated += HandleBountyBountyProgressUpdated;
	}

	private void OnDisable()
	{
		BountyChooser.BountyChanged -= HandleBountyChooserBountyChanged;
		Bounty.BountyProgressUpdated -= HandleBountyBountyProgressUpdated;
	}

	private void HandleBountyChooserBountyChanged(object sender, BountyChangeEventArgs e)
	{
		if (e.BountyNumber == index)
		{
			Bounty = e.NewBounty;
		}
	}

	private void HandleBountyBountyProgressUpdated(object sender, EventArgs e)
	{
		UpdateGraphic();
	}

	private void UpdateGraphic()
	{
		if (Bounty != null)
		{
			if (_iconMat == null)
			{
				_iconMat = icon.gameObject.GetComponent<Renderer>().material;
			}
			UpdateVisibleStars();
			descSpriteText.Text = _bounty.GetFormattedStoreDescription();
			if (_bounty.iconTexture != null)
			{
				icon.Hide(false);
				_iconMat.SetTexture("_MainTex", _bounty.iconTexture);
			}
			else
			{
				icon.Hide(true);
			}
			newText.Hide(true);
		}
	}

	private void UpdateVisibleStars()
	{
		foreach (PackedSprite item in oneBountyStars.Concat(twoBountyStars).Concat(threeBountyStars))
		{
			item.Hide(true);
		}
		PackedSprite[] array = null;
		switch (_bounty.bountyData.Reward)
		{
		case 1:
			array = oneBountyStars;
			break;
		case 2:
			array = twoBountyStars;
			break;
		case 3:
			array = threeBountyStars;
			break;
		}
		if (array != null)
		{
			PackedSprite[] array2 = array;
			foreach (PackedSprite packedSprite in array2)
			{
				packedSprite.Hide(false);
			}
		}
	}

	public PackedSprite[] GetEnabledStars()
	{
		PackedSprite[] source = null;
		switch (_bounty.bountyData.Reward)
		{
		case 1:
			source = oneBountyStars;
			break;
		case 2:
			source = twoBountyStars;
			break;
		case 3:
			source = threeBountyStars;
			break;
		}
		return source.Where((PackedSprite sprite) => !sprite.IsHidden()).ToArray();
	}

	public void OnClick()
	{
		ShowSkipButton();
		SwrveEventsUI.GoalButtonTouched(Bounty.bountyData.Id);
	}

	public void SwitchBounty(int soundToPlay)
	{
		ButtonEnabled = false;
		iTween.MoveAdd(base.gameObject, iTween.Hash("x", -1100f, "time", 0.222f, "easetype", iTween.EaseType.easeInQuad, "oncompletetarget", base.gameObject, "oncomplete", "OnMoveOutComplete"));
		switch (soundToPlay)
		{
		case 0:
			SoundEventManager.Instance.Play2D(bountyCompleteFirstSFX);
			break;
		case 1:
			SoundEventManager.Instance.Play2D(bountyCompleteSecondSFX);
			break;
		case 2:
			SoundEventManager.Instance.Play2D(bountyCompleteThirdSFX);
			break;
		}
		UIManager.instance.blockInput = true;
	}

	public void SkipBounty()
	{
		ButtonEnabled = false;
		iTween.ScaleTo(base.gameObject, iTween.Hash("y", 0f, "time", 0.222f, "easetype", iTween.EaseType.easeInQuad, "oncompletetarget", base.gameObject, "oncomplete", "OnSkipComplete"));
		SoundEventManager.Instance.Play2D(bountySkipYesSFX);
		UIManager.instance.blockInput = true;
	}

	public void OnMoveOutComplete()
	{
		BountyChooser.Instance.ChooseNewBounty(index);
		iTween.MoveAdd(base.gameObject, iTween.Hash("x", 1100f, "time", 0.222f, "easetype", iTween.EaseType.easeOutQuad, "oncompletetarget", base.gameObject, "oncomplete", "OnMoveInComplete"));
		newText.Hide(false);
	}

	public void OnSkipComplete()
	{
		BountyChooser.Instance.ChooseNewBounty(index);
		iTween.ScaleTo(base.gameObject, iTween.Hash("y", 1f, "time", 0.222f, "easetype", iTween.EaseType.easeOutQuad, "oncompletetarget", base.gameObject, "oncomplete", "OnMoveInComplete"));
		newText.Hide(false);
	}

	public void OnMoveInComplete()
	{
		ButtonEnabled = true;
		UIManager.instance.blockInput = false;
	}

	public void ShowSkipButton()
	{
		if (!_skipButtonVisible)
		{
			StopAllCoroutines();
			skipButtonText.Text = LocalizationManager.Instance.GetFormatString("GOAL_SKIP_PROMPT", SwrveEconomy.GoalSkipCoinCost.ToString());
			iTween.ScaleFrom(skipButtonRoot.gameObject, new Vector3(0f, 0f, 0f), 0.2f);
			SoundEventManager.Instance.Play2D(bountySelectSFX);
			HideSkipButtonAssets(false);
			StartCoroutine(HideSkipButtonLater());
		}
		else if (!_hidingSkipButton)
		{
			StartCoroutine(HideSkipButton());
		}
	}

	public IEnumerator HideSkipButtonLater()
	{
		yield return new WaitForSeconds(3f);
		if (_skipButtonVisible && !_hidingSkipButton)
		{
			yield return StartCoroutine(HideSkipButton());
		}
	}

	private IEnumerator HideSkipButton()
	{
		_hidingSkipButton = true;
		iTween.ScaleTo(skipButtonRoot.gameObject, new Vector3(0f, 0f, 0f), 0.2f);
		SoundEventManager.Instance.Play2D(bountySkipNoSFX);
		yield return new WaitForSeconds(0.5f);
		HideSkipButtonAssets(true);
		skipButtonRoot.transform.localScale = Vector3.one;
		_hidingSkipButton = false;
	}

	private void HideSkipButtonAssets(bool isHidden)
	{
		skipButtonText.Hide(isHidden);
		skipButton.Hide(isHidden);
		skipButton.boxCollider.enabled = !isHidden;
		skipButtonCoinIcon.Hide(isHidden);
		_skipButtonVisible = !isHidden;
	}

	public void SkipButtonClicked()
	{
		StopAllCoroutines();
		HideSkipButtonAssets(true);
		skipButtonRoot.transform.localScale = Vector3.one;
		ReplaceBounty();
	}

	private void ReplaceBounty()
	{
	}
}
