using System.Collections;
using UnityEngine;

public class AwardStarsLevelUpScreen : MonoBehaviour
{
	private const string LevelUpOutroAnimationName = "LevelUp_Animation_Exit";

	private const string LevelUpIntroAnimationName = "LevelUp_Animation";

	private const int OnboardingScreenZOffset = -80;

	public SpriteText gemCountText;

	public PackedSprite gemCountTextBackground;

	public GameObject gemCountPanel;

	public GameObject gemPrefab;

	public GameObject levelUpAnimationPrefab;

	public GameObject badgeUpgradeAnimationPrefab;

	public SoundEventData levelUpSFX;

	public SoundEventData gemSpawnSFX;

	public SoundEventData uiWipeEchoSFX;

	private void Start()
	{
		gemCountText.Hide(true);
		gemCountTextBackground.Hide(true);
	}

	public IEnumerator AnimateIn(int gemsAwarded)
	{
		yield return new WaitForSeconds(0.333f);
		Vector3 offscreenBottomOffset = new Vector3(0f, -400f, 0f);
		Vector3 offscreenTopOffset = new Vector3(0f, 600f, 0f);
		GameObject levelUpAnimationInstance = (GameObject)Object.Instantiate(badgeUpgradeAnimationPrefab);
		levelUpAnimationInstance.transform.parent = base.transform;
		levelUpAnimationInstance.transform.Translate(new Vector3(0f, 0f, -100f));
		levelUpAnimationInstance.GetComponent<Animation>().Play("LevelUp_Animation");
		SoundEventManager.Instance.Play2D(levelUpSFX);
		yield return new WaitForSeconds(0.333f);
		float waitBetweenGemsTime = 0.222f;
		GameObject[] gemInstances = new GameObject[gemsAwarded];
		Vector3 gemStartLinePosition = new Vector3(-700f, 450f, -225f);
		Vector3 gemEndLinePosition = new Vector3(700f, 450f, -225f);
		float totalMagnitude = (gemEndLinePosition - gemStartLinePosition).magnitude;
		float segmentSize = totalMagnitude / (float)(gemsAwarded - 1);
		for (int gemNumber = 0; gemNumber < gemsAwarded; gemNumber++)
		{
			GameObject instance2 = (GameObject)Object.Instantiate(position: gemStartLinePosition + (gemEndLinePosition - gemStartLinePosition).normalized * segmentSize * gemNumber, original: gemPrefab, rotation: Quaternion.identity);
			instance2.transform.parent = base.gameObject.transform;
			instance2.layer = base.gameObject.layer;
			gemInstances[gemNumber] = instance2;
			SoundEventManager.Instance.Play2D(gemSpawnSFX);
			iTween.MoveFrom(instance2, instance2.transform.position + offscreenTopOffset, 0.333f);
			iTween.RotateBy(instance2, new Vector3(1f, 3f, 1f), 1.998f);
			yield return new WaitForSeconds(waitBetweenGemsTime);
		}
		gemCountText.Text = LocalizationManager.Instance.GetFormatString("AWARD_LEVEL_UP_GEMS", gemsAwarded);
		gemCountText.Hide(false);
		gemCountTextBackground.Hide(false);
		iTween.MoveFrom(gemCountPanel.gameObject, gemCountPanel.transform.position + offscreenBottomOffset, waitBetweenGemsTime);
		SoundEventManager.Instance.Play(uiWipeEchoSFX, gemCountPanel.gameObject);
		yield return new WaitForSeconds(waitBetweenGemsTime);
		yield return new WaitForSeconds(0.666f);
		levelUpAnimationInstance.GetComponent<Animation>().Play("LevelUp_Animation_Exit");
		foreach (GameObject instance in gemInstances)
		{
			iTween.MoveTo(instance, instance.transform.position + offscreenTopOffset, 0.333f);
		}
		iTween.MoveTo(gemCountPanel.gameObject, gemCountText.transform.position + offscreenBottomOffset, waitBetweenGemsTime);
		yield return new WaitForSeconds(0.333f);
		Object.Destroy(levelUpAnimationInstance);
	}
}
