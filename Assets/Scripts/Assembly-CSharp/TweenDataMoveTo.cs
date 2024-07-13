using System.Collections;
using UnityEngine;

public class TweenDataMoveTo : TweenData
{
	public Vector3 position;

	public bool islocal;

	public float time = -1f;

	public float delay;

	public iTween.EaseType easetype;

	public iTween.LoopType looptype;

	public override Hashtable args
	{
		get
		{
			Hashtable h = new Hashtable();
			h.Add("position", position);
			h.Add("islocal", islocal);
			h.Add("time", time);
			h.Add("delay", delay);
			Hash(ref h);
			return h;
		}
	}
}
