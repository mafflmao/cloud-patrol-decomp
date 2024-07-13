using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TargetLine : MonoBehaviour
{
	private TargetQueue targetQueue;

	private LineRenderer lineRenderer;

	private DragMultiTarget dragMultiTarget;

	public GameObject spotPrefab;

	private List<GameObject> spotList;

	private List<Vector3> vertList;

	private Shooter shooter;

	private int lastTargetCount;

	public Material baseMaterial;

	private float lineZDepth = 3f;

	private void Start()
	{
		vertList = new List<Vector3>();
		if (targetQueue == null)
		{
			targetQueue = GetComponent<TargetQueue>();
		}
		shooter = GetComponent<Shooter>();
		lineRenderer = GetComponent<LineRenderer>();
		spotList = new List<GameObject>();
		dragMultiTarget = ShipManager.instance.dragMultiTarget[ShipManager.instance.shooter.FindIndex((Shooter x) => x == shooter)];
		for (int i = 0; i < 6; i++)
		{
			GameObject gameObject = Object.Instantiate(spotPrefab) as GameObject;
			gameObject.transform.parent = base.transform;
			gameObject.gameObject.SetActive(false);
			spotList.Add(gameObject);
		}
		lastTargetCount = 0;
		baseMaterial = lineRenderer.material;
	}

	public void RevertMaterialToBase()
	{
		lineRenderer.material = baseMaterial;
	}

	public void SetMaterial(Material mat)
	{
		lineRenderer.material = mat;
	}

	private void Update()
	{
		int count = targetQueue.Count;
		if (count > 0 && !shooter.isShooting)
		{
			vertList.Clear();
			Vector3 to = Vector3.zero;
			for (int i = 0; i < count; i++)
			{
				GameObject targetGameObject = targetQueue.GetTargetGameObject(i);
				if (!(targetGameObject == null))
				{
					Vector3 position = Camera.main.WorldToScreenPoint(targetGameObject.GetComponent<Collider>().bounds.center);
					position.z = lineZDepth;
					Vector3 vector = Camera.main.ScreenToWorldPoint(position);
					if (i == 0)
					{
						vertList.Add(vector);
						to = vector;
					}
					else
					{
						vertList.Add(Vector3.Lerp(vector, to, 0.02f));
						vertList.Add(vector);
					}
					spotList[i].gameObject.SetActive(true);
					spotList[i].transform.position = vector;
					to = vector;
				}
			}
			if (count < GameManager.gunSlotCount)
			{
				Vector3 position2 = Camera.main.WorldToScreenPoint(dragMultiTarget.transform.position);
				position2.z = lineZDepth;
				Vector3 item = Camera.main.ScreenToWorldPoint(position2);
				vertList.Add(item);
			}
			lineRenderer.SetVertexCount(vertList.Count);
			for (int j = 0; j < vertList.Count; j++)
			{
				lineRenderer.SetPosition(j, vertList[j]);
			}
		}
		else
		{
			lineRenderer.SetVertexCount(0);
			foreach (GameObject spot in spotList)
			{
				spot.gameObject.SetActive(false);
			}
		}
		if (lastTargetCount == count)
		{
			return;
		}
		if (count == GameManager.gunSlotCount)
		{
			lineRenderer.material.SetColor("_TintColor", new Color(1f, 1f, 0f, 0.6f));
			foreach (GameObject spot2 in spotList)
			{
				spot2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 0f, 0.6f));
			}
		}
		else
		{
			lineRenderer.material.SetColor("_TintColor", new Color(1f, 0f, 1f, 0.6f));
			foreach (GameObject spot3 in spotList)
			{
				spot3.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 0f, 1f, 0.6f));
			}
		}
		lastTargetCount = count;
	}
}
