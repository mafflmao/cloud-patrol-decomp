using UnityEngine;

public class UIBackground : SingletonMonoBehaviour
{
	public enum SkyTime
	{
		DAY = 0,
		NIGHT = 1
	}

	public GameObject skyBase;

	public Color startColor;

	public Color endColor;

	public float fadeTime;

	public static UIBackground Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<UIBackground>();
		}
	}

	public void FadeTo(SkyTime tod)
	{
		if (tod == SkyTime.NIGHT)
		{
			iTween.ColorTo(skyBase, iTween.Hash("name", "SkyColorFade", "color", endColor, "time", fadeTime, "includechildren", false));
		}
		else
		{
			iTween.ColorTo(skyBase, iTween.Hash("name", "SkyColorFade", "color", startColor, "time", fadeTime, "includechildren", false));
		}
	}
}
