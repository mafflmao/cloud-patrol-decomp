using UnityEngine;

public class NagTimeLock : MonoBehaviour
{
	public UIButton nagBtn;

	public static bool nagIsShowing;

	public void Start()
	{
		nagBtn.gameObject.transform.localScale = Vector3.one;
		nagBtn.Hide(true);
		nagIsShowing = false;
	}
}
