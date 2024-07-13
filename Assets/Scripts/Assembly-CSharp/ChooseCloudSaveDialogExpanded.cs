using System;
using UnityEngine;

public class ChooseCloudSaveDialogExpanded : MonoBehaviour
{
	public GameObject destroyParticle;

	public GameObject visibleStuff;

	public SoundEventData transitionInSFX;

	public SpriteText localDateTimeSpriteText;

	public SpriteText localDeviceSpriteText;

	public SpriteText localSkylandersSpriteText;

	public SpriteText localMagicItemsSpriteText;

	public SpriteText localGemsSpriteText;

	public SpriteText localRankSpriteText;

	public SpriteText onlineDateTimeSpriteText;

	public SpriteText onlineDeviceSpriteText;

	public SpriteText onlineSkylandersSpriteText;

	public SpriteText onlineMagicItemsSpriteText;

	public SpriteText onlineGemsSpriteText;

	public SpriteText onlineRankSpriteText;

	public SoundEventData OnYesDismissedSound { get; set; }

	public SoundEventData OnNoDismissedSound { get; set; }

	private void OnEnable()
	{
		Bedrock.UserVarCloudConflict += HandleRefresh;
		ActivateWatcher.ConnectionStatusChange += HandleConnectionStatusChanged;
	}

	private void OnDisable()
	{
		Bedrock.UserVarCloudConflict -= HandleRefresh;
		ActivateWatcher.ConnectionStatusChange -= HandleConnectionStatusChanged;
	}

	public void Display()
	{
		Refresh();
		SoundEventManager.Instance.Play2D(transitionInSFX);
		UIManager.instance.blockInput = false;
	}

	private void HandleConnectionStatusChanged(object sender, ConnectionStatusChangeEventArgs e)
	{
		if (e.NewStatus != Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_ONLINE)
		{
			ActivateWatcher.Instance.isResolvingConflict = false;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void HandleRefresh(object sender, EventArgs e)
	{
		Refresh();
	}

	private void Refresh()
	{
	}

	private string CheckForNull(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return string.Empty;
		}
		return input;
	}

	private void onlineBtnHit()
	{
		SoundEventManager.Instance.Play2D(OnNoDismissedSound);
		ActivateWatcher.Instance.PullFromTheCloud();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void localBtnHit()
	{
		ActivateWatcher.Instance.isResolvingConflict = false;
		SoundEventManager.Instance.Play2D(OnYesDismissedSound);
		UnityEngine.Object.Destroy(base.gameObject);
		LinkedContentManager.Instance.TryUpdateContentAndDisplayNotification();
	}
}
