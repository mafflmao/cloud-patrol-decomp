using UnityEngine;

public class VirtualJoystick : MonoBehaviour
{
	private const float xScreenOffset = 5f;

	private const float yScreenOffset = 3f;

	public Transform reticle;

	public Transform projectile;

	public float rechargeTime;

	private float fireTimer;

	private Transform currentGun;

	private float currentRechargeTimer;

	private float limitedY;

	private float limitedX;

	public bool fireContinuously;

	private Vector3 prevPos;

	private void Start()
	{
		currentGun = projectile;
		currentRechargeTimer = rechargeTime;
	}

	private void OnEnable()
	{
		FingerGestures.OnFingerDown += FingerGestures_OnFingerDown;
		FingerGestures.OnFingerUp += FingerGestures_OnFingerUp;
		FingerGestures.OnDragMove += FingerGestures_OnDragMove;
	}

	private void OnDisable()
	{
		FingerGestures.OnFingerDown -= FingerGestures_OnFingerDown;
		FingerGestures.OnFingerUp -= FingerGestures_OnFingerUp;
		FingerGestures.OnDragMove -= FingerGestures_OnDragMove;
	}

	private void Update()
	{
		if (fireContinuously)
		{
			Vector3 position = reticle.transform.position;
			float x = position.x;
			float y = position.y;
			float num = position.x - Camera.main.transform.position.x;
			float y2 = -6.5f * num;
			float x2 = ((!((double)y <= 0.5)) ? (y * 15f) : (y * 5f));
			Quaternion rotation = Quaternion.Euler(x2, y2, 0f);
			if (fireTimer <= Time.time)
			{
				Object.Instantiate(currentGun, new Vector3(x, -2f, 0f), rotation);
				fireTimer = Time.time + currentRechargeTimer;
			}
		}
	}

	private void FingerGestures_OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		base.transform.position = GetWorldPos(fingerPos);
		prevPos = base.transform.position;
	}

	private void FingerGestures_OnFingerUp(int fingerIndex, Vector2 fingerPos, float fingerDownTime)
	{
		if (!fireContinuously)
		{
			Vector3 position = reticle.transform.position;
			float x = position.x;
			float y = position.y;
			float num = position.x - Camera.main.transform.position.x;
			float y2 = -6.5f * num;
			float x2 = ((!((double)y <= 0.5)) ? (y * 15f) : (y * 5f));
			Quaternion rotation = Quaternion.Euler(x2, y2, 0f);
			if (fireTimer <= Time.time)
			{
				Object.Instantiate(currentGun, new Vector3(x, -2f, 0f), rotation);
				fireTimer = Time.time + currentRechargeTimer;
			}
		}
	}

	private void FingerGestures_OnDragMove(Vector2 fingerPos, Vector2 delta)
	{
		prevPos = base.transform.position;
		base.transform.position = GetWorldPos(fingerPos);
		reticle.transform.position += new Vector3((base.transform.position.x - prevPos.x) * 4f, (base.transform.position.y - prevPos.y) * 2f, base.transform.position.z);
		if (reticle.transform.position.y >= Camera.main.transform.position.y + 3f)
		{
			limitedY = 3f;
		}
		else if (reticle.transform.position.y <= Camera.main.transform.position.y - 3f)
		{
			limitedY = -3f;
		}
		else
		{
			limitedY = reticle.position.y;
		}
		if (reticle.transform.position.x >= Camera.main.transform.position.x + 5f)
		{
			limitedX = Camera.main.transform.position.x + 5f;
		}
		else if (reticle.transform.position.x <= Camera.main.transform.position.x - 5f)
		{
			limitedX = Camera.main.transform.position.x - 5f;
		}
		else
		{
			limitedX = reticle.transform.position.x;
		}
		reticle.transform.position = new Vector3(limitedX, limitedY, reticle.position.z);
	}

	public static Vector3 GetWorldPos(Vector2 screenPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(screenPos);
		float distance = (0f - ray.origin.z) / ray.direction.z;
		return ray.GetPoint(distance);
	}

	public void MoveToFingerPosition(Vector2 aFingerPos)
	{
		base.transform.position = GetWorldPos(aFingerPos);
	}
}
