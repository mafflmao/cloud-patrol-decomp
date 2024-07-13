using System.Collections;

public class TweenDataFadeTo : TweenData
{
	public float alpha = 1f;

	public bool includechildren = true;

	public string NamedValueColor = "_Color";

	public float time = -1f;

	public float delay;

	public iTween.EaseType easetype;

	public iTween.LoopType looptype;

	public override Hashtable args
	{
		get
		{
			Hashtable h = new Hashtable();
			h.Add("alpha", alpha);
			h.Add("includechildren", includechildren);
			h.Add("NamedValueColor", NamedValueColor);
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
}
