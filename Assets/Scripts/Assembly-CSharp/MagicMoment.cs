using System;
using System.Collections;
using UnityEngine;

public class MagicMoment : MonoBehaviour
{
	public SoundEventData VO_SkylanderAlive;

	public SoundEventData mmSwirlLoop;

	public SoundEventData mmLightningSFX;

	public SoundEventData mmOrbGrow;

	public SoundEventData mmOrbExplode;

	public SoundEventData mmChampionLeave;

	public GameObject skylanderRigged;

	public GameObject background;

	public float skylanderSpawnTime = 5f;

	public static event EventHandler<EventArgs> MagicMomentComplete;

	public static event EventHandler<EventArgs> MagicMomentFadingOut;

	private void Start()
	{
		background.GetComponent<Renderer>().material.renderQueue = 0;
		GameObjectUtils.HideObject(skylanderRigged);
		if (VO_SkylanderAlive == null)
		{
			VO_SkylanderAlive = StartGameSettings.Instance.activeSkylander.AudioResources.toyAliveSFX;
		}
		StartCoroutine(MagicMomentSequence());
	}

	public IEnumerator MagicMomentSequence()
	{
		iTween.ColorTo(background, new Color(0f, 0f, 0f, 1f), 1f);
		base.GetComponent<Animation>().Play();
		yield return new WaitForSeconds(skylanderSpawnTime);
		GameObjectUtils.ShowObject(skylanderRigged);
		skylanderRigged.GetComponent<Animation>().clip = AnimationUtils.PlayClip(skylanderRigged.GetComponent<Animation>(), "alive");
		yield return new WaitForSeconds(skylanderRigged.GetComponent<Animation>().clip.length);
		skylanderRigged.GetComponent<Animation>().clip = AnimationUtils.PlayClip(skylanderRigged.GetComponent<Animation>(), "toy");
		yield return new WaitForSeconds(0.4f);
		skylanderRigged.GetComponent<Animation>().clip = AnimationUtils.PlayClip(skylanderRigged.GetComponent<Animation>(), "jump");
		MagicMomentChampionLeave();
		yield return new WaitForSeconds(1f);
		Debug.Log("MM Fading Out");
		iTween.ColorTo(background, new Color(0f, 0f, 0f, 0f), 1f);
		OnMagicMomentFadingOut();
		MagicMomentSwirlLoopStop();
		yield return new WaitForSeconds(1f);
		UnityEngine.Object.Destroy(base.gameObject);
		OnMagicMomentComplete();
	}

	public void MagicMomentToyAlive()
	{
		if ((bool)VO_SkylanderAlive)
		{
			SoundEventManager.Instance.Play2D(VO_SkylanderAlive);
		}
	}

	public void MagicMomentSwirlLoop()
	{
		SoundEventManager.Instance.Play2D(mmSwirlLoop);
	}

	public void MagicMomentSwirlLoopStop()
	{
		SoundEventManager.Instance.Stop2D(mmSwirlLoop);
	}

	public void MagicMomentLightningSFX()
	{
		SoundEventManager.Instance.Play2D(mmLightningSFX);
	}

	public void MagicMomentOrbGrow()
	{
		SoundEventManager.Instance.Play2D(mmOrbGrow);
	}

	public void MagicMomentOrbExplode()
	{
		SoundEventManager.Instance.Play2D(mmOrbExplode);
	}

	public void MagicMomentChampionLeave()
	{
		SoundEventManager.Instance.Play2D(mmChampionLeave);
	}

	private void OnMagicMomentComplete()
	{
		if (MagicMoment.MagicMomentComplete != null)
		{
			MagicMoment.MagicMomentComplete(this, new EventArgs());
		}
	}

	private void OnMagicMomentFadingOut()
	{
		if (MagicMoment.MagicMomentFadingOut != null)
		{
			MagicMoment.MagicMomentFadingOut(this, new EventArgs());
		}
	}
}
