using UnityEngine;

public class ScreenSizeLimiter : MonoBehaviour
{
	public float baseSize;

	private float _previousScale = 0.1f;

	private void OnEnable()
	{
		base.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
	}

	private void Update()
	{
		Vector3 position = Camera.main.transform.position;
		float num = Vector3.Distance(position, base.transform.position);
		float num2 = num / baseSize;
		if (!Mathf.Approximately(num2, _previousScale))
		{
			_previousScale = num2;
			base.transform.localScale = Vector3.Scale(Vector3.one, new Vector3(num2, num2, num2));
		}
	}
}
