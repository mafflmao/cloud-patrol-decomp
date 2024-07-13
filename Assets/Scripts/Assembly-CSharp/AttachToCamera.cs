using UnityEngine;

public class AttachToCamera : MonoBehaviour
{
	public Vector3 cameraOffset;

	public Camera cameraToAttachTo;

	private void Start()
	{
		if (cameraToAttachTo == null)
		{
			if (Camera.main == null)
			{
				Debug.LogError("There is no Camera.main. Please tag at least one camera as the main camera.");
			}
			else
			{
				cameraToAttachTo = Camera.main;
			}
		}
		cameraOffset = cameraToAttachTo.transform.position - base.transform.position;
	}

	private void LateUpdate()
	{
		base.transform.position = cameraToAttachTo.transform.position - cameraOffset;
	}
}
