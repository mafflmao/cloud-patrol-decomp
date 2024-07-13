using System;
using UnityEngine;

public class LoadActivateController : StateController
{
	private bool returnToLoadout = true;

	public GameObject backgroundObject;

	private void OnEnable()
	{
		Bedrock.BedrockUIClosed += HandleBedrockBedrockUIClosed;
		ActivateWatcher.UserLoggedOff += HandleUserLoggedOff;
	}

	private void OnDisable()
	{
		Bedrock.BedrockUIClosed -= HandleBedrockBedrockUIClosed;
		ActivateWatcher.UserLoggedOff -= HandleUserLoggedOff;
	}

	protected override void ShowState()
	{
		base.ShowState();
		LoadingPanel instanceAutoCreate = LoadingPanel.InstanceAutoCreate;
		instanceAutoCreate.DismissIfNotLoading = false;
		instanceAutoCreate.DismissOnStateChange = true;
		ActivateWatcher.Instance.ReclaimMemoryThenLaunchActivate(true);
	}

	private void HandleBedrockBedrockUIClosed(object sender, Bedrock.brUserInterfaceReasonForCloseEventArgs e)
	{
		if (returnToLoadout)
		{
			StateManager.Instance.LoadAndActivateState("Loadout");
		}
		UnityEngine.Object.Destroy(backgroundObject);
	}

	private void HandleUserLoggedOff(object sender, EventArgs e)
	{
		returnToLoadout = false;
	}
}
