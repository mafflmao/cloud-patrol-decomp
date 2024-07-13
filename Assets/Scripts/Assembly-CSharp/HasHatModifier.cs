using System.Text;

public class HasHatModifier : NeedsOwnerModifier<IHasHealthScript>
{
	private void OnEnable()
	{
		HatAdder.PushHatRequest();
	}

	private void OnDisable()
	{
		HatAdder.PopHatRequest();
	}

	public override bool AllowIncrement()
	{
		return HatAdder.IsHat(base.Owner.Health);
	}

	public override void PerformDescriptionReplacement(StringBuilder stringBuilder)
	{
		stringBuilder.Replace("{hat}", "top-hats");
	}
}
