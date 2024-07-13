using UnityEngine;

public class RestoreMaterial : MonoBehaviour
{
	public Material mat;

	private void Awake()
	{
		if (mat == null)
		{
			mat = base.gameObject.GetComponent<Renderer>().sharedMaterial;
		}
	}

	private void RestoreMaterialTweenComplete()
	{
		Debug.Log(string.Concat("Setting material: ", mat, " to game object ", base.gameObject.name, " with material: ", base.gameObject.GetComponent<Renderer>().sharedMaterial));
		base.gameObject.GetComponent<Renderer>().sharedMaterial = mat;
		base.gameObject.GetComponent<Renderer>().material = mat;
		Object.Destroy(this);
	}
}
