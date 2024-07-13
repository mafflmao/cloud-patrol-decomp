using System.Collections;
using UnityEngine;

public class Nag : MonoBehaviour
{
	public UIButton nagBtn;

	public static bool nagIsShowing;

	public string nagText;

	public SoundEventData ShowSound;

	public SoundEventData TimeoutSound;

	public SoundEventData ClickedSound;

	public bool localizeText;

	public void Start()
	{
		nagBtn.gameObject.transform.localScale = Vector3.one;
		nagBtn.Hide(true);
		nagIsShowing = false;
	}

	private void OnEnable()
	{
		StateManager.StateDeactivated += HandleStateDeactivated;
	}

	private void OnDisable()
	{
		StateManager.StateDeactivated -= HandleStateDeactivated;
	}

	private void HandleStateDeactivated(object sender, StateEventArgs e)
	{
		HideInternal(false);
	}

	public void Show(float delay)
	{
		InvokeHelper.InvokeSafe(Show, delay, this);
	}

	public void Show()
	{
		Debug.Log("Trying to show " + base.gameObject.name);
		nagBtn.Text = ((!localizeText) ? nagText : LocalizationManager.Instance.GetString(nagText));
		if (!nagIsShowing)
		{
			StopAllCoroutines();
			StartCoroutine(ShowCoroutine());
		}
	}

	private IEnumerator ShowCoroutine()
	{
		if (ShowSound != null)
		{
			SoundEventManager.Instance.Play2D(ShowSound);
		}
		nagBtn.controlIsEnabled = true;
		nagBtn.gameObject.transform.localScale = Vector3.zero;
		nagIsShowing = true;
		nagBtn.Hide(false);
		nagBtn.GetComponent<Animation>().Play("NagShow");
		yield return new WaitForSeconds(5f);
		HideInternal(false);
	}

	private void HideInternal(bool wasClicked)
	{
		nagBtn.controlIsEnabled = false;
		StopAllCoroutines();
		if (nagIsShowing)
		{
			StartCoroutine(HideCoroutine());
			if (wasClicked && ClickedSound != null)
			{
				SoundEventManager.Instance.Play2D(ClickedSound);
			}
			else if (!wasClicked && TimeoutSound != null)
			{
				SoundEventManager.Instance.Play2D(TimeoutSound);
			}
		}
	}

	public void Hide()
	{
		HideInternal(true);
	}

	private IEnumerator HideCoroutine()
	{
		nagBtn.GetComponent<Animation>().Play("NagHide");
		yield return new WaitForSeconds(0.5f);
		nagBtn.gameObject.transform.localScale = Vector3.one;
		nagBtn.Hide(true);
		nagIsShowing = false;
	}
}
