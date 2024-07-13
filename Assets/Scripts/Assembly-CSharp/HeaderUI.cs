using System;
using System.Collections.Generic;
using UnityEngine;

public class HeaderUI : MonoBehaviour
{
	public class BackButtonOverrideContext : IDisposable
	{
		private HeaderUI _owner;

		private int _validStackHeight;

		public BackButtonOverrideContext(HeaderUI owner)
		{
			_owner = owner;
			_validStackHeight = owner._backButtonOverrideActionStack.Count;
		}

		public void Dispose()
		{
			if (_owner._backButtonOverrideActionStack.Count != _validStackHeight)
			{
				Debug.Log("Out-of-order disposal of back button override contexts. Back button behaviour will broken!");
			}
			_owner.PopBackButtonOverrideAction();
		}
	}

	public static HeaderUI _inst;

	[SerializeField]
	private bool _visible;

	public SpriteText title;

	public UIButton backBtn;

	private ActivateLoginController activateLoginController;

	public PackedSprite[] bar;

	public GameObject container;

	public UIButton gameCenterLeaderboardButton;

	public SoundEventData ui_BackSoundData;

	private Vector3 _leaderboardButtonStartPosition;

	private Stack<Action> _backButtonOverrideActionStack = new Stack<Action>();

	private string _title = string.Empty;

	public static HeaderUI Instance
	{
		get
		{
			if (_inst == null)
			{
				GameObject gameObject = GameObject.Find("Header");
				if (gameObject == null)
				{
					gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("UI Prefabs/Common/Header"));
					gameObject.name = "Header";
				}
				_inst = gameObject.GetComponent<HeaderUI>();
			}
			return _inst;
		}
	}

	[HideInInspector]
	public bool visible
	{
		get
		{
			return _visible;
		}
		set
		{
			if (_visible != value)
			{
				_visible = value;
				UpdateVisibility();
			}
		}
	}

	public string titleString
	{
		get
		{
			return _title;
		}
		set
		{
			if (_title != value)
			{
				_title = value;
				UpdateTitleText();
			}
		}
	}

	public IDisposable PushBackButtonOverrideAction(Action newAction)
	{
		_backButtonOverrideActionStack.Push(newAction);
		return new BackButtonOverrideContext(this);
	}

	private void PopBackButtonOverrideAction()
	{
		if (_backButtonOverrideActionStack.Count > 0)
		{
			_backButtonOverrideActionStack.Pop();
		}
		else
		{
			Debug.LogError("Mismatched override push/pop. Someone tried to pop the back button override, but there wasn't anything overriding it!!");
		}
	}

	private void Awake()
	{
		if (_inst != null)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
		_inst = this;
		container.gameObject.transform.localPosition = new Vector3(0f, 300f, 0f);
		gameCenterLeaderboardButton.Hide(true);
		_leaderboardButtonStartPosition = gameCenterLeaderboardButton.transform.localPosition;
	}

	private void Start()
	{
		activateLoginController = GetComponentInChildren<ActivateLoginController>();
	}

	private void OnEnable()
	{
		ActivateFriendInviteWatcher.FriendInviteCountUpdated += HandleActivateFriendInviteCountUpdated;
	}

	private void OnDisable()
	{
		ActivateFriendInviteWatcher.FriendInviteCountUpdated -= HandleActivateFriendInviteCountUpdated;
	}

	private void UpdateVisibility()
	{
		if (visible)
		{
			AnimateIn();
		}
		else
		{
			AnimateOut();
		}
	}

	private void HandleActivateFriendInviteCountUpdated(object sender, FriendInviteCountEventArgs e)
	{
		ShiftLeaderboardButtonToLeft(e.NumFriendInvites != 0);
	}

	private void ShiftLeaderboardButtonToLeft(bool isShifted)
	{
		if (gameCenterLeaderboardButton != null)
		{
			Vector3 vector = ((!isShifted) ? Vector3.zero : new Vector3(-90f, 0f, 0f));
			gameCenterLeaderboardButton.transform.localPosition = _leaderboardButtonStartPosition + vector;
		}
	}

	private void Update()
	{
		if (Application.platform == RuntimePlatform.Android && !(GameObject.FindWithTag("ModalDialog") == null))
		{
		}
	}

	private void UpdateTitleText()
	{
		Debug.Log("Title Swap to " + titleString);
		title.Text = titleString;
		iTween.ScaleTo(title.gameObject, iTween.Hash("scale", new Vector3(0.75f, 0.75f, 1f), "time", 0.1f, "easeType", iTween.EaseType.easeInQuad, "oncompletetarget", base.gameObject, "oncomplete", "OnUpdateTitleTextComplete"));
		iTween.ColorTo(title.gameObject, iTween.Hash("color", new Color(1f, 1f, 1f, 0f), "time", 0.1f));
	}

	private void OnUpdateTitleTextComplete()
	{
		title.Text = titleString;
		iTween.ScaleTo(title.gameObject, iTween.Hash("scale", new Vector3(1f, 1f, 1f), "time", 0.1f, "easeType", iTween.EaseType.easeOutQuad));
		iTween.ColorTo(title.gameObject, iTween.Hash("color", new Color(1f, 1f, 1f, 1f), "time", 0.1f));
	}

	public void ShowBackButton(bool showBackButton)
	{
		backBtn.Hide(!showBackButton);
		ShowSocialButtons(showBackButton);
	}

	public void ShowSocialButtons(bool showSocialButtons)
	{
		if (activateLoginController != null)
		{
			activateLoginController.Hide(!showSocialButtons);
		}
	}

	private void OnBackBtnClicked()
	{
		CommonAnimations.AnimateButton(backBtn.gameObject);
		if (_backButtonOverrideActionStack.Count == 0)
		{
			StateManager.Instance.LoadAndActivatePreviousState();
		}
		else
		{
			_backButtonOverrideActionStack.Peek()();
		}
	}

	public void AnimateOut()
	{
		iTween.MoveTo(container.gameObject, iTween.Hash("position", new Vector3(0f, 300f, 0f), "time", 0.5f, "islocal", true));
	}

	public void AnimateIn()
	{
		container.gameObject.transform.localPosition = new Vector3(0f, 300f, 0f);
		iTween.MoveTo(container.gameObject, Vector3.zero, 0.5f);
	}
}
