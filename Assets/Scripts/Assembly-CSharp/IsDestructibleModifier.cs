using UnityEngine;

public class IsDestructibleModifier : NeedsOwnerModifier<IHasHealthScript>
{
	public Destructible.Type? RequiredType { get; set; }

	public override bool AllowIncrement()
	{
		return IsHealthAttachedToDestructible(base.Owner.Health, RequiredType);
	}

	public static bool IsHealthAttachedToDestructible(Health healthComponent)
	{
		return IsHealthAttachedToDestructible(healthComponent, null);
	}

	public static bool IsHealthAttachedToDestructible(Health healthComponent, Destructible.Type? requiredType)
	{
		if (healthComponent == null)
		{
			Debug.LogError("Null health component passed! This is bad. This might be caused by SplashDamamge.cs.");
			return false;
		}
		Destructible component = healthComponent.gameObject.GetComponent<Destructible>();
		if (component == null)
		{
			return false;
		}
		if (!requiredType.HasValue)
		{
			return true;
		}
		return component.destructibleType == requiredType.GetValueOrDefault() && requiredType.HasValue;
	}
}
