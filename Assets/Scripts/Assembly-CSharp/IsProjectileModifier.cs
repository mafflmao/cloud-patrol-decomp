using System.Text;
using UnityEngine;

public class IsProjectileModifier : NeedsOwnerModifier<IHasHealthScript>
{
	public override bool AllowIncrement()
	{
		return IsHealthAttachedToProjectile(base.Owner.Health);
	}

	public static bool IsHealthAttachedToProjectile(Health healthScript)
	{
		if (healthScript == null)
		{
			Debug.LogError("Null health component passed! This is bad. This might be caused by SplashDamamge.cs.");
			return false;
		}
		return !healthScript.isEnemy && healthScript.gameObject.CompareTag("Projectile");
	}

	public override void PerformDescriptionReplacement(StringBuilder stringBuilder)
	{
	}
}
