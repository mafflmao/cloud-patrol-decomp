using System.Collections;
using UnityEngine;

public class AwardGameplayGemsScreen : ScreenSequenceScreen
{
	public GameObject gem;

	public GameObject textParent;

	public SpriteText gemText;

	private float rotateSpeed = 10f;

	private float yCoordToSlideInFrom = -1500f;

	public SoundEventData uiWipeEchoSFX;

	public SoundEventData gemSpawnSFX;

	protected override void Start()
	{
		base.Start();
		int gemsCollectedInVoyage = GameManager.gemsCollectedInVoyage;
		gemText.Text = ((gemsCollectedInVoyage != 1) ? LocalizationManager.Instance.GetFormatString("AWARD_GEMS_PLURAL", gemsCollectedInVoyage) : LocalizationManager.Instance.GetString("AWARD_GEMS_SINGLE"));
		SwrveEventsProgression.PresentGemsAwarded(gemsCollectedInVoyage);
	}

	protected override void AnimateIn()
	{
		StartCoroutine(AnimateInCoroutine());
	}

	public IEnumerator AnimateInCoroutine()
	{
		iTween.RotateBy(gem, new Vector3(1f, 3f, 1f), 3.996f);
		iTween.MoveFrom(gem, new Vector3(gem.transform.position.x, yCoordToSlideInFrom, gem.transform.position.z), 0.666f);
		SoundEventManager.Instance.Play(gemSpawnSFX, gem);
		yield return new WaitForSeconds(0.222f);
		iTween.MoveFrom(textParent, iTween.Hash("position", new Vector3(textParent.transform.position.x, yCoordToSlideInFrom, textParent.transform.position.z), "time", 0.666f));
		SoundEventManager.Instance.Play(uiWipeEchoSFX, textParent);
		StartTimeout(2f);
	}

	protected override void AnimateOut()
	{
		StartCoroutine(AnimateOutCoroutine());
	}

	public IEnumerator AnimateOutCoroutine()
	{
		iTween.MoveTo(textParent, new Vector3(textParent.transform.position.x, yCoordToSlideInFrom, textParent.transform.position.z), 0.666f);
		SoundEventManager.Instance.Play(uiWipeEchoSFX, textParent);
		yield return new WaitForSeconds(0.222f);
		iTween.MoveTo(gem, iTween.Hash("position", new Vector3(gem.transform.position.x, yCoordToSlideInFrom, gem.transform.position.z), "time", 0.666f));
		Suicide(0.666f);
	}

	private void Update()
	{
		Vector3 vector = new Vector3(0f, 1f, 1f);
		gem.transform.Rotate(vector.normalized, rotateSpeed * Time.deltaTime);
	}
}
