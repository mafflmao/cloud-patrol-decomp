using UnityEngine;

public class CopyPosition : MonoBehaviour
{
	public GameObject objectToCopyPositionFrom;

	private void LateUpdate()
	{
		if (objectToCopyPositionFrom != null && base.gameObject != null)
		{
			base.gameObject.transform.position = objectToCopyPositionFrom.transform.position;
		}
	}
}
