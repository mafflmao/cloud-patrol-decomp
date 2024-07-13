using System.Collections;
using UnityEngine;

public class TweenDataScaleTo : TweenData
{
	public Vector3 scale;

	public float time;

	public float delay;

	public iTween.EaseType easetype;

	public iTween.LoopType looptype;

	public override Hashtable args
	{
		get
		{
			Hashtable h = new Hashtable();
			h.Add("scale", scale);
			if (time != -1f)
			{
				h.Add("time", time);
			}
			if (delay != 0f)
			{
				h.Add("delay", delay);
			}
			h.Add("easetype", easetype);
			h.Add("looptype", looptype);
			Hash(ref h);
			return h;
		}
	}

	private void OnEnable()
	{
		type = TweenType.ScaleTo;
	}
}
