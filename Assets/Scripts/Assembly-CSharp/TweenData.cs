using System.Collections;
using UnityEngine;

public class TweenData : ScriptableObject
{
	public enum TweenType
	{
		FadeTo = 0,
		ScaleTo = 1,
		MoveTo = 2
	}

	public TweenType type;

	public string tweenName;

	public string onstart;

	public GameObject onstarttarget;

	public Object onstartparams;

	public string onupdate;

	public GameObject onupdatetarget;

	public Object onupdateparams;

	public string oncomplete;

	public GameObject oncompletetarget;

	public Object oncompleteparams;

	public virtual Hashtable args
	{
		get
		{
			return new Hashtable();
		}
	}

	public void Play(GameObject target)
	{
		if (target == null)
		{
			Debug.LogError("The game object target in the tween data is null");
			return;
		}
		switch (type)
		{
		case TweenType.FadeTo:
			iTween.FadeTo(target, args);
			break;
		case TweenType.ScaleTo:
			iTween.ScaleTo(target, args);
			break;
		case TweenType.MoveTo:
			iTween.MoveTo(target, args);
			break;
		}
	}

	public void Hash(ref Hashtable h)
	{
		if (base.name != string.Empty)
		{
			h.Add("name", tweenName);
		}
		if (onstart != string.Empty)
		{
			h.Add("onstart", onstart);
		}
		if (onstarttarget != null)
		{
			h.Add("onstarttarget", onstarttarget);
		}
		if (onstartparams != null)
		{
			h.Add("onstartparams", onstartparams);
		}
		if (onupdate != string.Empty)
		{
			h.Add("onupdate", onupdate);
		}
		if (onupdatetarget != null)
		{
			h.Add("onupdatetarget", onupdatetarget);
		}
		if (onupdateparams != null)
		{
			h.Add("onupdateparams", onupdateparams);
		}
		if (oncomplete != string.Empty)
		{
			h.Add("oncomplete", oncomplete);
		}
		if (oncompletetarget != null)
		{
			h.Add("oncompletetarget", oncompletetarget);
		}
		if (oncompleteparams != null)
		{
			h.Add("oncompleteparams", oncompleteparams);
		}
	}
}
