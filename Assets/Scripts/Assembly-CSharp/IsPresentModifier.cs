public class IsPresentModifier : NeedsOwnerModifier<IHasHealthScript>
{
	public override bool AllowIncrement()
	{
		return base.Owner.Health.GetComponent<PresentBox>() != null;
	}
}
