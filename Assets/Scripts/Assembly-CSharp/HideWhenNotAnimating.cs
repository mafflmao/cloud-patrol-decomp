using UnityEngine;

[RequireComponent(typeof(Animation))]
public class HideWhenNotAnimating : MonoBehaviour
{
	private void Start()
	{
		Hide();
	}

	public void Show()
	{
		foreach (Transform item in base.transform)
		{
			if ((bool)item.GetComponent<Renderer>())
			{
				item.GetComponent<Renderer>().enabled = true;
			}
		}
		if ((bool)base.GetComponent<Animation>().clip)
		{
			StopAllCoroutines();
			InvokeHelper.InvokeSafe(Hide, base.GetComponent<Animation>().clip.length, this);
		}
		else
		{
			Debug.Log(base.gameObject.name + "doesn't havea  default clip, so it won't be hiding when its animation is done.");
		}
	}

	private void Hide()
	{
		foreach (Transform item in base.transform)
		{
			if ((bool)item.GetComponent<Renderer>())
			{
				item.GetComponent<Renderer>().enabled = false;
			}
		}
	}
}
