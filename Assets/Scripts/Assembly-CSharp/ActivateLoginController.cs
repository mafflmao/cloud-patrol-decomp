using UnityEngine;

public class ActivateLoginController : MonoBehaviour
{
	public SimpleSprite activateButton;

	public ParticleSystem activateGlowParticles;

	public ParticleSystem activatePulseParticles;

	public Texture2D buttonDisabledTexture;

	public Texture2D buttonEnabledTexture;

	public UIButtonComposite friendInvitesTag;

	private bool _isHidden;

	private UIButton3D btn;

	[HideInInspector]
	public bool friendInviteTagIsShowing;

	private void Start()
	{
		UpdateFriendTagVisuals();
		UpdateConnectionStatusVisuals();
		btn = friendInvitesTag.GetComponent<UIButton3D>();
	}

	private void OnEnable()
	{
		ActivateWatcher.UserLoggedOn += HandleUserLoggedOn;
		ActivateWatcher.ConnectionStatusChange += HandleConnectionStatusChange;
		ActivateFriendInviteWatcher.FriendInviteCountUpdated += HandleActivateFriendInviteCountUpdated;
	}

	private void OnDisable()
	{
		ActivateWatcher.UserLoggedOn -= HandleUserLoggedOn;
		ActivateWatcher.ConnectionStatusChange -= HandleConnectionStatusChange;
		ActivateFriendInviteWatcher.FriendInviteCountUpdated -= HandleActivateFriendInviteCountUpdated;
	}

	public void Hide(bool doHide)
	{
		UpdateConnectionStatusVisuals();
		activateButton.Hide(doHide);
		_isHidden = doHide;
		if (doHide)
		{
			activateGlowParticles.GetComponent<Renderer>().enabled = false;
			friendInviteTagIsShowing = false;
			friendInvitesTag.Hide(true);
		}
		else if (Bedrock.getUserConnectionStatus() == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_ONLINE)
		{
			if (ActivateFriendInviteWatcher.Instance.friendInviteCount != 0)
			{
				btn.Text = ActivateFriendInviteWatcher.Instance.friendInviteCount.ToString();
				friendInvitesTag.Hide(false);
			}
			activateGlowParticles.GetComponent<Renderer>().enabled = true;
		}
	}

	private void UpdateConnectionStatusVisuals()
	{
		if (Bedrock.getUserConnectionStatus() == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_ONLINE)
		{
			activateGlowParticles.Play();
		}
		else
		{
			activateGlowParticles.Stop();
		}
		if (UserCanClick())
		{
			if (buttonEnabledTexture != null)
			{
				activateButton.SetTexture(buttonEnabledTexture);
			}
		}
		else if (buttonDisabledTexture != null)
		{
			activateButton.SetTexture(buttonDisabledTexture);
		}
	}

	private void UpdateFriendTagVisuals()
	{
		if (ActivateFriendInviteWatcher.Instance.friendInviteCount != 0 && !_isHidden)
		{
			friendInvitesTag.Hide(false);
			if (btn != null)
			{
				btn.Text = ActivateFriendInviteWatcher.Instance.friendInviteCount.ToString();
				friendInviteTagIsShowing = true;
			}
		}
		else
		{
			friendInvitesTag.Hide(true);
			friendInviteTagIsShowing = false;
		}
	}

	private bool UserCanClick()
	{
		return true;
	}

	private void HandleActivateFriendInviteCountUpdated(object sender, FriendInviteCountEventArgs e)
	{
		UpdateFriendTagVisuals();
	}

	private void HandleUserLoggedOn(object sender, LogOnEventArgs e)
	{
		UpdateConnectionStatusVisuals();
	}

	private void HandleConnectionStatusChange(object sender, ConnectionStatusChangeEventArgs e)
	{
		if (e.OldStatus == Bedrock.brUserConnectionStatus.BR_LOGGING_IN_REGISTERED && e.NewStatus == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_ONLINE)
		{
			activatePulseParticles.Play();
		}
		UpdateConnectionStatusVisuals();
	}

	public void OnActivateButtonClick()
	{
		if (UserCanClick())
		{
			if (StateManager.Instance.CurrentStateName == "Results")
			{
				StateManager.Instance.LoadAndActivateState("LoadActivate");
			}
			else
			{
				ActivateWatcher.Instance.ReclaimMemoryThenLaunchActivate(true);
			}
		}
	}
}
