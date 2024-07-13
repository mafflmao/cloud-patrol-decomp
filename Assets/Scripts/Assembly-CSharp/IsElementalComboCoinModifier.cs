using System.Text;

public class IsElementalComboCoinModifier : NeedsOwnerModifier<IHasComboCoin>
{
	public const string ElementNamePlaceholder = "{element}";

	public Elements.Type? RequiredElement { get; set; }

	public override bool AllowIncrement()
	{
		if (!base.Owner.ComboCoin.isElemental)
		{
			return false;
		}
		if (RequiredElement.HasValue)
		{
			return StartGameSettings.Instance.activeSkylander.elementData.elementType == RequiredElement.Value;
		}
		return true;
	}

	public override void PerformDescriptionReplacement(StringBuilder stringBuilder)
	{
		base.PerformDescriptionReplacement(stringBuilder);
		if (RequiredElement.HasValue)
		{
			stringBuilder.Replace("{element}", Elements.GetLocalizedName(RequiredElement.Value));
		}
	}
}
