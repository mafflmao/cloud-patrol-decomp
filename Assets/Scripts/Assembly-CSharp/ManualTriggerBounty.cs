using System;

public class ManualTriggerBounty : Bounty
{
	public const string TutorialCompleteId = "TutorialComplete";

	private static Action<string> TriggerDelegate;

	public string TriggerId { get; set; }

	private void OnEnable()
	{
		TriggerDelegate = (Action<string>)Delegate.Combine(TriggerDelegate, new Action<string>(HandleTriggered));
	}

	private void OnDisable()
	{
		TriggerDelegate = (Action<string>)Delegate.Remove(TriggerDelegate, new Action<string>(HandleTriggered));
	}

	private void HandleTriggered(string triggerId)
	{
		if (triggerId == TriggerId)
		{
			TryIncrementProgress();
		}
	}

	public static void Trigger(string triggerId)
	{
		if (TriggerDelegate != null)
		{
			TriggerDelegate(triggerId);
		}
	}
}
