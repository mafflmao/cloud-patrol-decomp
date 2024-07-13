using System.Text;
using UnityEngine;

public abstract class BountyModifier : MonoBehaviour
{
	public abstract bool AllowIncrement();

	public virtual void PerformDescriptionReplacement(StringBuilder stringBuilder)
	{
	}

	public virtual string GetSaveState()
	{
		return "None";
	}

	public virtual void LoadFromSaveState(string saveState)
	{
	}

	public virtual void Initialize(Bounty owner)
	{
	}

	public string GetIdentifier()
	{
		return GetType().ToString();
	}
}
