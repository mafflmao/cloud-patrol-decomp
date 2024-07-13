using System.Text;

public class IsEnemyModifier : NeedsOwnerModifier<IHasHealthScript>
{
	private string _enemyType;

	public override bool AllowIncrement()
	{
		if (base.Owner.Health.isEnemy && base.Owner.Health.GetComponent<IsShootablePropComponent>() == null)
		{
			switch (_enemyType)
			{
			default:
				return true;
			case "Dodger":
				return EnemyUtils.EnemyIsOfType(base.Owner.Health, EnemyModifierType.Dodger);
			case "Shield":
				return EnemyUtils.EnemyIsOfType(base.Owner.Health, EnemyModifierType.Shield);
			case "Shooter":
				return EnemyUtils.EnemyIsOfType(base.Owner.Health, EnemyModifierType.Shooter);
			case "Lobber":
				return EnemyUtils.EnemyIsOfType(base.Owner.Health, EnemyModifierType.Lobber);
			case "Flier":
				return EnemyUtils.EnemyIsOfType(base.Owner.Health, EnemyModifierType.Flier);
			}
		}
		return false;
	}

	public void SetEnemyType(string enemyTypeName)
	{
		_enemyType = enemyTypeName;
	}

	public override void LoadFromSaveState(string saveState)
	{
		_enemyType = saveState;
	}

	public override string GetSaveState()
	{
		return _enemyType;
	}

	public override void PerformDescriptionReplacement(StringBuilder stringBuilder)
	{
		stringBuilder.Replace("{enemy}", _enemyType);
	}
}
