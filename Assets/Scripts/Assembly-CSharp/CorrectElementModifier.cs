using System;
using System.Collections.Generic;
using System.Text;

public class CorrectElementModifier : BountyModifier
{
	public const string ElementPlaceholderString = "{element}";

	private Elements.Type _elementType;

	private readonly HashSet<string> _vowels = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "a", "e", "i", "o", "u", "y" };

	private void Awake()
	{
		_elementType = EnumUtils.GetRandomValue<Elements.Type>();
	}

	public void SetElement(Elements.Type element)
	{
		_elementType = element;
	}

	public override bool AllowIncrement()
	{
		return StartGameSettings.Instance.activeSkylander.elementData.elementType == _elementType;
	}

	public override string GetSaveState()
	{
		return _elementType.ToString();
	}

	public override void LoadFromSaveState(string saveState)
	{
		_elementType = EnumUtils.ToEnum(saveState, Elements.Type.Tech);
	}

	public override void PerformDescriptionReplacement(StringBuilder stringBuilder)
	{
		string text = _elementType.ToString();
		if (_vowels.Contains(text[0].ToString()))
		{
			stringBuilder.Replace(" a {element}", " an " + text);
		}
		stringBuilder.Replace("{element}", Elements.GetLocalizedName(_elementType));
	}
}
