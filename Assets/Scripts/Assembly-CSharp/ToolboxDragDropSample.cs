using UnityEngine;

public class ToolboxDragDropSample : SampleBase
{
	private enum DragPlaneMode
	{
		XY = 0,
		Plane = 1,
		Sphere = 2
	}

	public TBInputManager inputMgr;

	public Transform[] dragObjects;

	public Collider dragSphere;

	public Collider dragPlane;

	public Light pointlight;

	private DragPlaneMode dragPlaneMode;

	private Vector3[] initialPositions;

	public Rect dragModeButtonRect;

	private void SetDragPlaneMode(DragPlaneMode mode)
	{
		switch (mode)
		{
		case DragPlaneMode.XY:
			RestoreInitialPositions();
			dragSphere.gameObject.SetActive(false);
			dragPlane.gameObject.SetActive(false);
			inputMgr.dragPlaneType = TBInputManager.DragPlaneType.XY;
			break;
		case DragPlaneMode.Plane:
			RestoreInitialPositions();
			dragSphere.gameObject.SetActive(false);
			dragPlane.gameObject.SetActive(true);
			inputMgr.dragPlaneCollider = dragPlane;
			inputMgr.dragPlaneType = TBInputManager.DragPlaneType.UseCollider;
			break;
		case DragPlaneMode.Sphere:
			RestoreInitialPositions();
			dragSphere.gameObject.SetActive(true);
			dragPlane.gameObject.SetActive(false);
			inputMgr.dragPlaneCollider = dragSphere;
			inputMgr.dragPlaneType = TBInputManager.DragPlaneType.UseCollider;
			break;
		}
		dragPlaneMode = mode;
	}

	private void SaveInitialPositions()
	{
		initialPositions = new Vector3[dragObjects.Length];
		for (int i = 0; i < initialPositions.Length; i++)
		{
			initialPositions[i] = dragObjects[i].position;
		}
	}

	private void RestoreInitialPositions()
	{
		for (int i = 0; i < initialPositions.Length; i++)
		{
			dragObjects[i].position = initialPositions[i];
		}
	}

	protected override string GetHelpText()
	{
		return "This sample demonstrates the use of the Toolbox's Drag & Drop scripts";
	}

	protected override void Start()
	{
		base.Start();
		SaveInitialPositions();
		SetDragPlaneMode(DragPlaneMode.XY);
	}

	private void OnGUI()
	{
		if (!base.UI.showHelp)
		{
			SampleUI.ApplyVirtualScreen();
			string text;
			DragPlaneMode dragPlaneMode;
			switch (this.dragPlaneMode)
			{
			case DragPlaneMode.Plane:
				text = "Drag On Plane";
				dragPlaneMode = DragPlaneMode.Sphere;
				break;
			case DragPlaneMode.Sphere:
				text = "Drag On Sphere";
				dragPlaneMode = DragPlaneMode.XY;
				break;
			default:
				text = "Drag On XZ";
				dragPlaneMode = DragPlaneMode.Plane;
				break;
			}
			if (GUI.Button(dragModeButtonRect, text))
			{
				SetDragPlaneMode(dragPlaneMode);
			}
		}
	}

	private void ToggleLight()
	{
		pointlight.enabled = !pointlight.enabled;
	}
}
