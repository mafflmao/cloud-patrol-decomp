using UnityEngine;

public class StartMenu : MonoBehaviour
{
	public GUIStyle titleStyle;

	public GUIStyle buttonStyle;

	public float buttonHeight = 80f;

	private Rect screenRect = new Rect(0f, 0f, SampleUI.VirtualScreenWidth, SampleUI.VirtualScreenHeight);

	public float menuWidth = 450f;

	private void Start()
	{
	}

	private void OnGUI()
	{
		SampleUI.ApplyVirtualScreen();
		GUILayout.BeginArea(screenRect);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical(GUILayout.Width(menuWidth / 2f));
		GUILayout.Space(15f);
		GUILayout.Label("API Samples", titleStyle);
		GUILayout.Space(25f);
		SampleSceneButton("Finger Down, Stationary, Up", "FingerEventsPart1");
		SampleSceneButton("Finger Move", "FingerEventsPart2");
		SampleSceneButton("Tap, LongPress, Swipe, Drag", "OneFingerGestures");
		SampleSceneButton("Pinch, Rotation", "PinchRotation");
		GUILayout.EndVertical();
		GUILayout.Space(20f);
		GUILayout.BeginVertical(GUILayout.Width(menuWidth / 2f));
		GUILayout.Space(15f);
		GUILayout.Label("Toolbox Samples", titleStyle);
		GUILayout.Space(25f);
		SampleSceneButton("TBFingerDown, TBFingerUp", "Toolbox-FingerEvents");
		SampleSceneButton("TBTap, TBLongPress, TBSwipe, TBDrag", "Toolbox-OneFingerGestures");
		SampleSceneButton("Advanced Drag & Drop", "Toolbox-DragDrop");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void SampleSceneButton(string text, string scene)
	{
		if (GUILayout.Button(text, GUILayout.Height(buttonHeight)))
		{
			Application.LoadLevel(scene);
		}
		GUILayout.Space(5f);
	}
}
