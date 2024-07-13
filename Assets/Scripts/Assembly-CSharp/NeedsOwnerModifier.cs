using System;

public abstract class NeedsOwnerModifier<T> : BountyModifier where T : class
{
	protected T Owner { get; private set; }

	public override void Initialize(Bounty owner)
	{
		base.Initialize(owner);
		Owner = owner as T;
		if (Owner == null)
		{
			throw new Exception("Bounty owner for '" + GetType().ToString() + "' must impement '" + typeof(T).ToString() + "'");
		}
	}
}
