using System.Text;
using UnityEngine;

public class IsSheepModifier : NeedsOwnerModifier<IHasHealthScript>
{
	public override bool AllowIncrement()
	{
		return IsHealthAttachedToSheep(base.Owner.Health);
	}

	public static bool IsHealthAttachedToSheep(Health healthScript)
	{
		if (healthScript == null)
		{
			Debug.LogError("Null health component passed! This is bad. This might be caused by SplashDamamge.cs.");
			return false;
		}
		MoverBounce component = healthScript.gameObject.GetComponent<MoverBounce>();
		SheepCopter component2 = healthScript.gameObject.GetComponent<SheepCopter>();
		if ((component != null && component.sheepMover) || component2 != null)
		{
			return true;
		}
		return false;
	}

	public override void PerformDescriptionReplacement(StringBuilder stringBuilder)
	{
	}
}
