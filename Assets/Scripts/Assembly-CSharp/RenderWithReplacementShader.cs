using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class RenderWithReplacementShader : MonoBehaviour
{
	public Shader replacementShader;

	public Color color;

	private void OnPreCull()
	{
		if (base.enabled)
		{
			base.GetComponent<Camera>().SetReplacementShader(replacementShader, "RenderType");
			Shader.SetGlobalColor("_Color", color);
		}
	}

	private void OnDestroy()
	{
		base.GetComponent<Camera>().ResetReplacementShader();
	}
}
