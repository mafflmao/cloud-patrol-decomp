using System.Collections.Generic;
using UnityEngine;

public class FingerEventsSamplePart2 : SampleBase
{
	private class PathRenderer
	{
		private LineRenderer lineRenderer;

		private List<Vector3> points = new List<Vector3>();

		private List<GameObject> markers = new List<GameObject>();

		public PathRenderer(int index, LineRenderer lineRendererPrefab)
		{
			lineRenderer = Object.Instantiate(lineRendererPrefab) as LineRenderer;
			lineRenderer.name = lineRendererPrefab.name + index;
			lineRenderer.enabled = true;
			UpdateLines();
		}

		public void Reset()
		{
			points.Clear();
			UpdateLines();
			foreach (GameObject marker in markers)
			{
				Object.Destroy(marker);
			}
			markers.Clear();
		}

		public void AddPoint(Vector2 screenPos)
		{
			AddPoint(screenPos, null);
		}

		public void AddPoint(Vector2 screenPos, GameObject markerPrefab)
		{
			Vector3 worldPos = SampleBase.GetWorldPos(screenPos);
			if ((bool)markerPrefab)
			{
				AddMarker(worldPos, markerPrefab);
			}
			points.Add(worldPos);
			UpdateLines();
		}

		private GameObject AddMarker(Vector2 pos, GameObject prefab)
		{
			GameObject gameObject = Object.Instantiate(prefab, pos, Quaternion.identity) as GameObject;
			gameObject.name = prefab.name + "(" + markers.Count + ")";
			markers.Add(gameObject);
			return gameObject;
		}

		private void UpdateLines()
		{
			lineRenderer.SetVertexCount(points.Count);
			for (int i = 0; i < points.Count; i++)
			{
				lineRenderer.SetPosition(i, points[i]);
			}
		}
	}

	public LineRenderer lineRendererPrefab;

	public GameObject fingerDownMarkerPrefab;

	public GameObject fingerMoveBeginMarkerPrefab;

	public GameObject fingerMoveEndMarkerPrefab;

	public GameObject fingerUpMarkerPrefab;

	private PathRenderer[] paths;

	protected override void Start()
	{
		base.Start();
		base.UI.StatusText = "Drag your fingers anywhere on the screen";
		paths = new PathRenderer[FingerGestures.Instance.MaxFingers];
		for (int i = 0; i < paths.Length; i++)
		{
			paths[i] = new PathRenderer(i, lineRendererPrefab);
		}
	}

	protected override string GetHelpText()
	{
		return "This sample lets you visualize the FingerDown, FingerMoveBegin, FingerMove, FingerMoveEnd and FingerUp events.\r\n\r\nINSTRUCTIONS:\r\nMove your finger accross the screen and observe what happens.\r\n\r\nLEGEND:\r\n- Red Circle = FingerDown position\r\n- Yellow Square = FingerMoveBegin position\r\n- Green Sphere = FingerMoveEnd position\r\n- Blue Circle = FingerUp position";
	}

	private void OnEnable()
	{
		Debug.Log("Registering finger gesture events from C# script");
		FingerGestures.OnFingerDown += FingerGestures_OnFingerDown;
		FingerGestures.OnFingerMoveBegin += FingerGestures_OnFingerMoveBegin;
		FingerGestures.OnFingerMove += FingerGestures_OnFingerMove;
		FingerGestures.OnFingerMoveEnd += FingerGestures_OnFingerMoveEnd;
		FingerGestures.OnFingerUp += FingerGestures_OnFingerUp;
	}

	private void OnDisable()
	{
		FingerGestures.OnFingerDown -= FingerGestures_OnFingerDown;
		FingerGestures.OnFingerMoveBegin -= FingerGestures_OnFingerMoveBegin;
		FingerGestures.OnFingerMove -= FingerGestures_OnFingerMove;
		FingerGestures.OnFingerMoveEnd -= FingerGestures_OnFingerMoveEnd;
		FingerGestures.OnFingerUp -= FingerGestures_OnFingerUp;
	}

	private void FingerGestures_OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		PathRenderer pathRenderer = paths[fingerIndex];
		pathRenderer.Reset();
		pathRenderer.AddPoint(fingerPos, fingerDownMarkerPrefab);
	}

	private void FingerGestures_OnFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
	{
		PathRenderer pathRenderer = paths[fingerIndex];
		pathRenderer.AddPoint(fingerPos, fingerUpMarkerPrefab);
		base.UI.StatusText = "Finger " + fingerIndex + " was held down for " + timeHeldDown.ToString("N2") + " seconds";
	}

	private void FingerGestures_OnFingerMoveBegin(int fingerIndex, Vector2 fingerPos)
	{
		base.UI.StatusText = "Started moving finger " + fingerIndex;
		PathRenderer pathRenderer = paths[fingerIndex];
		pathRenderer.AddPoint(fingerPos, fingerMoveBeginMarkerPrefab);
	}

	private void FingerGestures_OnFingerMove(int fingerIndex, Vector2 fingerPos)
	{
		PathRenderer pathRenderer = paths[fingerIndex];
		pathRenderer.AddPoint(fingerPos);
	}

	private void FingerGestures_OnFingerMoveEnd(int fingerIndex, Vector2 fingerPos)
	{
		base.UI.StatusText = "Stopped moving finger " + fingerIndex;
		PathRenderer pathRenderer = paths[fingerIndex];
		pathRenderer.AddPoint(fingerPos, fingerMoveEndMarkerPrefab);
	}

	private bool CheckSpawnParticles(Vector2 fingerPos, GameObject requiredObject)
	{
		GameObject gameObject = SampleBase.PickObject(fingerPos);
		if (!gameObject || gameObject != requiredObject)
		{
			return false;
		}
		SpawnParticles(gameObject);
		return true;
	}

	private void SpawnParticles(GameObject obj)
	{
	}
}
