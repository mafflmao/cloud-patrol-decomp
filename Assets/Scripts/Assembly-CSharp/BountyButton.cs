using System;
using UnityEngine;

public class BountyButton : MonoBehaviour
{
	public GameObject posterPrefab;

	private BountyPoster _poster;

	public UIButton button;

	public bool isAvailable;

	public bool isTimed = true;

	public bool showPoster;

	public string posterClip;

	public float totalTime;

	public string title;

	public string stateToLoad = string.Empty;

	private Bounty _bountyData;

	private GoalController _controller;

	public Bounty bountyData
	{
		get
		{
			return _bountyData;
		}
		set
		{
			_bountyData = value;
			UpdateBountyData();
		}
	}

	private void Awake()
	{
		_controller = (GoalController)UnityEngine.Object.FindObjectOfType(typeof(GoalController));
		button = base.gameObject.GetComponent<UIButton>();
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(posterPrefab, base.transform.position, base.transform.rotation);
		gameObject.transform.parent = base.transform;
		_poster = gameObject.GetComponent<BountyPoster>();
	}

	private void Start()
	{
		if (!showPoster)
		{
			_poster.Hide(true);
		}
		UIManager.AddAction<BountyButton>(button, base.gameObject, "OnBountyBtnClicked", POINTER_INFO.INPUT_EVENT.RELEASE);
	}

	private void UpdateBountyData()
	{
		if (bountyData != null)
		{
			_poster.hideAtStart = false;
			_poster.Hide(false);
			_poster.Bounty = bountyData;
		}
	}

	private void OnBountyBtnClicked()
	{
		_controller.OnBountyBtnClick(this);
	}

	public static string FormatTimeString(float t)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(t);
		return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
	}
}
