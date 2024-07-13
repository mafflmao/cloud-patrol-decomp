using System.Collections;
using UnityEngine;

public class CreditsController : StateController
{
	public UIScrollList scrollListIOS;

	public UIScrollList scrollListKindle;

	[HideInInspector]
	public UIScrollList scrollList;

	public GameObject[] iOSSpecificItems;

	public GameObject[] KindleSpecificItems;

	public GameObject creditsBackground;

	public bool isStopped;

	public float speed = 60f;

	private void Awake()
	{
		scrollListIOS.gameObject.SetActive(true);
		scrollList = scrollListIOS;
		for (int i = 0; i < KindleSpecificItems.Length; i++)
		{
			KindleSpecificItems[i].SetActive(false);
		}
		scrollList.AddInputDelegate(OnScrollListInput);
	}

	private void OnDestroy()
	{
		scrollList.RemoveInputDelegate(OnScrollListInput);
	}

	private void OnScrollListInput(ref POINTER_INFO ptr)
	{
		if (!isStopped && ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
		{
			isStopped = true;
		}
	}

	protected override void ShowState()
	{
		isStopped = false;
		scrollList.ScrollListTo(0f);
		scrollList.transform.localScale = Vector3.zero;
		base.ShowState();
	}

	private void Update()
	{
		if (mActive && !isStopped)
		{
			scrollList.ScrollListTo(scrollList.ScrollPosition + 1f / speed * Time.deltaTime);
			if (!isStopped && scrollList.ScrollPosition >= 0.99f)
			{
				StateManager.Instance.LoadAndActivateState("Help");
			}
		}
	}

	protected override void HideState()
	{
		base.HideState();
	}

	protected override IEnumerator AnimateStateIn()
	{
		UIManager.instance.blockInput = true;
		creditsBackground.GetComponent<Renderer>().sharedMaterial.color = Color.clear;
		iTween.FadeTo(creditsBackground, iTween.Hash("alpha", 0.5f, "time", 0.5f));
		iTween.ScaleTo(scrollList.gameObject, iTween.Hash("scale", Vector3.one, "time", 2f));
		ShowState();
		yield return new WaitForSeconds(2f);
	}

	protected override IEnumerator AnimateStateOut()
	{
		UIManager.instance.blockInput = true;
		iTween.FadeTo(creditsBackground, iTween.Hash("alpha", 0f, "time", 0.5f));
		iTween.ScaleTo(scrollList.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f));
		yield return new WaitForSeconds(1f);
		UIManager.instance.blockInput = false;
		HideState();
	}
}
