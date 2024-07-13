using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverWithSpeed : MonoBehaviour
{
	public enum Mode
	{
		noEase = 0,
		easeIn = 1,
		easeOut = 2,
		easeInOut = 3
	}

	public Vector3 destinationPoint;

	public float easeInDistance;

	public float easeOutDistance;

	public int easeInSteps = 7;

	public int easeOutSteps = 7;

	public Mode mode = Mode.easeInOut;

	public int steps = 4;

	private List<Vector3> splinePoints;

	private Vector3[] splinePointArray;

	private List<Vector3> easeInPoints;

	private List<Vector3> easeOutPoints;

	private IEnumerator easeFunction;

	public static event EventHandler MoveComplete;

	private void Start()
	{
		splinePoints = new List<Vector3>();
		if (mode == Mode.noEase)
		{
			steps = 30;
			splinePoints.Add(base.transform.position);
			splinePoints.Add(destinationPoint);
		}
		else
		{
			float num = Vector3.Distance(base.transform.position, destinationPoint);
			while (easeInDistance + easeOutDistance >= num)
			{
				easeOutDistance *= 0.5f;
			}
			if (mode == Mode.easeOut)
			{
				splinePoints.Add(base.transform.position);
			}
			if (mode == Mode.easeIn || mode == Mode.easeInOut)
			{
				Vector3 endPoint = Vector3.MoveTowards(base.transform.position, destinationPoint, easeInDistance);
				easeInPoints = GenerateEasePoints(base.transform.position, endPoint, easeInSteps, true);
				splinePoints.AddRange(easeInPoints);
			}
			if (mode == Mode.easeOut || mode == Mode.easeInOut)
			{
				Vector3 startPoint = Vector3.MoveTowards(destinationPoint, base.transform.position, easeOutDistance);
				easeOutPoints = GenerateEasePoints(startPoint, destinationPoint, easeOutSteps, false);
				if (Mathf.Abs(easeInDistance - easeOutDistance) > easeInDistance / 2f)
				{
					Vector3 item = Vector3.MoveTowards(base.transform.position, destinationPoint, num / 2f);
					splinePoints.Add(item);
				}
				splinePoints.AddRange(easeOutPoints);
			}
			if (mode == Mode.easeIn)
			{
				splinePoints.Add(destinationPoint);
			}
		}
		splinePointArray = splinePoints.ToArray();
		IEnumerable<Vector3> enumerable = Interpolate.NewCatmullRom(splinePointArray, steps, false);
		easeFunction = enumerable.GetEnumerator();
	}

	private void Update()
	{
		if (!GameManager.Instance.IsPaused && GameManager.gameState == GameManager.GameState.Playing)
		{
			if (easeFunction.MoveNext())
			{
				base.transform.position = (Vector3)easeFunction.Current;
			}
			else
			{
				OnMoveComplete();
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (splinePointArray != null)
		{
			iTween.DrawPath(splinePointArray, Color.green);
		}
	}

	private List<Vector3> GenerateEasePoints(Vector3 startPoint, Vector3 endPoint, float steps, bool slowAtStart)
	{
		float num = Vector3.Distance(startPoint, endPoint);
		List<Vector3> list = new List<Vector3>();
		if (slowAtStart)
		{
			list.Add(endPoint);
			for (int i = 0; (float)i < steps; i++)
			{
				num /= 2f;
				list.Add(Vector3.MoveTowards(startPoint, endPoint, num));
			}
			list.Add(startPoint);
			list.Reverse();
		}
		else
		{
			list.Add(startPoint);
			for (int j = 0; (float)j < steps; j++)
			{
				num /= 2f;
				list.Add(Vector3.MoveTowards(endPoint, startPoint, num));
			}
			list.Add(endPoint);
		}
		return list;
	}

	private void OnMoveComplete()
	{
		OnMoveCompleteHack(this);
		UnityEngine.Object.Destroy(this);
	}

	public static void OnMoveCompleteHack(UnityEngine.Object source)
	{
		if (MoverWithSpeed.MoveComplete != null)
		{
			MoverWithSpeed.MoveComplete(source, new EventArgs());
		}
	}
}
