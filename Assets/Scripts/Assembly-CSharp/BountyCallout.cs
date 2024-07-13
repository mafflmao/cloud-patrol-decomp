using UnityEngine;

[RequireComponent(typeof(Callout))]
public class BountyCallout : MonoBehaviour
{
	private Callout _callout;

	public SpriteText description;

	public UIButton btnOkay;

	public Bounty bounty;

	public Callout callout
	{
		get
		{
			return _callout;
		}
	}

	private void Awake()
	{
		_callout = GetComponent<Callout>();
	}

	public void Show()
	{
		callout.Show();
		UpdateGraphics();
	}

	public void Hide()
	{
		if (callout.isShowing)
		{
			callout.Hide();
		}
	}

	public void UpdateGraphics()
	{
		if (bounty != null)
		{
			description.Text = bounty.GetFormattedStoreDescription();
		}
	}
}
