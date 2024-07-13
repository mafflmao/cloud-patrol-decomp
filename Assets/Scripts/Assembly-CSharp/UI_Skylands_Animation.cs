using System;
using System.Collections;
using UnityEngine;

public class UI_Skylands_Animation : MonoBehaviour
{
	public bool m_StartOnEnable;

	private void OnEnable()
	{
		LoadingPanel.PanelDisplayed += PauseAnimation;
		LoadingPanel.PanelDismissed += StartAnimation;
		if (m_StartOnEnable)
		{
			StartAnimation(null, null);
		}
	}

	private void OnDisable()
	{
		LoadingPanel.PanelDisplayed -= PauseAnimation;
		LoadingPanel.PanelDismissed -= StartAnimation;
		PauseAnimation();
		StopAllCoroutines();
	}

	private void StartAnimation(object sender, EventArgs args)
	{
		foreach (AnimationState item in base.GetComponent<Animation>())
		{
			item.speed = 1f;
		}
		if (!base.GetComponent<Animation>().isPlaying)
		{
			PlayIntro();
		}
	}

	private void PauseAnimation()
	{
		foreach (AnimationState item in base.GetComponent<Animation>())
		{
			item.speed = 0f;
		}
	}

	private void PauseAnimation(object sender, EventArgs args)
	{
		PauseAnimation();
	}

	private void PlayIntro()
	{
		AnimationUtils.PlayClip(base.GetComponent<Animation>(), "Title_Screen_Skylands_Intro");
		StartCoroutine(CoroutineInvoke(base.GetComponent<Animation>()["Title_Screen_Skylands_Intro"].length));
	}

	private void PlayLoop()
	{
		AnimationUtils.PlayClip(base.GetComponent<Animation>(), "Title_Screen_Skylands");
	}

	private IEnumerator CoroutineInvoke(float time)
	{
		yield return new WaitForSeconds(time);
		PlayLoop();
	}
}
