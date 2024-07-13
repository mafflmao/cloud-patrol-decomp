using UnityEngine;

public class FireProjectile : MonoBehaviour
{
	public Transform projectile;

	public Transform projectile2;

	public float rechargeTime;

	public float rechargeTime2;

	private float fireTimer;

	private Transform currentGun;

	private float currentRechargeTimer;

	public bool is3dMode;

	private float yRotAngle;

	private float xRotAngleHigh;

	private float xRotAngleLow;

	private float yTolerance;

	private void Start()
	{
		base.GetComponent<Renderer>().enabled = false;
		currentGun = projectile;
		currentRechargeTimer = rechargeTime;
		if (!is3dMode)
		{
			yRotAngle = -6.5f;
			xRotAngleHigh = 15f;
			xRotAngleLow = 5f;
			yTolerance = 5f;
		}
		else
		{
			yRotAngle = -6.5f;
			xRotAngleHigh = 10f;
			xRotAngleLow = 6f;
			yTolerance = Camera.main.transform.position.y - 5f;
		}
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		FingerGestures.OnFingerDown += FingerGestures_OnFingerDown;
		FingerGestures.OnFingerUp += FingerGestures_OnFingerUp;
		FingerGestures.OnTap += FingerGestures_OnTap;
		FingerGestures.OnDragMove += FingerGestures_OnDragMove;
	}

	private void OnDisable()
	{
		FingerGestures.OnFingerDown -= FingerGestures_OnFingerDown;
		FingerGestures.OnTap -= FingerGestures_OnTap;
		FingerGestures.OnFingerUp -= FingerGestures_OnFingerUp;
		FingerGestures.OnDragMove -= FingerGestures_OnDragMove;
	}

	private void FingerGestures_OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		base.transform.position = GetWorldPos(fingerPos);
		base.GetComponent<Renderer>().enabled = true;
	}

	private void FingerGestures_OnFingerUp(int fingerIndex, Vector2 fingerPos, float fingerDownTime)
	{
		base.GetComponent<Renderer>().enabled = false;
	}

	private void FingerGestures_OnTap(Vector2 fingerPos)
	{
		Vector3 worldPos = GetWorldPos(fingerPos);
		float x = worldPos.x;
		float y = worldPos.y;
		float num = worldPos.x - Camera.main.transform.position.x;
		float y2 = yRotAngle * num;
		float x2 = ((!(y <= yTolerance)) ? (y * xRotAngleLow) : (y * xRotAngleHigh));
		Quaternion rotation = Quaternion.Euler(x2, y2, 0f);
		if (fireTimer <= Time.time)
		{
			Object.Instantiate(currentGun, new Vector3(x, -2f, Camera.main.transform.position.z), rotation);
			fireTimer = Time.time + currentRechargeTimer;
		}
	}

	private void FingerGestures_OnDragMove(Vector2 fingerPos, Vector2 delta)
	{
		base.transform.position = GetWorldPos(fingerPos);
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

	public void SwapGuns()
	{
		if (currentGun == projectile)
		{
			currentGun = projectile2;
			currentRechargeTimer = rechargeTime2;
		}
		else
		{
			currentGun = projectile;
			currentRechargeTimer = rechargeTime;
		}
	}
}
