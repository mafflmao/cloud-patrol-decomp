using UnityEngine;

public class DestroyHeaderCamera : MonoBehaviour
{
	private void Start()
	{
		if (StateManager.Instance.StateCount > 0)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
