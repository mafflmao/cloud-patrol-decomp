using UnityEngine;

public class SpinTransform : MonoBehaviour
{
	public float rotationDegreesPerSecond;

	public void LateUpdate()
	{
		base.transform.Rotate(new Vector3(0f, 0f, rotationDegreesPerSecond));
	}
}
