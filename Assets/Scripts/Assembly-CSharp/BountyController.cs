using System.Collections.Generic;
using UnityEngine;

public class BountyController : MonoBehaviour
{
	private enum State
	{
		FirstTab = 0,
		SecondTab = 1
	}

	public SpriteText tapToSkipText;

	public SpriteText secondTabTitleText;

	public PackedSprite tabs;

	public UIButton3D btnSecondTab;

	public UIButton3D btnFirstTab;

	public UIPanelManager panelManager;

	public UIPanel panelGoals;

	public PrefabPlaceholder[] bountyBarPlaceholders;

	public PrefabPlaceholder leaderboardPlaceholder;

	public PrefabPlaceholder challengesPlaceholder;

	public SoundEventData sfxPanelChanged;

	public ChallengeOverviewDialog challengeOverviewDialog;

	public int gamesBetweenChallengeReminders;

	public string challengeTabReminderString;

	private UIPanel _secondTab;

	private List<BountyBar> _bountyBars = new List<BountyBar>();

	private State _currentTab;

	private void Awake()
	{
	}

	public void Reset()
	{
		panelManager.initialPanel = panelGoals;
		SetState(State.FirstTab);
	}

	private void SetState(State newState)
	{
		_currentTab = newState;
		bool flag = _currentTab == State.FirstTab;
		tabs.PlayAnim((!flag) ? "SecondTab" : "FirstTab");
		btnFirstTab.controlIsEnabled = !flag;
		btnSecondTab.controlIsEnabled = flag;
		panelManager.BringIn((!flag) ? 1 : 0);
		if (!flag && GameManager.gameState != GameManager.GameState.Playing)
		{
			ChallengeOverviewDialog challengeOverviewDialog = (ChallengeOverviewDialog)Object.Instantiate(this.challengeOverviewDialog);
			challengeOverviewDialog.Display();
		}
	}

	private void FirstTabButtonPressed()
	{
		SoundEventManager.Instance.Play2D(sfxPanelChanged);
		SetState(State.FirstTab);
	}

	private void SecondTabButtonPressed()
	{
		SoundEventManager.Instance.Play2D(sfxPanelChanged);
		SetState(State.SecondTab);
	}

	public BountyBar GetBountyBar(int index)
	{
		if (index < _bountyBars.Count && index >= 0)
		{
			return _bountyBars[index];
		}
		return null;
	}

	public void UpdateBounties()
	{
		for (int i = 0; i < Mathf.Min(_bountyBars.Count, BountyChooser.Instance.ActiveBounties.Length); i++)
		{
			_bountyBars[i].Bounty = BountyChooser.Instance.ActiveBounties[i];
		}
	}

	public void SetTabSwitchingEnabled(bool enabled)
	{
		bool flag = _currentTab == State.FirstTab;
		btnFirstTab.controlIsEnabled = enabled && !flag;
		btnSecondTab.controlIsEnabled = enabled && flag;
	}

	public void SetBountyButtonsEnabled(bool enabled)
	{
		foreach (BountyBar bountyBar in _bountyBars)
		{
			bountyBar.ButtonEnabled = enabled;
		}
	}
}
