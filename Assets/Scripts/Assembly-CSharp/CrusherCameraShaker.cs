using UnityEngine;

public class CrusherCameraShaker : MonoBehaviour
{
	private void OnEnable()
	{
		GameManager.CameraShake();
	}
}
