using System;
using UnityEngine;

public class ToyLinkScreen : MonoBehaviour
{
	private const float TimeoutTime = 10f;

	private const string NoTextEnteredError = "TOYLINK_ERROR_NO_TEXT_ENTERED";

	private const string NotEnoughTextEnteredError = "TOYLINK_ERROR_MISSING_DIGITS";

	private const string NoInternetConnectivityError = "TOYLINK_ERROR_NOT_ONLINE";

	private const string NoBedrockConnectionError = "TOYLINK_ERROR_NO_BEDROCK";

	private const string IncorrectToyCodeError = "TOYLINK_ERROR_WRONG_TOY";

	private const string TaskTimeoutError = "TOYLINK_ERROR_TIMEOUT";

	private const string UnlockedTooManyTimesError = "TOYLINK_ERROR_TOO_MANY_UNLOCKS";

	private const string InvalidCodeError = "TOYLINK_ERROR_INVALID_CODE";

	private const string UnknownErrorCodeError = "TOYLINK_ERROR_UNEXPECTED_ERROR";

	private readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(ToyLinkScreen), LogLevel.Debug);

	public UITextField textEntryBox;

	public ErrorDialog linkFailedDialogPrefab;

	public SoundEventData enterPressedSound;

	public Scale3Grid textEntryBackground;

	public SpriteText placeholderText;

	public SpriteText instructionsText;

	private string _oldHeaderText;

	private IDisposable _backButtonOverrideContext;

	private ILinkable _linkableData;

	public Action SuccessAction { get; set; }

	public Action DismissAction { get; set; }

	public ILinkable LinkableData
	{
		get
		{
			return _linkableData;
		}
		set
		{
			_linkableData = value;
			UpdateText();
		}
	}

	private void Start()
	{
		_backButtonOverrideContext = HeaderUI.Instance.PushBackButtonOverrideAction(BackButtonPressed);
		FooterUI.Instance.AnimateOut();
		textEntryBox.AddFocusDelegate(GotFocus);
		textEntryBox.AddValidationDelegate(ValidateText);
	}

	private void OnEnable()
	{
		StateManager.StateDeactivated += HandleStateDeactivated;
	}

	private void OnDisable()
	{
		StateManager.StateDeactivated -= HandleStateDeactivated;
	}

	private void HandleStateDeactivated(object sender, StateEventArgs e)
	{
		Dismiss();
	}

	private void GotFocus(UITextField textField)
	{
		textEntryBackground.Pulse();
	}

	private string ValidateText(UITextField field, string text, ref int insertionPoint)
	{
		if (!placeholderText.IsHidden())
		{
			placeholderText.Hide(true);
		}
		return text.ToUpper();
	}

	private void UpdateText()
	{
		if (HeaderUI.Instance.titleString != LinkableData.ToyLinkDisplayName)
		{
			_oldHeaderText = HeaderUI.Instance.titleString;
			HeaderUI.Instance.titleString = LinkableData.ToyLinkDisplayName;
		}
		instructionsText.Text = LinkableData.ToyLinkCardInstructionText;
	}

	private void BackButtonPressed()
	{
		SwrveEventsProgression.ToyRegistrationCancelled(LinkableData.ToyLinkDisplayName);
		Dismiss();
	}

	public void EnterButtonPressed()
	{
		SoundEventManager.Instance.Play2D(enterPressedSound);
		string text = textEntryBox.Text;
		if (string.IsNullOrEmpty(text))
		{
			ErrorDialog errorDialog = (ErrorDialog)UnityEngine.Object.Instantiate(linkFailedDialogPrefab);
			errorDialog.Display(LocalizationManager.Instance.GetString("TOYLINK_ERROR_TITLE_TRY_AGAIN"), LocalizationManager.Instance.GetString("TOYLINK_ERROR_NO_TEXT_ENTERED"), LocalizationManager.Instance.GetString("DIALOG_OK"), null);
			return;
		}
		text = text.Replace("-", string.Empty);
		if (text.Length > 4)
		{
			text = text.Insert(5, "-");
		}
		if (text.Length > 11)
		{
			text = text.Substring(0, 11);
		}
		textEntryBox.Text = text;
		Debug.Log(text);
		if (text.Length < 11)
		{
			ErrorDialog errorDialog2 = (ErrorDialog)UnityEngine.Object.Instantiate(linkFailedDialogPrefab);
			errorDialog2.Display(LocalizationManager.Instance.GetString("TOYLINK_ERROR_TITLE_TRY_AGAIN"), LocalizationManager.Instance.GetString("TOYLINK_ERROR_MISSING_DIGITS"), LocalizationManager.Instance.GetString("TOYLINK_ERROR_BUTTON_RETRY"), null);
		}
		else
		{
			CheckValue();
		}
	}

	private void Dismiss()
	{
		if (_oldHeaderText != null)
		{
			HeaderUI.Instance.titleString = _oldHeaderText;
		}
		if (_backButtonOverrideContext != null)
		{
			_backButtonOverrideContext.Dispose();
		}
		FooterUI.Instance.AnimateIn();
		if (DismissAction != null)
		{
			DismissAction();
		}
		UnityEngine.Object.Destroy(base.gameObject);
		HeaderUI.Instance.ShowSocialButtons(true);
		UIManager.instance.blockInput = false;
		UIManager.instance.FocusObject = null;
	}

	private void CheckValue()
	{
		string webcode = textEntryBox.Text.ToUpper();
		ToyLinkManager.Instance.BeginToyLink(_linkableData, webcode, SuccessAction_Internal);
	}

	private void SuccessAction_Internal()
	{
		_log.LogDebug("SuccessAction_Internal()");
		Dismiss();
		if (SuccessAction != null)
		{
			_log.LogDebug("Running user-provided success action.");
			SuccessAction();
		}
	}
}
