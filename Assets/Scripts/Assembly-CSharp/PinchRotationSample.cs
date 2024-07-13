using UnityEngine;

public class PinchRotationSample : SampleBase
{
	public enum InputMode
	{
		PinchOnly = 0,
		RotationOnly = 1,
		PinchAndRotation = 2
	}

	public Transform target;

	public Material rotationMaterial;

	public Material pinchMaterial;

	public Material pinchAndRotationMaterial;

	public float pinchScaleFactor = 0.02f;

	private Material originalMaterial;

	private InputMode inputMode = InputMode.PinchAndRotation;

	private bool rotating;

	private bool pinching;

	public Rect inputModeButtonRect;

	private bool Rotating
	{
		get
		{
			return rotating;
		}
		set
		{
			if (rotating != value)
			{
				rotating = value;
				UpdateTargetMaterial();
			}
		}
	}

	public bool RotationAllowed
	{
		get
		{
			return inputMode == InputMode.RotationOnly || inputMode == InputMode.PinchAndRotation;
		}
	}

	private bool Pinching
	{
		get
		{
			return pinching;
		}
		set
		{
			if (pinching != value)
			{
				pinching = value;
				UpdateTargetMaterial();
			}
		}
	}

	public bool PinchAllowed
	{
		get
		{
			return inputMode == InputMode.PinchOnly || inputMode == InputMode.PinchAndRotation;
		}
	}

	protected override string GetHelpText()
	{
		return "This sample demonstrates how to use the two-fingers Pinch and Rotation gesture events to control the scale and orientation of a rectangle on the screen\r\n\r\n- Pinch: move two fingers closer or further apart to change the scale of the rectangle\r\n- Rotation: twist two fingers in a circular motion to rotate the rectangle\r\n\r\n";
	}

	protected override void Start()
	{
		base.Start();
		base.UI.StatusText = "Use two fingers anywhere on the screen to rotate and scale the green object.";
		originalMaterial = target.GetComponent<Renderer>().sharedMaterial;
	}

	private void OnEnable()
	{
		FingerGestures.OnRotationBegin += FingerGestures_OnRotationBegin;
		FingerGestures.OnRotationMove += FingerGestures_OnRotationMove;
		FingerGestures.OnRotationEnd += FingerGestures_OnRotationEnd;
		FingerGestures.OnPinchBegin += FingerGestures_OnPinchBegin;
		FingerGestures.OnPinchMove += FingerGestures_OnPinchMove;
		FingerGestures.OnPinchEnd += FingerGestures_OnPinchEnd;
	}

	private void OnDisable()
	{
		FingerGestures.OnRotationBegin -= FingerGestures_OnRotationBegin;
		FingerGestures.OnRotationMove -= FingerGestures_OnRotationMove;
		FingerGestures.OnRotationEnd -= FingerGestures_OnRotationEnd;
		FingerGestures.OnPinchBegin -= FingerGestures_OnPinchBegin;
		FingerGestures.OnPinchMove -= FingerGestures_OnPinchMove;
		FingerGestures.OnPinchEnd -= FingerGestures_OnPinchEnd;
	}

	private void FingerGestures_OnRotationBegin(Vector2 fingerPos1, Vector2 fingerPos2)
	{
		if (RotationAllowed)
		{
			base.UI.StatusText = "Rotation gesture started.";
			Rotating = true;
		}
	}

	private void FingerGestures_OnRotationMove(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta)
	{
		if (Rotating)
		{
			base.UI.StatusText = "Rotation updated by " + rotationAngleDelta + " degrees";
			target.Rotate(0f, 0f, rotationAngleDelta);
		}
	}

	private void FingerGestures_OnRotationEnd(Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle)
	{
		if (Rotating)
		{
			base.UI.StatusText = "Rotation gesture ended. Total rotation: " + totalRotationAngle;
			Rotating = false;
		}
	}

	private void FingerGestures_OnPinchBegin(Vector2 fingerPos1, Vector2 fingerPos2)
	{
		if (PinchAllowed)
		{
			Pinching = true;
		}
	}

	private void FingerGestures_OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
	{
		if (Pinching)
		{
			target.transform.localScale += delta * pinchScaleFactor * Vector3.one;
		}
	}

	private void FingerGestures_OnPinchEnd(Vector2 fingerPos1, Vector2 fingerPos2)
	{
		if (Pinching)
		{
			Pinching = false;
		}
	}

	private void UpdateTargetMaterial()
	{
		Material sharedMaterial = ((pinching && rotating) ? pinchAndRotationMaterial : (pinching ? pinchMaterial : ((!rotating) ? originalMaterial : rotationMaterial)));
		target.GetComponent<Renderer>().sharedMaterial = sharedMaterial;
	}

	private void OnGUI()
	{
		SampleUI.ApplyVirtualScreen();
		string text;
		InputMode inputMode;
		switch (this.inputMode)
		{
		case InputMode.PinchOnly:
			text = "Pinch Only";
			inputMode = InputMode.RotationOnly;
			break;
		case InputMode.RotationOnly:
			text = "Rotation Only";
			inputMode = InputMode.PinchAndRotation;
			break;
		default:
			text = "Pinch + Rotation";
			inputMode = InputMode.PinchOnly;
			break;
		}
		if (GUI.Button(inputModeButtonRect, text))
		{
			this.inputMode = inputMode;
		}
	}
}
