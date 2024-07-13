using System;
using System.Collections;
using UnityEngine;

public class HelpController : StateController
{
	public UIButtonComposite[] buttons;

	public UIButtonComposite importSaveButton;

	public SoundEventData sfxBtnClick;

	public ConfirmationDialog importSaveDialogPrefab;

	public ConfirmationDialog importSaveDialogConfirmPrefab;

	public ErrorDialog errorDialogPrefab;

	public SpriteText bedrockVersionText;

	public SpriteText changelistText;

	private float lastErrorTime;

	private bool animateToNextScene = true;

	private Vector3[] buttonLocations;

	private void Awake()
	{
		buttonLocations = new Vector3[buttons.Length];
		for (int i = 0; i < buttons.Length; i++)
		{
			buttonLocations[i] = buttons[i].transform.localPosition;
		}
	}

	protected override void ShowState()
	{
		animateToNextScene = true;
		bool flag = Bedrock.getUserConnectionStatus().IsRegistered();
		importSaveButton.IsButtonColliderEnabled = flag;
		importSaveButton.SetColor((!flag) ? Color.gray : Color.white);
		bedrockVersionText.Text = "Bedrock v." + Bedrock.GetBedrockVersion();
		TextAsset textAsset = (TextAsset)Resources.Load("BuildId", typeof(TextAsset));
		if (textAsset != null)
		{
			string[] array = textAsset.text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			string text = "?";
			if (array.Length > 0)
			{
				text = array[0];
			}
			changelistText.Text = "v." + 1.8f + " (" + text + ")";
		}
		base.ShowState();
	}

	protected override void HideState()
	{
		Debug.Log("HelpController: HideState");
		base.HideState();
	}

	protected override IEnumerator AnimateStateIn()
	{
		Debug.Log("HelpController: AnimateStatein");
		float delay = 0f;
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].transform.localPosition = buttonLocations[i];
			float xCoord = 2000f;
			if (i % 2 == 0)
			{
				xCoord *= -1f;
			}
			iTween.MoveFrom(buttons[i].gameObject, iTween.Hash("x", xCoord, "easeType", iTween.EaseType.easeOutQuad, "islocal", true, "time", 0.3f, "delay", delay));
			delay += 0.15f;
		}
		yield return new WaitForEndOfFrame();
		Debug.Log("HelpController: AnimateStateIn End of sequence");
		ShowState();
	}

	protected override IEnumerator AnimateStateOut()
	{
		UIManager.instance.blockInput = true;
		if (animateToNextScene)
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				float xCoord = 2000f;
				if (i % 2 == 0)
				{
					xCoord *= -1f;
				}
				iTween.MoveTo(buttons[i].gameObject, iTween.Hash("x", xCoord, "easeType", iTween.EaseType.easeInQuad, "islocal", true, "time", 0.3f));
				yield return new WaitForSeconds(0.15f);
			}
		}
		else
		{
			HeaderUI.Instance.visible = false;
		}
		HideState();
		yield return null;
	}

	public void OnTutorialBtnClick()
	{
		animateToNextScene = false;
		SoundEventManager.Instance.Play2D(sfxBtnClick);
		Debug.Log("Tutorial button clicked");
		ElementOfTheDayChanger.AllowElementOfTheDayChanges = false;
		SwrveEventsUI.TutorialButtonTouched();
		StartCoroutine(OnTutorialBtnClickRte());
	}

	private IEnumerator OnTutorialBtnClickRte()
	{
		UIManager.instance.blockInput = true;
		yield return new WaitForSeconds(0.5f);
		UIManager.instance.blockInput = false;
		yield return new WaitForFixedUpdate();
		StateManager.Instance.LoadAndActivateState("Intro");
	}

	public void OnFAQsBtnClick()
	{
		SoundEventManager.Instance.Play2D(sfxBtnClick);
		Debug.Log("FAQ button clicked");
		StartCoroutine(OnFAQsBtnClickRte());
		SwrveEventsUI.FAQButtonTouched();
	}

	private IEnumerator OnFAQsBtnClickRte()
	{
		UIManager.instance.blockInput = true;
		yield return new WaitForSeconds(0.5f);
		UIManager.instance.blockInput = false;
		yield return new WaitForFixedUpdate();
		if (Application.internetReachability != 0)
		{
			ActivateWatcher.Instance.ShowActivateUI(Bedrock.brUserInterfaceScreen.BR_CUSTOMER_SERVICE_UI);
		}
	}

	public void OnCreditsBtnClick()
	{
		SoundEventManager.Instance.Play2D(sfxBtnClick);
		Debug.Log("Credits button clicked");
		StartCoroutine(OnCreditsBtnClickRte());
		SwrveEventsUI.CreditsButtonTouched();
	}

	private IEnumerator OnCreditsBtnClickRte()
	{
		UIManager.instance.blockInput = true;
		yield return new WaitForSeconds(0.5f);
		UIManager.instance.blockInput = false;
		AchievementManager.Instance.autoSync = true;
		AchievementManager.Instance.SetStep(Achievements.ViewCredits, 1);
		AchievementManager.Instance.autoSync = false;
		StateManager.Instance.LoadAndActivateState("Credits");
	}

	public void OnSaveDataButtonClicked()
	{
		SoundEventManager.Instance.Play2D(sfxBtnClick);
		Debug.Log("Save Data Button Clicked");
		StartCoroutine(OnSaveDataButtonClickedRte());
		SwrveEventsUI.SaveDataImportButtonTouched();
	}

	private IEnumerator OnSaveDataButtonClickedRte()
	{
		UIManager.instance.blockInput = true;
		yield return new WaitForSeconds(0.5f);
		UIManager.instance.blockInput = false;
		ConfirmationDialog importSaveDialogInstance = (ConfirmationDialog)UnityEngine.Object.Instantiate(importSaveDialogPrefab);
		StartCoroutine(importSaveDialogInstance.Display(string.Empty, ImportSaveDialogConfirmed, ImportSaveDialogCancelled));
	}

	private void ImportSaveDialogConfirmed()
	{
		StartCoroutine(ShowImportSaveConfirmation());
	}

	private IEnumerator ShowImportSaveConfirmation()
	{
		UIManager.instance.blockInput = true;
		yield return new WaitForSeconds(0.5f);
		UIManager.instance.blockInput = false;
		ConfirmationDialog instance = (ConfirmationDialog)UnityEngine.Object.Instantiate(importSaveDialogConfirmPrefab);
		yield return StartCoroutine(instance.Display(string.Empty, ImportSave, ImportSaveDialogCancelled));
	}

	private void ImportSaveDialogCancelled()
	{
		SwrveEventsUI.SaveDataImportCancelled();
	}

	private void ImportSave()
	{
		bool flag = MigrateSaveDialog.CopyAnonymousUserDataToCurrentBedrockAccount();
		ErrorDialog errorDialog = (ErrorDialog)UnityEngine.Object.Instantiate(errorDialogPrefab);
		if (flag)
		{
			errorDialog.titleSpriteText.SetColor(new Color(0.11f, 0.51f, 0.91f, 1f));
			errorDialog.Display(LocalizationManager.Instance.GetString("IMPORT_SAVE_HEADER"), LocalizationManager.Instance.GetString("ACTIVATE_SAVE_DATA_TRANSFER_SUCCESS"), LocalizationManager.Instance.GetString("DIALOG_OK"), null);
			SwrveEventsUI.SaveDataImportCompleted();
		}
		else
		{
			errorDialog.Display(LocalizationManager.Instance.GetString("IMPORT_SAVE_HEADER"), LocalizationManager.Instance.GetString("ACTIVATE_SAVE_DATA_TRANSFER_FAILURE"), LocalizationManager.Instance.GetString("DIALOG_OK"), null);
			SwrveEventsUI.SaveDataImportFailed();
		}
	}

	private void OnMailComposerFinished(string message)
	{
		Debug.Log(message);
		if (message.Contains("Failed"))
		{
			ShowMailError();
		}
	}

	private void ShowMailError()
	{
		if (Time.realtimeSinceStartup - lastErrorTime > 0.5f)
		{
			lastErrorTime = Time.realtimeSinceStartup;
		}
	}
}
