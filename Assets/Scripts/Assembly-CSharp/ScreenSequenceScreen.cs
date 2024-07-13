using System.Collections;
using UnityEngine;

public abstract class ScreenSequenceScreen : MonoBehaviour
{
	private bool _startedAnimateIn;

	private bool _startedAnimateOut;

	public static readonly Vector3 ScreenHeightVector = new Vector3(0f, 1536f, 0f);

	public static readonly Vector3 ScreenWidthVector = new Vector3(2048f, 0f, 0f);

	public ScreenSequenceController Owner { get; set; }

	protected virtual void Start()
	{
		AnimateIn();
	}

	protected void StartTimeout(float timeout)
	{
		InvokeHelper.InvokeSafe(Timeout, timeout, this);
	}

	protected void Timeout()
	{
		if (Owner != null)
		{
			Owner.ScreenTimedOut(this);
		}
	}

	protected void Suicide(float delay)
	{
		InvokeHelper.InvokeSafe(Suicide, delay, this);
	}

	private void Suicide()
	{
		Object.Destroy(base.gameObject);
	}

	public void StartAnimateIn()
	{
		if (!_startedAnimateIn)
		{
			_startedAnimateIn = true;
			AnimateIn();
		}
	}

	public void StartAnimateOut()
	{
		if (!_startedAnimateOut)
		{
			_startedAnimateOut = true;
			AnimateOut();
		}
	}

	protected abstract void AnimateIn();

	protected abstract void AnimateOut();

	public virtual bool AllowAdvanceToNextScreenFromUserPress()
	{
		return true;
	}

	public static void MoveTo(GameObject go, Vector3 pos, float time, float delay)
	{
		Hashtable hashtable = iTween.Hash("position", pos, "time", time, "easetype", iTween.EaseType.easeInOutBack);
		if (delay != 0f)
		{
			hashtable.Add("delay", delay);
		}
		iTween.MoveTo(go, hashtable);
	}

	public static void MoveFrom(GameObject go, Vector3 pos, float time, float delay)
	{
		Hashtable hashtable = iTween.Hash("position", pos, "time", time, "easetype", iTween.EaseType.easeInOutBack);
		if (delay != 0f)
		{
			hashtable.Add("delay", delay);
		}
		iTween.MoveFrom(go, hashtable);
	}
}
