using UnityEngine;

public class ScrollListController : MonoBehaviour
{
	public const float AmountToScaleButtonsByWhenSelected = 1.33f;

	public Transform buttonPopoutTransform;

	private UIScrollList _scrollList;

	private bool _isFaded;

	private bool _isButtonRemoved;

	private UIButton _btnRemoved;

	private Transform _btnRemovedPosition;

	public TweenData[] fadeInTweens;

	public TweenData[] fadeOutTweens;

	private void Awake()
	{
		_scrollList = GetComponent<UIScrollList>();
	}

	public void Reset()
	{
		if (_isFaded)
		{
			_scrollList.transform.localScale = Vector3.one;
			if (_isButtonRemoved)
			{
				ReplaceButtonImmediate();
			}
			iTween.FadeTo(_scrollList.gameObject, iTween.Hash("alpha", 1f, "time", 0f));
		}
		EnableButtons();
		_scrollList.ScrollListTo(0f);
	}

	public void FadeOut()
	{
		_isFaded = true;
		DisableButtons();
		_scrollList.CancelDrag();
		iTween.ScaleTo(base.gameObject, iTween.Hash("scale", new Vector3(0.7f, 0.7f, 1f), "time", 0.25f, "easeType", iTween.EaseType.easeOutQuad));
	}

	public void FadeIn()
	{
		FadeIn(false);
	}

	public void FadeIn(bool replaceButton)
	{
		iTween.ScaleTo(base.gameObject, iTween.Hash("scale", Vector3.one, "time", 0.25f, "easeType", iTween.EaseType.easeInQuad, "oncompletetarget", base.gameObject, "oncomplete", "OnScrollListFadeInComplete", "oncompleteparams", replaceButton));
	}

	public void OnScrollListFadeInComplete(bool replaceButton)
	{
		_isFaded = false;
		if (replaceButton)
		{
			ReplaceButtonImmediate();
		}
		EnableButtons();
	}

	public void EnableButtons()
	{
		_scrollList.controlIsEnabled = true;
		BaseItemStoreButton[] componentsInChildren = _scrollList.GetComponentsInChildren<BaseItemStoreButton>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].EnableButton();
		}
	}

	public void DisableButtons()
	{
		_scrollList.controlIsEnabled = false;
		BaseItemStoreButton[] componentsInChildren = _scrollList.GetComponentsInChildren<BaseItemStoreButton>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DisableButton();
		}
	}

	public void RemoveButton(UIButton btn)
	{
		if (!_isButtonRemoved)
		{
			_isButtonRemoved = true;
			_btnRemoved = btn;
			_btnRemovedPosition = _btnRemoved.transform.parent;
			_btnRemoved.transform.parent = buttonPopoutTransform;
			_btnRemoved.controlIsEnabled = false;
			_btnRemoved.SetState(2);
			iTween.MoveTo(_btnRemoved.gameObject, iTween.Hash("position", Vector3.zero, "time", 0.25f, "islocal", true));
			iTween.ScaleTo(_btnRemoved.gameObject, new Vector3(1.33f, 1.33f, 1.33f), 0.25f);
		}
	}

	public void ReplaceButtonAndFadeIn()
	{
		Vector3 localScale = _scrollList.transform.localScale;
		_scrollList.transform.localScale = Vector3.one;
		Vector3 position = _btnRemovedPosition.position;
		_scrollList.transform.localScale = localScale;
		iTween.MoveTo(_btnRemoved.gameObject, iTween.Hash("position", position, "time", 0.25f));
		iTween.ScaleTo(_btnRemoved.gameObject, Vector3.one, 0.25f);
		FadeIn(true);
	}

	public void ReplaceButtonImmediate()
	{
		if (!_isButtonRemoved)
		{
			Debug.Log(Time.realtimeSinceStartup + ": ScrollListController - Replace button immediate. Button is not removed. Returning");
			return;
		}
		_isButtonRemoved = false;
		_btnRemoved.transform.parent = _btnRemovedPosition;
		_btnRemoved.transform.localPosition = Vector3.zero;
	}
}
