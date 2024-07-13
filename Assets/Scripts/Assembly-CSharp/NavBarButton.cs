using System;
using System.Collections;
using UnityEngine;

public class NavBarButton : MonoBehaviour
{
	public PackedSprite icon;

	public SpriteText txtCount;

	public UIScrollList scrollList;

	public UIListItemContainer scrollItem;

	public UIButton3D button;

	public SoundEventData sfxClick;

	public Elements.Type elementType;

	private void Awake()
	{
		button.scriptWithMethodToInvoke = this;
		button.methodToInvoke = "OnNavBarBtnClick";
	}

	private void OnEnable()
	{
		Bedrock.UnlockContentChanged += HandleUnlockContentChanged;
	}

	private void OnDisable()
	{
		Bedrock.UnlockContentChanged -= HandleUnlockContentChanged;
	}

	private void HandleUnlockContentChanged(object sender, EventArgs e)
	{
		UpdateGraphics();
	}

	public void UpdateGraphics()
	{
		ElementUserData elementUserData = ElementDataManager.Instance.GetElementUserData(elementType);
		elementUserData.Update();
		UpdateCount(elementUserData.numUnlockedCharacters + elementUserData.numClaimableCharacters);
	}

	private void UpdateCount(int count)
	{
		string text = count.ToString();
		Color color = Color.white;
		if (count == 0)
		{
			text = "-";
			color = Color.gray;
		}
		if (txtCount.Text != text)
		{
			txtCount.Text = text;
		}
		if (txtCount.color != color)
		{
			txtCount.SetColor(color);
		}
	}

	private void OnNavBarBtnClick()
	{
		scrollList.ScrollToItem(scrollItem, 1f);
		SoundEventManager.Instance.Play2D(sfxClick);
		StartCoroutine(OnNavBarBtnClickRte());
		SwrveEventsUI.ElementGroupButtonTouched(Elements.Names[(int)elementType]);
	}

	private IEnumerator OnNavBarBtnClickRte()
	{
		UIManager.instance.blockInput = true;
		CommonAnimations.AnimateButtonRestore(icon.gameObject);
		yield return new WaitForSeconds(0.3f);
		UIManager.instance.blockInput = false;
	}
}
