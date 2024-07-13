using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Bounty : MonoBehaviour
{
	public BountyData bountyData;

	public Texture2D iconTexture;

	private bool _isInitialized;

	private BountyModifier[] _modifiersInternal;

	public bool RememberProgress
	{
		get
		{
			return !HasModifierOfType<InSingleRunModifier>();
		}
	}

	public bool IsComplete { get; private set; }

	public int Progress { get; private set; }

	private BountyModifier[] Modifiers
	{
		get
		{
			if (_modifiersInternal == null)
			{
				throw new Exception("Someone's trying to get modifiers, but bounty is not initialized yet...");
			}
			return _modifiersInternal;
		}
	}

	public static event EventHandler<EventArgs> BountyComplete;

	public static event EventHandler<EventArgs> BountyProgressUpdated;

	private void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!_isInitialized)
		{
			_isInitialized = true;
			_modifiersInternal = GetComponents<BountyModifier>().ToArray();
			BountyModifier[] modifiers = Modifiers;
			foreach (BountyModifier bountyModifier in modifiers)
			{
				bountyModifier.Initialize(this);
			}
		}
	}

	public bool HasModifierOfType<T>() where T : BountyModifier
	{
		Type typeFromHandle = typeof(T);
		BountyModifier[] modifiers = Modifiers;
		foreach (BountyModifier bountyModifier in modifiers)
		{
			if (bountyModifier.GetType() == typeFromHandle || bountyModifier.GetType().IsSubclassOf(typeFromHandle))
			{
				return true;
			}
		}
		return false;
	}

	public void OnBountyComplete()
	{
		if (IsComplete)
		{
			return;
		}
		IsComplete = true;
		if (Bounty.BountyComplete != null)
		{
			Bounty.BountyComplete(this, new EventArgs());
		}
		AchievementManager.Instance.IncrementStep(Achievements.BountySmall);
		AchievementManager.Instance.IncrementStep(Achievements.BountyMedium);
		AchievementManager.Instance.IncrementStep(Achievements.BountyLarge);
		DateTime now = DateTime.Now;
		int num = now.Year * 365 + now.DayOfYear;
		List<int> list = new List<int>();
		bool flag = false;
		if (list.Count == 0)
		{
			Debug.Log("First bounty date, just add it");
			list.Add(num);
			flag = true;
		}
		else if (list.Count < 7)
		{
			int num2 = list[list.Count - 1];
			if (num > num2)
			{
				Debug.Log("Earned bounty on newer date, adding: " + num + " vs. " + num2);
				list.Add(num);
				flag = true;
			}
			else
			{
				Debug.Log("Earned bounty date not newer");
			}
		}
		else
		{
			int num3 = list[list.Count - 1];
			if (num > num3)
			{
				Debug.Log("Earned bounty on newer date, removing oldest date");
				list.RemoveAt(0);
				Debug.Log("Adding newer bounty date");
				list.Add(num);
				flag = true;
			}
			else
			{
				Debug.Log("Earned bounty date not newer");
			}
		}
		if (flag && list.Count == 7)
		{
			bool flag2 = true;
			int num4 = list[0] + 1;
			for (int i = 1; i < 7; i++)
			{
				Debug.Log("Checking date " + list[i].ToString() + " against " + num4);
				if (list[i] != num4)
				{
					flag2 = false;
					break;
				}
				num4++;
			}
			if (flag2)
			{
				AchievementManager.Instance.SetStep(Achievements.BountyWeek, 1);
			}
		}
		int step = 0;
		AchievementManager.Instance.SetStep(Achievements.BountyElements, step);
		BountyChooser.Instance.RememberBountyComplete(bountyData);
		base.enabled = false;
	}

	protected void OnBountyProgressUpdated()
	{
		if (Bounty.BountyProgressUpdated != null)
		{
			Bounty.BountyProgressUpdated(this, new EventArgs());
		}
	}

	public void SetProgressFromLoad(int totalProgress)
	{
		Progress = totalProgress;
		IsComplete = totalProgress >= bountyData.Goal;
	}

	private void SetProgress(int totalProgresss)
	{
		if (!IsComplete && Progress != totalProgresss)
		{
			Progress = totalProgresss;
			OnBountyProgressUpdated();
			if (Progress >= bountyData.Goal)
			{
				OnBountyComplete();
			}
		}
	}

	protected void TryIncrementProgress()
	{
		TryIncrementProgress(1);
	}

	protected void TryIncrementProgress(int amount)
	{
		bool flag = true;
		BountyModifier[] modifiers = Modifiers;
		foreach (BountyModifier bountyModifier in modifiers)
		{
			if (!bountyModifier.AllowIncrement())
			{
				flag = false;
			}
		}
		if (flag)
		{
			SetProgress(Progress + amount);
		}
	}

	public void ResetProgress()
	{
		SetProgress(0);
	}

	public string GetSaveState()
	{
		StringBuilder stringBuilder = new StringBuilder(GetSaveStateInternal());
		BountyModifier[] modifiers = Modifiers;
		foreach (BountyModifier bountyModifier in modifiers)
		{
			stringBuilder.Append(",").Append(bountyModifier.GetSaveState());
		}
		return stringBuilder.ToString();
	}

	public void LoadState(string saveState)
	{
		if (string.IsNullOrEmpty(saveState))
		{
			return;
		}
		string[] array = saveState.Split(',');
		LoadFromSaveState(array[0]);
		if (array.Length - 1 < Modifiers.Length)
		{
			Debug.LogError("Not enough parts in '" + saveState + "' to load data for " + Modifiers.Length + " modifiers.");
			return;
		}
		for (int i = 0; i < Modifiers.Length; i++)
		{
			try
			{
				Modifiers[i].LoadFromSaveState(array[i + 1]);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Concat("Caught exception while trying to load state for bounty '", bountyData, "':\n", ex));
			}
		}
	}

	public string GetIdentifier()
	{
		StringBuilder stringBuilder = new StringBuilder(GetType().ToString() + "." + bountyData.Goal + "." + bountyData.Reward);
		BountyModifier[] modifiers = Modifiers;
		foreach (BountyModifier bountyModifier in modifiers)
		{
			stringBuilder.Append(",").Append(bountyModifier.GetType().ToString());
		}
		return stringBuilder.ToString();
	}

	public string GetFormattedStoreDescription()
	{
		StringBuilder stringBuilder = new StringBuilder(LocalizationManager.Instance.GetString(bountyData.LocalizedDescription).Trim());
		PerformDescriptionReplacement(stringBuilder);
		if (Progress != 0 && !IsComplete)
		{
			string @string = LocalizationManager.Instance.GetString("GOAL_REMAINING_TEXT");
			stringBuilder.AppendFormat(" [#F7F28C]{0} {1}", bountyData.Goal - Progress, @string);
		}
		else if (IsComplete)
		{
			string string2 = LocalizationManager.Instance.GetString("GOAL_COMPLETED_TEXT");
			stringBuilder.AppendFormat(" [#F7F28C]{0}", string2);
		}
		BountyModifier[] modifiers = Modifiers;
		foreach (BountyModifier bountyModifier in modifiers)
		{
			bountyModifier.PerformDescriptionReplacement(stringBuilder);
		}
		return stringBuilder.ToString().ToUpper();
	}

	protected virtual void PerformDescriptionReplacement(StringBuilder builder)
	{
		builder.Replace("{goal}", bountyData.Goal.ToString());
	}

	protected virtual string GetSaveStateInternal()
	{
		return "No State";
	}

	public virtual void LoadFromSaveState(string saveState)
	{
	}
}
