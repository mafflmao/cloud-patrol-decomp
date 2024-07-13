using UnityEngine;

public class TurretCapsule : MonoBehaviour
{
	public Texture2D normalTexture;

	public Texture2D maxTexture;

	public MeshRenderer meshRenderer;

	public void Show()
	{
		meshRenderer.enabled = true;
	}

	public void Hide()
	{
		meshRenderer.enabled = false;
	}

	public void MaxOut()
	{
		meshRenderer.material.SetTexture("_MainTex", maxTexture);
	}

	public void UnMaxOut()
	{
		meshRenderer.material.SetTexture("_MainTex", normalTexture);
	}
}
