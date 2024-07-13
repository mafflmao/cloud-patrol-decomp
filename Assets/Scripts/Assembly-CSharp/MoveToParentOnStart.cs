using UnityEngine;

public class MoveToParentOnStart : MonoBehaviour
{
	public Transform newParentAtStart;

	private void Awake()
	{
		base.transform.parent = newParentAtStart;
		base.transform.localPosition = Vector3.zero;
	}
}
