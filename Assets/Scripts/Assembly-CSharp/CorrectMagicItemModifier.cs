using System.Text;
using UnityEngine;

public class CorrectMagicItemModifier : NeedsOwnerModifier<IHasMagicItem>
{
	public const string MagicItemPlaceholder = "{magicItem}";

	private PowerupData _neededMagicItem;

	private PowerupList AllPowerups
	{
		get
		{
			return BountyChooser.Instance.allPowerups;
		}
	}

	protected void Awake()
	{
		_neededMagicItem = AllPowerups.ChooseRandomCollectablePowerup();
	}

	public override bool AllowIncrement()
	{
		return base.Owner.PowerupData == _neededMagicItem;
	}

	public override string GetSaveState()
	{
		return _neededMagicItem.name;
	}

	public override void LoadFromSaveState(string saveState)
	{
		foreach (PowerupData powerup in AllPowerups.powerups)
		{
			if (powerup.name.Equals(saveState))
			{
				_neededMagicItem = powerup;
				return;
			}
		}
		Debug.LogError("Unable to parse '" + saveState + "' into powerup. Choosing a new random one.");
		_neededMagicItem = AllPowerups.ChooseRandomCollectablePowerup();
	}

	public override void PerformDescriptionReplacement(StringBuilder builder)
	{
		builder.Replace("{magicItem}", _neededMagicItem.LocalizedName);
	}
}
