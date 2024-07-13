using UnityEngine;

[ExecuteInEditMode]
public class StickToMainCameraInEditor : MonoBehaviour
{
	private void LateUpdate()
	{
		if (base.transform.position != Camera.main.transform.position)
		{
			base.transform.position = Camera.main.transform.position;
		}
	}
}
