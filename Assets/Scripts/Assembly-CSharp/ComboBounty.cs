using System.Text;
using UnityEngine;

public class ComboBounty : Bounty
{
	private enum MatchType
	{
		Exact = 0,
		EqualToOrGreaterThan = 1,
		EqualToOrLessThan = 2
	}

	private int comboNumber = 6;

	private MatchType _matchType;

	private void Start()
	{
		if (comboNumber == -1)
		{
			comboNumber = 6;
		}
	}

	private void OnEnable()
	{
		Shooter.Shooting += HandleShooterShooting;
	}

	private void OnDisable()
	{
		Shooter.Shooting -= HandleShooterShooting;
	}

	private void HandleShooterShooting(object sender, Shooter.ShootEventArgs e)
	{
		switch (_matchType)
		{
		case MatchType.Exact:
			if (e.ComboSize != comboNumber)
			{
				break;
			}
			if (comboNumber == 1 && e.Targets[0] != null)
			{
				Health component = e.Targets[0].GetComponent<Health>();
				if (component != null && component.isEnemy)
				{
					TryIncrementProgress();
				}
			}
			else
			{
				TryIncrementProgress();
			}
			break;
		case MatchType.EqualToOrLessThan:
			if (e.ComboSize <= comboNumber)
			{
				TryIncrementProgress();
			}
			break;
		case MatchType.EqualToOrGreaterThan:
			if (e.ComboSize >= comboNumber)
			{
				TryIncrementProgress();
			}
			break;
		}
	}

	public void ParseFromBountyParams(string bountyParams)
	{
		string text = bountyParams;
		if (bountyParams.Length > 0)
		{
			if (bountyParams[bountyParams.Length - 1] == '+')
			{
				_matchType = MatchType.EqualToOrGreaterThan;
				text = bountyParams.Substring(0, bountyParams.Length - 1);
			}
			else if (bountyParams[bountyParams.Length - 1] == '-')
			{
				_matchType = MatchType.EqualToOrLessThan;
				text = bountyParams.Substring(0, bountyParams.Length - 1);
			}
		}
		if (!int.TryParse(text, out comboNumber))
		{
			Debug.LogError("Unable to parse '" + text + "' into meaningfull combo limit. Assuming Max.");
			comboNumber = 6;
		}
	}

	protected override void PerformDescriptionReplacement(StringBuilder builder)
	{
		base.PerformDescriptionReplacement(builder);
		builder.Replace("{combo}", comboNumber.ToString());
	}
}
