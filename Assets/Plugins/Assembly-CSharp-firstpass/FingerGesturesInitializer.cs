using UnityEngine;

public class FingerGesturesInitializer : MonoBehaviour
{
	public FingerGestures editorGestures;

	public FingerGestures desktopGestures;

	public FingerGestures iosGestures;

	public FingerGestures androidGestures;

	public bool makePersistent = true;

	private void Awake()
	{
	}
}
