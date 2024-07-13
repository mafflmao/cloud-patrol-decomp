using UnityEngine;

public class GoalController : StateController
{
	public BountyCallout callout;

	public BountyScrollList scrollList;

	public ScrollListController scrollListController;

	public Transform buttonLocator;

	private BountyButton _currButton;

	public UIButton btnRefresh;

	public PackedSprite iconNotification;

	public BountyMessageBox messageBox;

	public int spinCost;

	protected override void HideState()
	{
		callout.Hide();
		base.HideState();
	}

	protected override void ShowState()
	{
		base.ShowState();
		scrollListController.Reset();
		UIManager.AddAction<GoalController>(btnRefresh, base.gameObject, "OnRefreshBtnClick", POINTER_INFO.INPUT_EVENT.RELEASE);
		UIManager.AddAction<GoalController>(callout.btnOkay, base.gameObject, "OnCalloutOkayBtnClick", POINTER_INFO.INPUT_EVENT.RELEASE);
		callout.callout.cancelButton.scriptWithMethodToInvoke = this;
		callout.callout.cancelButton.methodToInvoke = "OnCalloutCancelBtnClick";
	}

	public void OnBountyBtnClick(BountyButton btn)
	{
		_currButton = btn;
		if (btn.bountyData == null)
		{
			OnMoveToPowerupStore();
			return;
		}
		scrollListController.RemoveButton(_currButton.button);
		callout.bounty = btn.bountyData;
		callout.Show();
		scrollListController.FadeOut();
	}

	public void OnCalloutBtnClick(bool confirm)
	{
		scrollListController.ReplaceButtonAndFadeIn();
		callout.Hide();
	}

	public void OnMoveToPowerupStore()
	{
		callout.Hide();
		StateManager.Instance.LoadAndActivateState("Loadout");
	}

	private void OnRefreshBtnClick()
	{
		messageBox.Show();
	}

	private void OnCalloutOkayBtnClick()
	{
		OnMoveToPowerupStore();
	}

	private void OnCalloutCancelBtnClick()
	{
		OnCalloutBtnClick(false);
	}

	private void SpinPay()
	{
		AchievementManager.Instance.autoSync = true;
		AchievementManager.Instance.IncrementStepBy(Achievements.CoinsSpend, spinCost);
		AchievementManager.Instance.autoSync = false;
		messageBox.Hide();
		scrollList.UpdateBounties();
	}

	private void SpinFree()
	{
		scrollList.UpdateBounties();
		messageBox.Hide();
	}
}
