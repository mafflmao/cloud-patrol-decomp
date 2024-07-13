using System;
using UnityEngine;

[RequireComponent(typeof(UIButton3D))]
[RequireComponent(typeof(BoxCollider))]
public class UIButtonComposite : Scale3Grid
{
	public BoxCollider boxCollider;

	public static readonly Color DisabledColor = new Color(0.25f, 0.25f, 0.25f);

	public static readonly Color EnabledColor = new Color(0.5f, 0.5f, 0.5f);

	public SoundEventData clickedSound;

	private MonoBehaviour _originalScriptWithMethodToInvoke;

	private string _originalMethodToInvoke;

	private bool _isHandlingClick;

	private UIButton3D _uiButton3d;

	public UIButton3D UIButton3D
	{
		get
		{
			if (_uiButton3d == null)
			{
				_uiButton3d = GetComponent<UIButton3D>();
			}
			return _uiButton3d;
		}
	}

	public bool IsButtonColliderEnabled
	{
		get
		{
			return base.GetComponent<Collider>().enabled;
		}
		set
		{
			base.GetComponent<Collider>().enabled = value;
		}
	}

	public static event EventHandler<CancellableEventArgs> ButtonClicking;

	protected override void Start()
	{
		base.Start();
		_originalScriptWithMethodToInvoke = UIButton3D.scriptWithMethodToInvoke;
		_originalMethodToInvoke = UIButton3D.methodToInvoke;
		UIButton3D.scriptWithMethodToInvoke = this;
		UIButton3D.methodToInvoke = "HandleClicked";
	}

	protected override void SetupReferences()
	{
		base.SetupReferences();
		if (boxCollider == null)
		{
			boxCollider = GetComponent<BoxCollider>();
		}
	}

	protected override bool IsStateOkForRebuild()
	{
		if (boxCollider == null)
		{
			Debug.LogError("Could not rebuild the UIButtonComposite " + base.gameObject.name + " a required component is null");
			return false;
		}
		return base.IsStateOkForRebuild();
	}

	protected override void RebuildInternal()
	{
		base.RebuildInternal();
		boxCollider.size = new Vector3(size.x, size.y, 0f);
	}

	public override void Hide(bool isHidden)
	{
		base.Hide(isHidden);
		if (UIButton3D.spriteText != null)
		{
			UIButton3D.spriteText.Hide(isHidden);
		}
	}

	public override void SetColor(Color color)
	{
		base.SetColor(color);
		if (UIButton3D.spriteText != null)
		{
			UIButton3D.spriteText.SetColor(color);
		}
	}

	private void HandleClicked()
	{
		if (_isHandlingClick)
		{
			Debug.LogError("Caught double-activation of button!");
			return;
		}
		_isHandlingClick = true;
		CancellableEventArgs cancellableEventArgs = new CancellableEventArgs();
		OnButtonClicking(cancellableEventArgs);
		if (!cancellableEventArgs.IsCancelled)
		{
			if (clickedSound != null)
			{
				SoundEventManager.Instance.Play2D(clickedSound);
			}
			Pulse();
			if (_originalScriptWithMethodToInvoke != null)
			{
				_originalScriptWithMethodToInvoke.SendMessage(_originalMethodToInvoke);
			}
		}
		_isHandlingClick = false;
	}

	private void OnButtonClicking(CancellableEventArgs args)
	{
		if (UIButtonComposite.ButtonClicking != null)
		{
			UIButtonComposite.ButtonClicking(this, args);
		}
	}
}
