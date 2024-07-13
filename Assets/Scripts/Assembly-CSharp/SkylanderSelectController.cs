using System.Collections;
using UnityEngine;

public class SkylanderSelectController : StateController
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(SkylanderSelectController), LogLevel.Debug);

	public UIScrollList scrollList;

	public CharacterButtonContainer[] containers;

	public NavBar navBar;

	public NavCaret caret;

	public SoundEventData sfxSkylanderSelect;

	public static string LastStateName;

	private static SkylanderSelectController m_Instance = null;

	public static CharacterData OneTimeScrollToSkylander;

	public static SkylanderSelectController Instance
	{
		get
		{
			return m_Instance;
		}
	}

	private void Awake()
	{
		m_Instance = this;
	}

	protected override IEnumerator AnimateStateIn()
	{
		yield return StartCoroutine(base.AnimateStateIn());
		caret.enabled = true;
		if (!string.IsNullOrEmpty(LastStateName))
		{
			GetComponent<StateRoot>().backStateName = LastStateName;
		}
		if (UIBackground.Instance != null)
		{
			UIBackground.Instance.FadeTo(UIBackground.SkyTime.DAY);
		}
		UpdateSkylanderGraphics();
		navBar.UpdateGraphics();
		scrollList.viewableArea = new Vector2(GUISystem.Instance.guiCamera.width, scrollList.viewableArea.y);
		if (StateManager.Instance.lastStateName == "SkylanderDetails")
		{
			_log.LogDebug("Returning from skylander details. Not moving scroll bar because we must have gone there from here.");
		}
	}

	private IEnumerator ScrollToSkylander(CharacterData skylanderToScrollTo)
	{
		float startPos = -0.25f;
		if (skylanderToScrollTo.scrollListPosition < 0.5f)
		{
			startPos = 1.25f;
		}
		scrollList.ScrollPosition = startPos;
		float positionToScrollTo2 = skylanderToScrollTo.scrollListPosition / scrollList.UnviewableArea;
		positionToScrollTo2 = Mathf.Clamp01(positionToScrollTo2);
		_log.LogDebug("\t\t {0} (startPos = {1})", positionToScrollTo2, startPos);
		yield return null;
		iTween.ValueTo(base.gameObject, iTween.Hash("from", startPos, "to", positionToScrollTo2, "time", 1.5f, "easetype", iTween.EaseType.easeOutCirc, "onupdate", "OnScrollListValueChanged", "onupdatetarget", base.gameObject));
		yield return new WaitForSeconds(1.5f);
	}

	private void OnScrollListValueChanged(float inVal)
	{
		scrollList.ScrollPosition = inVal;
	}

	protected override void HideState()
	{
		caret.enabled = false;
		base.HideState();
	}

	public void OnCharacterBtnClick(CharacterData cd)
	{
		if (cd.IsReleased)
		{
			SoundEventManager.Instance.Play2D(sfxSkylanderSelect);
			LoadingPanel instanceAutoCreate = LoadingPanel.InstanceAutoCreate;
			instanceAutoCreate.DismissIfNotLoading = false;
			SkylanderDetailsController.SelectedCharacterData = cd;
			UIManager.instance.blockInput = true;
			StartGameSettings.Instance.activeSkylander = cd;
			if (OperatorMenu.Instance.m_ShowIntroVideo)
			{
				MoviePlayer.Instance.PlayMovie(StartGameSettings.Instance.activeSkylander.movieIntro);
			}
			else
			{
				TransitionController.Instance.StartTransitionFromFrontEnd();
			}
			SwrveEventsUI.SkylanderTouched(cd.charName);
		}
	}

	private void UpdateSkylanderGraphics()
	{
		CharacterButtonContainer[] array = containers;
		foreach (CharacterButtonContainer characterButtonContainer in array)
		{
			characterButtonContainer.UpdateGraphics();
		}
	}

	private CharacterButton FindCharacterButton(CharacterData cd)
	{
		CharacterButtonContainer[] array = containers;
		foreach (CharacterButtonContainer characterButtonContainer in array)
		{
			CharacterButton[] buttons = characterButtonContainer.buttons;
			foreach (CharacterButton characterButton in buttons)
			{
				if (characterButton.cd != null && characterButton.cd.IsReleased && characterButton.cd == cd)
				{
					return characterButton;
				}
			}
		}
		return null;
	}

	public void PlaySelectSound()
	{
		if (sfxSkylanderSelect != null)
		{
			SoundEventManager.Instance.Play2D(sfxSkylanderSelect);
		}
	}
}
