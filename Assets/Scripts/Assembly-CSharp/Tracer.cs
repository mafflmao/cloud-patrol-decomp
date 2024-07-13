using UnityEngine;

public class Tracer : MonoBehaviour
{
	private LineRenderer tracerLine;

	public Vector3 startPoint;

	public Vector3 endPoint;

	public float lifeTime = 0.4f;

	public float travelSpeed = 10f;

	public Color color;

	public float length = 0.01f;

	private float totalTime;

	private GameObject _fxOverlayInstance;

	private Vector3 _fxOverlayStartPoint;

	private Vector3 endPointMoving;

	public void SetFXOverlay(GameObject particleSystemPrefab)
	{
		if (particleSystemPrefab != null)
		{
			color = new Color(0f, 0f, 0f, 0f);
			Quaternion rotation = Quaternion.LookRotation(endPoint - startPoint);
			_fxOverlayStartPoint = startPoint;
			_fxOverlayInstance = Object.Instantiate(particleSystemPrefab, _fxOverlayStartPoint, rotation) as GameObject;
			_fxOverlayInstance.GetComponent<ParticleSystem>().Play(true);
			Object.Destroy(_fxOverlayInstance, lifeTime * 3f);
		}
	}

	private void Start()
	{
		tracerLine = GetComponent<LineRenderer>();
		tracerLine.SetPosition(0, startPoint);
		tracerLine.SetPosition(1, startPoint);
		Color start = new Color(color.r, color.g, color.b, 0f);
		tracerLine.SetColors(start, color);
		Object.Destroy(base.gameObject, lifeTime);
	}

	private void Update()
	{
		totalTime += Time.deltaTime;
		float t = totalTime / lifeTime;
		endPointMoving = Vector3.Lerp(startPoint, endPoint, t);
		tracerLine.SetPosition(1, endPointMoving);
		if (_fxOverlayInstance != null)
		{
			_fxOverlayInstance.transform.position = _fxOverlayStartPoint + (endPointMoving - startPoint);
		}
	}
}
