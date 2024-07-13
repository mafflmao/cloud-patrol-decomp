using System.Collections;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
	private readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(PauseScreen), LogLevel.Debug);

	public UIButtonComposite godButton;

	public SpriteText godButtonStateText;

	public PrefabPlaceholder _bountyPlaceholder;

	public ConfirmationDialog confirmationDialogPrefab;

	public SoundEventData confirmationDialogDismissedSound;

	public SoundEventData confirmationDialogAcceptedSound;

	public SoundEventData confirmationDialogCancelSound;

	private bool _handlingContinue;

	private bool _handlingQuit;

	public void Start()
	{
		UpdateVisuals();
	}

	public void UpdateVisuals()
	{
		if (!(ShipManager.instance != null))
		{
		}
	}

	public void ContinuePressed()
	{
		_log.Log("ContinuePressed()");
		if (_handlingContinue)
		{
			_log.LogWarning("Continue pressed 2x in a row in single frame!");
			return;
		}
		if (_handlingQuit)
		{
			_log.LogError("User hit continue while quit was being handled. Ignoring.");
			return;
		}
		_handlingContinue = true;
		UIManager.instance.blockInput = true;
		SoundEventManager.Instance.Play2D(GlobalSoundEventData.Instance.UnPauseSound);
		SwrveEventsUI.ContinueTouched();
		StartCoroutine(DoContinue());
	}

	private IEnumerator DoContinue()
	{
		_log.LogDebug("DoContinue()");
		float startTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < startTime + 0.5f)
		{
			yield return new WaitForEndOfFrame();
		}
		GameManager.Instance.PopPause(PauseReason.System);
		UIManager.instance.blockInput = false;
		_handlingContinue = false;
	}

	public void QuitPressed()
	{
		_log.LogDebug("QuitPressed");
		if (_handlingQuit)
		{
			_log.LogError("Somehow the user was able to hit QUIT twice!!! Skipped second activation.");
			return;
		}
		if (_handlingContinue)
		{
			_log.LogError("User somehow hit quite while continue was being handled. Ignoring.");
			return;
		}
		_handlingQuit = true;
		ConfirmationDialog confirmationDialog = (ConfirmationDialog)Object.Instantiate(confirmationDialogPrefab);
		confirmationDialog.OnNoDismissedSound = confirmationDialogDismissedSound;
		string @string = LocalizationManager.Instance.GetString("PAUSE_QUIT_CONFIRM");
		StartCoroutine(confirmationDialog.Display(@string, DoQuit, CancelQuit));
	}

	public void DoQuit()
	{
		_log.LogDebug("DoQuit()");
		SwrveEventsUI.QuitTouched();
		SoundEventManager.Instance.Play2D(confirmationDialogAcceptedSound);
		GameManager.Instance.PopPause(PauseReason.System);
		HealingElixir.IsUsable = false;
		GameManager.Instance.ShowGameOverScreen();
		_handlingQuit = false;
	}

	public void CancelQuit()
	{
		SoundEventManager.Instance.Play2D(confirmationDialogCancelSound);
		_handlingQuit = false;
	}

	public void GodToggle()
	{
		if (GameManager.invincible)
		{
			GameManager.invincible = false;
			godButtonStateText.Text = "OFF";
		}
		else
		{
			GameManager.invincible = true;
			godButtonStateText.Text = "ON";
		}
	}
}
