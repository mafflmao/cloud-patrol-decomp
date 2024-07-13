using UnityEngine;

public class iPadTranslateHack : MonoBehaviour
{
	public Vector3 transformOnIPad = Vector3.zero;

	private void Start()
	{
		if (Mathf.Approximately(Camera.main.aspect, 1.3333334f))
		{
			base.transform.position += transformOnIPad;
		}
	}
}
