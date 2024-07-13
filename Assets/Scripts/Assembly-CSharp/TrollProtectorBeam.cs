using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class TrollProtectorBeam : MonoBehaviour
{
	private LineRenderer _lr;

	public GameObject target;

	public bool targetColliderCenter;

	public Color lineColor;

	public bool growTowardsTarget;

	public float growSpeed = 0.05f;

	public bool hasArrivedAtTarget;

	private Vector3 endPoint = Vector3.zero;

	private Vector3 targetPosition;

	private void Awake()
	{
		if (_lr == null)
		{
			_lr = GetComponent<LineRenderer>();
		}
	}

	private void Update()
	{
		if (target == null)
		{
			return;
		}
		_lr.SetColors(lineColor, lineColor);
		if (targetColliderCenter)
		{
			if (target.GetComponent<Collider>() != null)
			{
				targetPosition = target.GetComponent<Collider>().bounds.center;
			}
		}
		else
		{
			targetPosition = target.transform.position;
		}
		if (growTowardsTarget && !hasArrivedAtTarget)
		{
			if (endPoint == Vector3.zero)
			{
				endPoint = base.transform.position;
			}
			else
			{
				Vector3 vector = Vector3.MoveTowards(endPoint, targetPosition, growSpeed);
				if ((double)Vector3.SqrMagnitude(vector - targetPosition) > 0.001)
				{
					endPoint = vector;
				}
				else
				{
					hasArrivedAtTarget = true;
				}
			}
		}
		else
		{
			endPoint = targetPosition;
			hasArrivedAtTarget = true;
		}
		_lr.SetWidth(0.35f, 0.75f);
		_lr.SetPosition(0, base.transform.position);
		_lr.SetPosition(1, endPoint);
	}
}
