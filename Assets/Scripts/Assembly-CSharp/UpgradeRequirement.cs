using System;
using UnityEngine;

public abstract class UpgradeRequirement : MonoBehaviour
{
	public abstract string NotMetText { get; }

	public static event EventHandler RecheckRequirements;

	public abstract bool CheckRequirement();

	protected void OnRecheckRequirements()
	{
		if (UpgradeRequirement.RecheckRequirements != null)
		{
			UpgradeRequirement.RecheckRequirements(this, new EventArgs());
		}
	}
}
