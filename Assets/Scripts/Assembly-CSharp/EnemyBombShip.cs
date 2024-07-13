using System.Collections;
using UnityEngine;

public class EnemyBombShip : TrollBase
{
	private const float acceleration = 4f;

	private const float drag = 0.5f;

	private const float maxVelocity = 2f;

	private const float maxDistance = 2f;

	private Vector3 currentVelocity = Vector3.zero;

	private Vector3 targetLoc = Vector3.zero;

	private bool _chaseFinger;

	private bool _flyTowards = true;

	public bool IsChasingFinger
	{
		get
		{
			return _chaseFinger;
		}
		set
		{
			_chaseFinger = value;
		}
	}

	private void Start()
	{
		Vector3 position = base.transform.position;
		position.z = ShipManager.instance.dragMultiTarget[0].transform.position.z;
		base.transform.position = position;
	}

	public override void StartTrollBehaviour()
	{
		_chaseFinger = false;
	}

	private void Update()
	{
		if (_chaseFinger && !(ShipManager.instance.dragMultiTarget[0] == null))
		{
			targetLoc = ShipManager.instance.dragMultiTarget[0].transform.position;
			if (_flyTowards || !ShipManager.instance.dragMultiTarget[0].isSelecting)
			{
				AccelerateTowards();
			}
			else
			{
				AccelerateAway();
			}
			Vector3 position = base.transform.position;
			Vector3 vector = currentVelocity * Time.deltaTime;
			Vector3 vector2 = position + vector - targetLoc;
			float num = 0.5f;
			if (vector2.sqrMagnitude < num * num)
			{
				vector *= 0.5f;
			}
			base.transform.position = position + vector;
		}
	}

	private void AccelerateTowards()
	{
		Vector3 vector = base.transform.position - targetLoc;
		currentVelocity.x = AccelerateValue(currentVelocity.x, vector.x);
		currentVelocity.y = AccelerateValue(currentVelocity.y, vector.y);
	}

	private void AccelerateAway()
	{
		Vector3 vector = base.transform.position + (base.transform.position - targetLoc);
		currentVelocity.x = AccelerateValue(currentVelocity.x, vector.x);
		currentVelocity.y = AccelerateValue(currentVelocity.y, vector.y);
	}

	private float AccelerateValue(float velocity, float delta)
	{
		float num = 0f;
		velocity = ((!(delta > 0f)) ? (velocity + 4f * Time.deltaTime) : (velocity - 4f * Time.deltaTime));
		return Mathf.Clamp(velocity, -2f, 2f);
	}

	private void Deccelerate()
	{
		currentVelocity.x = DeccelerateValue(currentVelocity.x);
		currentVelocity.y = DeccelerateValue(currentVelocity.y);
	}

	private float DeccelerateValue(float velocity)
	{
		if (velocity == 0f || velocity * velocity < 0.25f)
		{
			return 0f;
		}
		if (velocity > 0f)
		{
			return velocity - 0.5f * Time.deltaTime;
		}
		if (velocity < 0f)
		{
			return velocity + 0.5f * Time.deltaTime;
		}
		return 0f;
	}

	protected override IEnumerator VictoryDanceCoroutine()
	{
		yield break;
	}
}
