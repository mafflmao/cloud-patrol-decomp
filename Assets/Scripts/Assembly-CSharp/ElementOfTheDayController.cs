using UnityEngine;

public class ElementOfTheDayController : MonoBehaviour
{
	private const string MatchClipName = "EotD_BonusMatch";

	private const string NoMatchClipName = "EotD_BonusMatchNo";

	public SoundEventData playerSFX;

	public SoundEventData todaySFX;

	public SoundEventData playerMoveMatchSFX;

	public SoundEventData playerMoveNoMatchSFX;

	public SoundEventData matchSFX;

	public SoundEventData noMatchSFX;

	public GameObject playerElementSymbol;

	public GameObject dailyElementSymbol;

	public float WaitTime { get; private set; }

	public void Awake()
	{
		if (true)
		{
			base.GetComponent<Animation>().Play("EotD_BonusMatch");
			WaitTime = base.GetComponent<Animation>().GetClip("EotD_BonusMatch").length;
		}
		else
		{
			base.GetComponent<Animation>().Play("EotD_BonusMatchNo");
			WaitTime = base.GetComponent<Animation>().GetClip("EotD_BonusMatchNo").length;
		}
	}

	public void PlaySoundPlayer()
	{
		SoundEventManager.Instance.Play2D(playerSFX);
	}

	public void PlaySoundToday()
	{
		SoundEventManager.Instance.Play2D(todaySFX);
	}

	public void PlaySoundPlayerMoveMatch()
	{
		SoundEventManager.Instance.Play2D(playerMoveMatchSFX);
	}

	public void PlaySoundPlayerMoveNoMatch()
	{
		SoundEventManager.Instance.Play2D(playerMoveNoMatchSFX);
	}

	public void PlaySoundMatch()
	{
		SoundEventManager.Instance.Play2D(matchSFX);
	}

	public void PlaySoundNoMatch()
	{
		SoundEventManager.Instance.Play2D(noMatchSFX);
	}
}
