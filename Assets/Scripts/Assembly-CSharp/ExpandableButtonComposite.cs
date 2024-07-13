using System.Collections.Generic;
using UnityEngine;

public class ExpandableButtonComposite : MonoBehaviour
{
	public UIButtonComposite[] items;

	private List<Vector3> startingPositions;

	private UIButtonComposite thisUIButton;

	public SoundEventData sfxExpand;

	public SoundEventData sfxCollapse;

	public bool expanded;

	private float incrementingDelay;

	private void Start()
	{
		thisUIButton = GetComponent<UIButtonComposite>();
		if (thisUIButton == null)
		{
			Debug.LogError("Can't find UIButton to set.");
		}
		else
		{
			thisUIButton.UIButton3D.methodToInvoke = "ToggleExpand";
			thisUIButton.UIButton3D.scriptWithMethodToInvoke = this;
		}
		startingPositions = new List<Vector3>();
		UIButtonComposite[] array = items;
		foreach (UIButtonComposite uIButtonComposite in array)
		{
			startingPositions.Add(uIButtonComposite.transform.localPosition);
		}
		CollapseImmediately();
	}

	private void ToggleExpand()
	{
		if (!expanded)
		{
			expanded = true;
		}
		else
		{
			expanded = false;
		}
		thisUIButton.IsButtonColliderEnabled = false;
		SetExpand(expanded);
	}

	private void SetExpand(bool exp)
	{
		expanded = exp;
		if (exp)
		{
			SoundEventManager.Instance.Play2D(sfxExpand);
		}
		else
		{
			SoundEventManager.Instance.Play2D(sfxCollapse);
		}
		incrementingDelay = 0f;
		for (int i = 0; i < items.Length; i++)
		{
			if (exp)
			{
				items[i].Hide(false);
				items[i].IsButtonColliderEnabled = false;
				iTween.MoveTo(items[i].gameObject, iTween.Hash("islocal", true, "position", startingPositions[i], "time", 0.3f, "delay", incrementingDelay));
				if (i == items.Length - 1)
				{
					iTween.MoveTo(items[i].gameObject, iTween.Hash("islocal", true, "position", startingPositions[i], "time", 0.3f, "delay", incrementingDelay, "oncomplete", "OnCompleteShow", "oncompletetarget", base.gameObject));
				}
			}
			else
			{
				iTween.MoveTo(items[i].gameObject, iTween.Hash("islocal", true, "position", new Vector3(0f, 0f, items[i].transform.localPosition.z), "time", 0.3f, "delay", incrementingDelay, "oncompletetarget", base.gameObject, "oncomplete", "OnCompleteHide", "oncompleteparams", items[i]));
				items[i].IsButtonColliderEnabled = false;
			}
			incrementingDelay += 0.05f;
		}
	}

	public void CollapseImmediately()
	{
		UIButtonComposite[] array = items;
		foreach (UIButtonComposite uIButtonComposite in array)
		{
			iTween.Stop(uIButtonComposite.gameObject);
			uIButtonComposite.transform.localPosition = new Vector3(0f, 0f, uIButtonComposite.transform.localPosition.z);
			uIButtonComposite.IsButtonColliderEnabled = false;
			uIButtonComposite.Hide(true);
		}
		expanded = false;
	}

	private void OnCompleteShow()
	{
		UIButtonComposite[] array = items;
		foreach (UIButtonComposite uIButtonComposite in array)
		{
			uIButtonComposite.IsButtonColliderEnabled = true;
		}
		thisUIButton.IsButtonColliderEnabled = true;
	}

	private void OnCompleteHide(UIButtonComposite item)
	{
		item.Hide(true);
		if (item == items[items.Length - 1])
		{
			thisUIButton.IsButtonColliderEnabled = true;
		}
	}
}
