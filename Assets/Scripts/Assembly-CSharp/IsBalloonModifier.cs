using System.Text;
using UnityEngine;

public class IsBalloonModifier : NeedsOwnerModifier<IHasHealthScript>
{
	public override bool AllowIncrement()
	{
		return IsHealthAttachedToBalloon(base.Owner.Health);
	}

	public static bool IsHealthAttachedToBalloon(Health healthScript)
	{
		if (healthScript == null)
		{
			Debug.LogError("Null health component passed! This is bad. This might be caused by SplashDamamge.cs.");
			return false;
		}
		Balloon component = healthScript.gameObject.GetComponent<Balloon>();
		return component != null;
	}

	public override void PerformDescriptionReplacement(StringBuilder stringBuilder)
	{
	}
}
