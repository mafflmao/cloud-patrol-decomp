using UnityEngine;

public class FadeFromZ : MonoBehaviour
{
	public Transform targetToFade;

	private void Update()
	{
		if ((bool)targetToFade && (bool)targetToFade.GetComponent<Renderer>())
		{
			Color color = targetToFade.GetComponent<Renderer>().material.GetColor("_Color");
			color.a = base.transform.localPosition.z;
			targetToFade.GetComponent<Renderer>().material.color = color;
		}
	}
}
