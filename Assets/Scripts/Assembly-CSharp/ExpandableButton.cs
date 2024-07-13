using System.Collections.Generic;
using UnityEngine;

public class ExpandableButton : MonoBehaviour
{
	public AutoSpriteControlBase[] items;

	private List<Vector3> startingPositions;

	private UIButton thisUIButton;

	public SoundEventData sfxExpand;

	public SoundEventData sfxCollapse;

	public bool expanded;

	private float incrementingDelay;

	private void Start()
	{
		thisUIButton = GetComponent<UIButton>();
		if (thisUIButton == null)
		{
			Debug.LogError("Can't find UIButton to set.");
		}
		else
		{
			UIManager.AddAction<ExpandableButton>(thisUIButton, base.gameObject, "ToggleExpand", POINTER_INFO.INPUT_EVENT.RELEASE);
		}
		startingPositions = new List<Vector3>();
		AutoSpriteControlBase[] array = items;
		foreach (AutoSpriteControlBase autoSpriteControlBase in array)
		{
			startingPositions.Add(autoSpriteControlBase.transform.localPosition);
			autoSpriteControlBase.transform.localPosition = new Vector3(0f, 0f, autoSpriteControlBase.transform.localPosition.z);
			autoSpriteControlBase.controlIsEnabled = false;
			autoSpriteControlBase.Hide(true);
		}
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
		thisUIButton.controlIsEnabled = false;
		SetExpand(expanded);
	}

	private void SetExpand(bool exp)
	{
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
				items[i].controlIsEnabled = false;
				iTween.MoveTo(items[i].gameObject, iTween.Hash("islocal", true, "position", startingPositions[i], "time", 0.3f, "delay", incrementingDelay));
				if (i == items.Length - 1)
				{
					iTween.MoveTo(items[i].gameObject, iTween.Hash("islocal", true, "position", startingPositions[i], "time", 0.3f, "delay", incrementingDelay, "oncomplete", "OnCompleteShow", "oncompletetarget", base.gameObject));
				}
			}
			else
			{
				iTween.MoveTo(items[i].gameObject, iTween.Hash("islocal", true, "position", new Vector3(0f, 0f, items[i].transform.localPosition.z), "time", 0.3f, "delay", incrementingDelay, "oncompletetarget", base.gameObject, "oncomplete", "OnCompleteHide", "oncompleteparams", items[i]));
				items[i].controlIsEnabled = false;
			}
			incrementingDelay += 0.05f;
		}
	}

	private void OnCompleteShow()
	{
		AutoSpriteControlBase[] array = items;
		foreach (AutoSpriteControlBase autoSpriteControlBase in array)
		{
			autoSpriteControlBase.controlIsEnabled = true;
		}
		thisUIButton.controlIsEnabled = true;
	}

	private void OnCompleteHide(AutoSpriteControlBase item)
	{
		item.Hide(true);
		if (item == items[items.Length - 1])
		{
			thisUIButton.controlIsEnabled = true;
		}
	}
}
