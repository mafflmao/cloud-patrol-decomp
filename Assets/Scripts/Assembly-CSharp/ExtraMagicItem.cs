using UnityEngine;

public class ExtraMagicItem : Powerup
{
	protected override void HandleTriggered()
	{
		base.HandleTriggered();
		Debug.LogError("Shouldn't be able to trigger this powerup!...");
	}
}
