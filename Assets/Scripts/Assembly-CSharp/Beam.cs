using UnityEngine;

public class Beam : MonoBehaviour
{
	public Color startColor = Color.blue;

	public Color endColor = Color.red;

	public float startWidth = 0.25f;

	public float endWidth = 0.25f;

	private LineRenderer lineRenderer;

	private void Start()
	{
		lineRenderer = base.gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		lineRenderer.SetColors(startColor, endColor);
		lineRenderer.SetWidth(startWidth, endWidth);
		Target(new Vector3(0f, 0f, 20f));
		Enable();
	}

	private void Update()
	{
	}

	private void Stop()
	{
	}

	public void Enable()
	{
		lineRenderer.enabled = true;
	}

	public void Disable()
	{
		lineRenderer.enabled = false;
	}

	public void Target(Vector3 target)
	{
		lineRenderer.SetPosition(1, target);
	}

	public void SetEndWidth(float thickness)
	{
		endWidth = thickness;
		lineRenderer.SetWidth(startWidth, endWidth);
	}

	public void SetStartWidth(float thickness)
	{
		startWidth = thickness;
		lineRenderer.SetWidth(startWidth, endWidth);
	}
}
