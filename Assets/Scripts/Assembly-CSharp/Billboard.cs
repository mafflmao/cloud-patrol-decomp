using UnityEngine;

public class Billboard : MonoBehaviour
{
	private Vector3 initialRotation;

	public bool keepOriginalRotation = true;

	private void Start()
	{
		initialRotation = base.transform.rotation.eulerAngles;
	}

	private void Update()
	{
		base.transform.LookAt(Camera.main.transform);
		if (keepOriginalRotation)
		{
			base.transform.Rotate(initialRotation, Space.Self);
		}
	}
}
