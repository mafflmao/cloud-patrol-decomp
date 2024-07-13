using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
	public BoxCollider boxCollider;

	public UIButton3D closeButton;

	public PackedSprite background;

	public SpriteText message;

	public List<TweenData> tweenShow;

	public List<TweenData> tweenHide;

	public bool hideAtStart = true;

	public MonoBehaviour scriptWithMethodToInvoke;

	public string methodToInvoke;

	public bool isEnabled;

	public virtual IEnumerator Start()
	{
		if (boxCollider != null)
		{
			boxCollider.enabled = !hideAtStart;
			boxCollider.size = GUISystem.Instance.guiCamera.sizeMinAspect;
		}
		closeButton.scriptWithMethodToInvoke = this;
		closeButton.methodToInvoke = "OnCloseBtnClick";
		yield return null;
		base.gameObject.SetActive(false);
		isEnabled = false;
	}

	private void OnCloseBtnClick()
	{
		Hide();
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		boxCollider.enabled = true;
		isEnabled = true;
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
		boxCollider.enabled = false;
		isEnabled = false;
	}
}
