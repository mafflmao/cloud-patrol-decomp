using UnityEngine;

public class PowerUpScrollList : MonoBehaviour
{
	private UIScrollList _scrollList;

	public GameObject powerupPrefab;

	public GameObject consumablePrefab;

	public PowerupList powerupList;

	private void Awake()
	{
		_scrollList = base.gameObject.GetComponent<UIScrollList>();
		if (_scrollList == null)
		{
			Debug.LogError("Could not get the scroll list component. Please make sure one is attached");
		}
	}

	private void Start()
	{
		foreach (PowerupData powerup in powerupList.powerups)
		{
			AddPowerup(powerup);
		}
	}

	public BaseItemStoreButton AddPowerup(PowerupData powerupData)
	{
		GameObject gameObject = ((!(powerupData != null) || !(consumablePrefab != null) || powerupData.Type != PowerupData.ItemType.Consumable) ? ((GameObject)Object.Instantiate(powerupPrefab)) : ((GameObject)Object.Instantiate(consumablePrefab)));
		_scrollList.AddItem(gameObject);
		BaseItemStoreButton componentInChildren = gameObject.GetComponentInChildren<BaseItemStoreButton>();
		if (componentInChildren == null)
		{
			Debug.LogError("Could not locate the power up button component");
			return null;
		}
		if (powerupData != null)
		{
			powerupData.UpdateDataFromSwrve();
		}
		componentInChildren.powerupData = powerupData;
		return componentInChildren;
	}

	public void Show()
	{
		iTween.FadeTo(base.gameObject, iTween.Hash("alpha", 1f, "time", 0.25f, "easeType", iTween.EaseType.easeInQuad));
		iTween.ScaleTo(base.gameObject, iTween.Hash("scale", Vector3.one, "time", 0.25f, "easeType", iTween.EaseType.easeInQuad));
		EnableButtons();
	}

	public void Hide()
	{
		_scrollList.CancelDrag();
		iTween.FadeTo(base.gameObject, iTween.Hash("alpha", 0.5f, "time", 0.25f, "easeType", iTween.EaseType.easeOutQuad));
		iTween.ScaleTo(base.gameObject, iTween.Hash("scale", new Vector3(0.7f, 0.7f, 1f), "time", 0.25f, "easeType", iTween.EaseType.easeOutQuad));
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
