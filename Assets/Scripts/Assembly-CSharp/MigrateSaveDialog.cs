using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MigrateSaveDialog : MonoBehaviour
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(MigrateSaveDialog), LogLevel.Debug);

	public GameObject destroyParticle;

	public GameObject visibleStuff;

	public SoundEventData transitionInSFX;

	public SoundEventData OnYesDismissedSound { get; set; }

	public SoundEventData OnNoDismissedSound { get; set; }

	public event EventHandler<EventArgs> UserDismissed;

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
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Display()
	{
		iTween.ScaleFrom(visibleStuff.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.333f, "ignoretimescale", true));
		SoundEventManager.Instance.Play2D(transitionInSFX);
	}

	private IEnumerator Dismiss()
	{
		UIManager.instance.blockInput = true;
		iTween.ScaleTo(visibleStuff.gameObject, Vector3.zero, 0.333f);
		yield return new WaitForSeconds(0.3f);
		UnityEngine.Object.Instantiate(destroyParticle, base.transform.position, Quaternion.identity);
		NotificationPanel.Instance.Display(new ActivateNotificationPanelSettings(LocalizationManager.Instance.GetString("ACTIVATE_IMPORT_FINISHED"), 2f));
		UnityEngine.Object.Destroy(base.gameObject);
		UIManager.instance.blockInput = false;
		if (this.UserDismissed != null)
		{
			this.UserDismissed(this, new EventArgs());
		}
	}

	private void newBtnHit()
	{
		UIManager.instance.blockInput = true;
		SoundEventManager.Instance.Play2D(OnNoDismissedSound);
		StartCoroutine(Dismiss());
	}

	private void transferBtnHit()
	{
		StartCoroutine(TransferUserData());
	}

	private IEnumerator TransferUserData()
	{
		CopyAnonymousUserDataToCurrentBedrockAccount();
		_log.Log("UserID: " + Bedrock.getDefaultOnlineId());
		StartCoroutine(Dismiss());
		yield break;
	}

	public static bool CopyAnonymousUserDataToCurrentBedrockAccount()
	{
		_log.Log("Transferring anonymous data from current user to bedrock account...");
		if (!Bedrock.MoveAnonymousUserCacheDataToUser())
		{
			_log.LogError("Failed to transfer anonymous data to registered account.");
			return false;
		}
		if (!Bedrock.ResolveUserCacheVariablesWithCloud(true))
		{
			_log.LogError("Failed to resolve user cache variables with cloud.");
		}
		CharacterData[] allReleasedSkylanders = BountyChooser.Instance.allCharacters.GetAllReleasedSkylanders();
		CharacterData[] array = allReleasedSkylanders;
		foreach (CharacterData cd in array)
		{
			CharacterUserData characterUserData = new CharacterUserData(cd);
			if (characterUserData.IsUnlocked && characterUserData.IsToyLinked)
			{
				characterUserData.ResetUnlockState();
			}
		}
		List<PowerupData> powerups = BountyChooser.Instance.allPowerups.powerups;
		foreach (PowerupData item in powerups)
		{
			if (!item.IsLinked)
			{
			}
		}
		return true;
	}
}
