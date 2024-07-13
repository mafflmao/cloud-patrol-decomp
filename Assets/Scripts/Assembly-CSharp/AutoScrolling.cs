using UnityEngine;

public class AutoScrolling : MonoBehaviour
{
	public float moveSpeed = 2f;

	private void LateUpdate()
	{
		base.transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
	}
}
