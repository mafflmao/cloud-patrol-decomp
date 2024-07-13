using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Weblink Button")]
public class UIBtnWWW : UIButton
{
	public string URL;

	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (!deleted)
		{
			base.OnInput(ref ptr);
			if (m_controlIsEnabled && !IsHidden() && EventMatchWithButton(ptr.evt))
			{
				Invoke("DoURL", actionInfo[0].delay);
			}
		}
	}

	protected void DoURL()
	{
		Application.OpenURL(URL);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);
		if (s is UIBtnWWW)
		{
			UIBtnWWW uIBtnWWW = (UIBtnWWW)s;
			if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
			{
				URL = uIBtnWWW.URL;
			}
		}
	}

	public new static UIBtnWWW Create(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		return (UIBtnWWW)gameObject.AddComponent(typeof(UIBtnWWW));
	}

	public new static UIBtnWWW Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.position = pos;
		gameObject.transform.rotation = rotation;
		return (UIBtnWWW)gameObject.AddComponent(typeof(UIBtnWWW));
	}
}
