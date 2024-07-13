using UnityEngine;

public class DragTrail : MonoBehaviour
{
	public LineRenderer lineRendererPrefab;

	private LineRenderer lineRenderer;

	private void Start()
	{
		lineRenderer = Object.Instantiate(lineRendererPrefab, base.transform.position, base.transform.rotation) as LineRenderer;
		lineRenderer.transform.parent = base.transform;
		lineRenderer.enabled = false;
	}

	private void OnDragBegin()
	{
		lineRenderer.enabled = true;
		lineRenderer.SetPosition(0, base.transform.position);
		lineRenderer.SetPosition(1, base.transform.position);
		lineRenderer.SetWidth(0.01f, base.transform.localScale.x);
	}

	private void OnDragEnd()
	{
		lineRenderer.enabled = false;
	}

	private void Update()
	{
		if (lineRenderer.enabled)
		{
			lineRenderer.SetPosition(1, base.transform.position);
		}
	}
}
