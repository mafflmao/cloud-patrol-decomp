using System;
using UnityEngine;

[Serializable]
public class UIActionInfo
{
	public MonoBehaviour scriptWithMethodToInvoke;

	public string methodToInvoke;

	public string sArgument;

	public POINTER_INFO.INPUT_EVENT whenToInvoke = POINTER_INFO.INPUT_EVENT.TAP;

	public float delay;

	private bool mIsProcessing;

	public bool IsProcessing
	{
		get
		{
			return mIsProcessing;
		}
		set
		{
			mIsProcessing = value;
		}
	}
}
