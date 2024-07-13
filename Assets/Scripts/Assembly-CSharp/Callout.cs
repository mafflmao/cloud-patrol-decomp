using System.Collections;
using UnityEngine;

public class Callout : MonoBehaviour
{
	public delegate void CalloutBtnClickHandler(bool confirm);

	public bool hideAtStart = true;

	[HideInInspector]
	public bool isShowing;

	public BoxCollider boxCollider;

	public UIButton3D cancelButton;

	public event CalloutBtnClickHandler calloutBtnClickEvt;

	private void Awake()
	{
		isShowing = !hideAtStart;
	}

	private IEnumerator Start()
	{
		((BoxCollider)cancelButton.GetComponent<Collider>()).size = GUISystem.Instance.guiCamera.sizeMinAspect;
		boxCollider.enabled = !hideAtStart;
		yield return null;
		base.gameObject.SetActive(!hideAtStart);
	}

	public void Show()
	{
		hideAtStart = false;
		Debug.Log("Showing callout");
		isShowing = true;
		base.gameObject.SetActive(true);
		boxCollider.enabled = true;
	}

	public void Hide()
	{
		Debug.Log("Hiding callout");
		isShowing = false;
		base.gameObject.SetActive(false);
		boxCollider.enabled = false;
	}

	private void OnDestroy()
	{
		this.calloutBtnClickEvt = null;
	}
}
