using System.Collections.Generic;
using UnityEngine;

public class BountyScrollList : MonoBehaviour
{
	public GameObject prefabButton;

	public GameObject bountyPosterPrefab;

	public UIButton spinButton;

	[HideInInspector]
	public List<BountyButton> bountyButtonList;

	private UIScrollList _scrollList;

	private void Awake()
	{
		_scrollList = base.gameObject.GetComponent<UIScrollList>();
	}

	private void Start()
	{
		bountyButtonList = new List<BountyButton>();
		Bounty[] currentBounties = BountyChooser.Instance.GetCurrentBounties();
		foreach (Bounty bountyData in currentBounties)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(prefabButton);
			_scrollList.AddItem(gameObject);
			BountyButton componentInChildren = gameObject.GetComponentInChildren<BountyButton>();
			componentInChildren.bountyData = bountyData;
			componentInChildren.isTimed = false;
			bountyButtonList.Add(componentInChildren);
		}
	}

	public void Show()
	{
		iTween.FadeTo(base.gameObject, iTween.Hash("alpha", 1f, "time", 0.25f, "easeType", iTween.EaseType.easeInQuad));
		EnableButtons();
	}

	public void Hide()
	{
		_scrollList.CancelDrag();
		iTween.FadeTo(base.gameObject, iTween.Hash("alpha", 0.5f, "time", 0.25f, "easeType", iTween.EaseType.easeOutQuad));
		DisableButtons();
	}

	private void DisableButtons()
	{
		_scrollList.controlIsEnabled = false;
		BaseItemStoreButton[] componentsInChildren = _scrollList.GetComponentsInChildren<BaseItemStoreButton>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DisableButton();
		}
	}

	public void UpdateBounties()
	{
		int num = 0;
		foreach (BountyButton bountyButton in bountyButtonList)
		{
			bountyButton.bountyData = BountyChooser.Instance.ActiveBounties[num];
			num++;
		}
	}

	private void EnableButtons()
	{
		_scrollList.controlIsEnabled = true;
		BaseItemStoreButton[] componentsInChildren = _scrollList.GetComponentsInChildren<BaseItemStoreButton>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].EnableButton();
		}
	}
}
