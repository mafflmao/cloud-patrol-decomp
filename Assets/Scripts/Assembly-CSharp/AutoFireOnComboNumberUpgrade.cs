using System;

public class AutoFireOnComboNumberUpgrade : CharacterUpgrade
{
	public int comboToFireOn = 6;

	private void OnEnable()
	{
		TargetQueue.TargetAdded += HandleTargetQueueTargetAdded;
	}

	private void OnDisable()
	{
		TargetQueue.TargetAdded -= HandleTargetQueueTargetAdded;
	}

	private void HandleTargetQueueTargetAdded(object sender, EventArgs e)
	{
		TargetQueue targetQueue = (TargetQueue)sender;
		if (targetQueue.Count < comboToFireOn)
		{
			return;
		}
		for (int i = 0; i < ShipManager.instance.shooter.Count; i++)
		{
			if (ShipManager.instance.shooter[i].targetQueue == targetQueue)
			{
				ShipManager.instance.shooter[i].FireAtTargets();
				break;
			}
		}
	}
}
