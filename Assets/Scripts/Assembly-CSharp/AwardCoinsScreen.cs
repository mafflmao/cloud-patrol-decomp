using System;
using System.Collections;
using UnityEngine;

public class AwardCoinsScreen : ScreenSequenceScreen
{
	private const float offscreenAmount = 1324f;

	private const float TallyTimePerCoin = 0.01f;

	private const float MaxTallyTime = 1f;

	private Vector3 OffscreenOffset = new Vector3(1324f, 0f, 0f);

	public GameObject chest;

	public GameObject voyageCoinPanel;

	public GameObject bonusPanel;

	public SpriteText totalCoinText;

	public SpriteText voyageCoinText;

	public PackedSprite voyageCoinIcon;

	public SpriteText bonusText;

	public SpriteText eotdTextLabel;

	public PackedSprite bonusIcon;

	public SpriteText bonusSkylanderText;

	public SoundEventData coinTallySound;

	public SoundEventData coinSubTallyFinishedSound;

	public SoundEventData coinOverallTallyFinishedSound;

	public SoundEventData bonusSound;

	public SoundEventData uiSlideInSound;

	public ParticleSystem coinShowerParticleEmitter;

	private float _deltaTime = 0.01f;

	private Animation _chestAnimation;

	private int _totalMoneyBeforeTally;

	private int _bonusMoney;

	public static bool testMode;

	protected override void Start()
	{
		_chestAnimation = chest.GetComponent<Animation>();
		coinShowerParticleEmitter.Play();
		string format = ((GameManager.skylandersUnlockedForBonus != 1) ? LocalizationManager.Instance.GetString("AWARD_COINS_SKYLANDERS_PLURAL") : LocalizationManager.Instance.GetString("AWARD_COINS_SKYLANDERS_SINGLE"));
		bonusSkylanderText.Text = string.Format(format, GameManager.skylandersUnlockedForBonus);
		SetBonusIconVisible(false);
		SetVoyageCoinsVisible(false);
		_bonusMoney = GameManager.moneyAwardedForBonus;
		_totalMoneyBeforeTally = 0;
		SetTotalCoins(_totalMoneyBeforeTally);
		if (testMode)
		{
			_totalMoneyBeforeTally = 1234;
			_bonusMoney = 567;
		}
		SetVoyageCoins(0);
		base.Start();
		StartCoroutine(StartCoinTally());
		SwrveEventsProgression.ElementalBonusCoinsAwarded(_bonusMoney);
	}

	protected override void AnimateIn()
	{
		AwardScreen.MoveFrom(base.gameObject, base.gameObject.transform.position + OffscreenOffset, 0.333f);
		SoundEventManager.Instance.Play(uiSlideInSound, base.gameObject);
	}

	protected override void AnimateOut()
	{
		AwardScreen.MoveTo(base.gameObject, base.gameObject.transform.position + OffscreenOffset, 0.333f);
		SoundEventManager.Instance.Play(uiSlideInSound, base.gameObject);
		StopAllCoroutines();
		SoundEventManager.Instance.Stop2D(coinTallySound);
	}

	private void OnDestroy()
	{
		SoundEventManager.Instance.Stop2D(coinTallySound);
	}

	private IEnumerator StartCoinTally()
	{
		float coinTallyTime = Mathf.Clamp((float)_bonusMoney * 0.01f, 0f, 1f);
		Debug.Log("Tally time - " + coinTallyTime);
		yield return new WaitForSeconds(0.333f);
		_chestAnimation.Play("Open");
		yield return new WaitForSeconds(0.222f);
		SetVoyageCoinsVisible(true);
		SetBonusIconVisible(true);
		Vector3 originalVoyageCoinPanelPosition = voyageCoinPanel.transform.position;
		voyageCoinPanel.transform.position += OffscreenOffset;
		iTween.MoveFrom(bonusPanel, bonusPanel.transform.position + OffscreenOffset, 0.333f);
		SoundEventManager.Instance.Play2D(bonusSound);
		yield return new WaitForSeconds(0.333f);
		yield return new WaitForSeconds(0.666f);
		SetVoyageCoins(_bonusMoney);
		iTween.MoveTo(voyageCoinPanel, originalVoyageCoinPanelPosition, 0.333f);
		iTween.MoveTo(bonusPanel, bonusPanel.transform.position + OffscreenOffset, 0.333f);
		yield return new WaitForSeconds(0.333f);
		SetBonusIconVisible(false);
		coinShowerParticleEmitter.Play();
		StartCoroutine(Tally(_bonusMoney, 0, coinTallyTime, SetVoyageCoins));
		StartCoroutine(Tally(_totalMoneyBeforeTally, _totalMoneyBeforeTally + _bonusMoney, coinTallyTime, SetTotalCoins));
		SoundEventManager.Instance.Play2D(coinTallySound);
		yield return new WaitForSeconds(coinTallyTime);
		coinShowerParticleEmitter.Play();
        SoundEventManager.Instance.Stop2D(coinTallySound);
		SoundEventManager.Instance.Play2D(coinOverallTallyFinishedSound);
		SetVoyageCoinsVisible(false);
		UnityEngine.Object.Destroy(coinShowerParticleEmitter.gameObject);
		_chestAnimation.Play("Close");
		StartTimeout(1f);
	}

	private void SetVoyageCoinsVisible(bool isVisible)
	{
		voyageCoinText.Hide(!isVisible);
		voyageCoinIcon.Hide(!isVisible);
	}

	private void SetBonusIconVisible(bool isVisible)
	{
		bonusSkylanderText.Hide(!isVisible);
		bonusText.Hide(!isVisible);
		eotdTextLabel.Hide(!isVisible);
		bonusIcon.Hide(!isVisible);
	}

	private void SetVoyageCoins(int value)
	{
		voyageCoinText.Text = value.ToString("n0");
	}

	private void SetTotalCoins(int value)
	{
		totalCoinText.Text = value.ToString("n0");
	}

	private IEnumerator Tally(int start, int end, float totalTime, Action<int> setValueAction)
	{
		float endTime = Time.time + totalTime;
		while (endTime - Time.time >= 0f)
		{
			yield return new WaitForSeconds(_deltaTime);
			float remainingTime = endTime - Time.time;
			float value = Mathf.Lerp(start, end, (totalTime - remainingTime) / totalTime);
			setValueAction((int)Math.Round(value));
		}
		setValueAction(end);
	}
}
