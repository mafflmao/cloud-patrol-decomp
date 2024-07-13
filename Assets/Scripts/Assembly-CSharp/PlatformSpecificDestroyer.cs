using UnityEngine;

public class PlatformSpecificDestroyer : MonoBehaviour
{
	public bool IsForAndroidKindle;

	private void Start()
	{
		if (IsForAndroidKindle)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
