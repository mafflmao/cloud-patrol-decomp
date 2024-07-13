using System;
using System.Collections;
using UnityEngine;

public class IntroController : StateController
{
	public Animation anim;

	public GameObject faderPlane;

	public SpriteText skipText;

	private bool _userWantsToSkip;

	private bool _isSkipPromptShowing;

	private bool _allowSkip = true;

	public void Start()
	{
		AudioListener componentInChildren = base.gameObject.GetComponentInChildren<AudioListener>();
		if (componentInChildren != null)
		{
			Debug.Log("Extra audio listener found, deleting");
			UnityEngine.Object.Destroy(componentInChildren);
		}
		MusicManager.Instance.PlayIntroMusic();
		ScreenTimeoutUtility.Instance.AllowTimeout = false;
		skipText.Hide(true);
	}

	private void OnEnable()
	{
		FingerGestures.OnFingerDown += HandleFingerGesturesOnFingerDown;
		MagicMoment.MagicMomentFadingOut += HandleMagicMomentFadingOut;
	}

	private void OnDisable()
	{
		FingerGestures.OnFingerDown -= HandleFingerGesturesOnFingerDown;
		MagicMoment.MagicMomentFadingOut -= HandleMagicMomentFadingOut;
	}

	private void HandleFingerGesturesOnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		if (_allowSkip)
		{
			if (!_isSkipPromptShowing)
			{
				StartCoroutine(ShowSkipPromptCoroutine());
			}
			else
			{
				_userWantsToSkip = true;
			}
		}
	}

	private IEnumerator ShowSkipPromptCoroutine()
	{
		_isSkipPromptShowing = true;
		skipText.gameObject.transform.localScale = Vector3.one;
		skipText.Hide(false);
		iTween.ScaleFrom(skipText.gameObject, Vector3.zero, 0.5f);
		yield return new WaitForSeconds(0.5f);
		yield return new WaitForSeconds(2f);
		iTween.ScaleTo(skipText.gameObject, Vector3.zero, 0.5f);
		yield return new WaitForSeconds(0.5f);
		skipText.Hide(true);
		_isSkipPromptShowing = false;
	}

	protected override IEnumerator AnimateStateIn()
	{
		UIManager.instance.blockInput = false;
		base.AnimateStateIn();
		if (!_userWantsToSkip)
		{
			yield return new WaitForSeconds(1f);
			iTween.ColorTo(faderPlane, new Color(0f, 0f, 0f, 0f), 1f);
			anim.Play();
			while (anim.isPlaying && !_userWantsToSkip)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
		if (_userWantsToSkip)
		{
			anim.Stop();
			MusicManager.Instance.StopMusicAndDontStartTheWind();
			iTween.ColorTo(faderPlane, new Color(0f, 0f, 0f, 1f), 0.25f);
			yield return new WaitForSeconds(0.25f);
		}
		else
		{
			yield return new WaitForSeconds(2f);
			iTween.ColorTo(faderPlane, new Color(0f, 0f, 0f, 1f), 1f);
			yield return new WaitForSeconds(2f);
		}
		_allowSkip = false;
		skipText.Hide(true);
		UIManager.instance.blockInput = true;
		Application.LoadLevelAdditiveAsync(StartGameSettings.Instance.activeSkylander.magicMomentScene);
		yield return new WaitForSeconds(3f);
		GetComponent<UIBackgroundLoader>().LoadBackground();
		GetComponent<UIBackgroundLoader>().StartAnimation();
	}

	private void TransitionIntoGameplay()
	{
		CharacterUserData characterUserData = ElementDataManager.Instance.GetCharacterUserData(StartGameSettings.Instance.activeSkylander);
		characterUserData.UnlockCharacter(0, CharacterUserData.ToyLink.None);
		ElementUserData elementUserData = ElementDataManager.Instance.GetElementUserData(StartGameSettings.Instance.activeSkylander.elementData.elementType);
		elementUserData.Update();
		InvokeHelper.InvokeSafe(TransitionController.Instance.StartTransitionFromFrontEnd, 0.25f, TransitionController.Instance);
		UIManager.instance.blockInput = false;
	}

	public void HandleMagicMomentFadingOut(object sender, EventArgs args)
	{
		MagicMoment.MagicMomentFadingOut -= HandleMagicMomentFadingOut;
		TransitionIntoGameplay();
	}
}
