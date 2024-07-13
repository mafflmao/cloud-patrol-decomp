using System;
using UnityEngine;

public class ArcProjectile : SafeMonoBehaviour
{
	public const float corkscrewRadius = 0.35f;

	public Transform target;

	public Vector3 startOffset = new Vector3(0f, 0f, 0f);

	public float heightToDistanceRatio = 0.1f;

	public Vector3 rotationSpeed;

	private float _rotationSpeedScale;

	private float _timeCoefficient = 1f;

	public bool corkscrewOn;

	public float corkscrewSpeed = 3f;

	private float angle;

	public SoundEventData tellSFX;

	private float arcHeight;

	private float timeToTarget;

	private Vector3 startPoint;

	private Vector3 midPoint;

	private Vector3 endPoint;

	private Vector3 currentPosition;

	private float _distance;

	private float halfTime;

	private float lengthLerp;

	private int travelState;

	private float currTime;

	private Health myHealth;

	private bool hitShip;

	private bool tellHasBeenPlayed;

	private Vector3 startScale;

	private float nearScale = 1.5f;

	private float startTime;

	private GameObject m_Exploder;

	public GameObject poof;

	public SoundEventData cannonHit;

	private string _shooter = "None";

	private float _tmpScale;

	private bool _spawnedWind;

	public GameObject explosionFX;

	public bool HasHitShip
	{
		get
		{
			return hitShip;
		}
	}

	private void Start()
	{
		m_Exploder = null;
		if (target == null)
		{
			target = ShipManager.instance.enemyProjectileTarget;
		}
		startPoint = new Vector3(base.transform.position.x + startOffset.x, base.transform.position.y + startOffset.y, base.transform.position.z + startOffset.z);
		startScale = base.transform.localScale;
		float from = target.position.x - 0.5f;
		float to = target.position.x + 0.5f;
		float x = Mathf.Lerp(from, to, UnityEngine.Random.value);
		endPoint = new Vector3(x, target.position.y, target.position.z);
		midPoint = (startPoint + endPoint) / 2f;
		_distance = Vector3.Distance(startPoint, endPoint);
		arcHeight = _distance * heightToDistanceRatio;
		SetTime();
		startTime = Time.time;
		Vector3 vector = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
		vector.Normalize();
		rotationSpeed.x *= vector.x;
		rotationSpeed.y *= vector.y;
		rotationSpeed.z *= vector.z;
		_rotationSpeedScale = 3f / timeToTarget;
		travelState = 0;
	}

	public void SetShooter(string shooter)
	{
		_shooter = shooter;
	}

	public void SetTime()
	{
		timeToTarget = GetTimeToTarget();
		halfTime = timeToTarget * 0.5f;
	}

	private float GetTimeToTarget()
	{
		if (corkscrewOn)
		{
			return DifficultyManager.Instance.CorkscrewProjectileTime * _timeCoefficient;
		}
		if (arcHeight > 0f)
		{
			return DifficultyManager.Instance.LobberProjectileTime * _timeCoefficient;
		}
		return DifficultyManager.Instance.ProjectileTime * _timeCoefficient;
	}

	public void SetSpeedCoefficient(float timeCoefficient)
	{
		_timeCoefficient = timeCoefficient;
		SetTime();
		currTime = Time.time + halfTime * (1f - lengthLerp);
	}

	private Vector3 GetXYZ(float _radius, float _theta)
	{
		Vector3 result = new Vector3(0f, 0f, 0f);
		result.x = _radius * Mathf.Sin(_theta * ((float)Math.PI / 180f));
		result.y = _radius * Mathf.Cos(_theta * ((float)Math.PI / 180f));
		return result;
	}

	private void LateUpdate()
	{
		if (GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver || hitShip)
		{
			return;
		}
		_tmpScale = nearScale * Mathf.Clamp01((Time.time - startTime) / timeToTarget);
		base.transform.localScale = startScale + new Vector3(_tmpScale, _tmpScale, _tmpScale);
		angle += corkscrewSpeed / _timeCoefficient * 60f * Time.deltaTime;
		if (angle >= 360f)
		{
			angle -= 360f;
		}
		else if (angle < 0f)
		{
			angle = 360f - Mathf.Abs(angle % 360f);
		}
		switch (travelState)
		{
		case 0:
			currTime = Time.time + halfTime;
			travelState = 1;
			break;
		case 1:
			if (Time.time <= currTime)
			{
				lengthLerp = LerpTime(currTime, halfTime);
				lengthLerp = Mathf.Clamp(lengthLerp, 0f, 1f);
				currentPosition = Vector3.Lerp(startPoint, midPoint, lengthLerp);
				currentPosition.y += Mathf.SmoothStep(0f, arcHeight, lengthLerp);
				if (corkscrewOn)
				{
					base.transform.position = currentPosition + GetXYZ(Mathf.Lerp(0f, 0.35f, lengthLerp), angle);
				}
				else
				{
					base.transform.position = currentPosition;
				}
				base.transform.Rotate(rotationSpeed * _rotationSpeedScale * 60f * Time.deltaTime, Space.Self);
			}
			else
			{
				SoundEventManager.Instance.Play(tellSFX, base.gameObject);
				travelState = 2;
				currTime = Time.time + halfTime;
			}
			break;
		case 2:
			if (Time.time <= currTime)
			{
				lengthLerp = Mathf.Clamp01(LerpTime(currTime, halfTime));
				currentPosition = Vector3.Lerp(midPoint, endPoint, lengthLerp);
				currentPosition.y += Mathf.SmoothStep(arcHeight, 0f, lengthLerp);
				if (corkscrewOn)
				{
					base.transform.position = currentPosition + GetXYZ(Mathf.Lerp(0.35f, 0f, lengthLerp), angle);
				}
				else
				{
					base.transform.position = currentPosition;
				}
				base.transform.Rotate(rotationSpeed * _rotationSpeedScale * 60f * Time.deltaTime, Space.Self);
				if (lengthLerp >= 0.6f && SkyIronShield.ActiveShield != null)
				{
					SkyIronShield.ActiveShield.SpawnShield(base.transform.position, false);
					travelState = 4;
					GetComponent<Health>().Kill();
				}
				if (lengthLerp >= 0.5f && !_spawnedWind)
				{
					ProjectileSpeedUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<ProjectileSpeedUpgrade>();
					if (passiveUpgradeOrDefault != null)
					{
						_timeCoefficient /= passiveUpgradeOrDefault.projectileSpeedMultiplier;
						halfTime = GetTimeToTarget() * 0.5f;
						currTime = Time.time + halfTime * (1f - lengthLerp);
						passiveUpgradeOrDefault.SpawnWindOnProjectile(base.transform);
					}
					_spawnedWind = true;
				}
			}
			else
			{
				travelState = 3;
			}
			break;
		case 3:
			if (SkyIronShield.ActiveShield == null && GameManager.sessionStats.deathAI == "None")
			{
				GameManager.sessionStats.deathAI = _shooter;
				GameManager.sessionStats.deathType = "Projectile";
				GameManager.sessionStats.deathScreenLocation = Camera.main.WorldToScreenPoint(base.transform.position);
			}
			if ((bool)cannonHit)
			{
				SoundEventManager.Instance.Play(cannonHit, base.gameObject);
			}
			Explode();
			break;
		}
	}

	public void Explode()
	{
		if (!hitShip)
		{
			if (!ShipManager.instance.isShooting)
			{
				m_Exploder = UnityEngine.Object.Instantiate(explosionFX, base.transform.position, Quaternion.identity) as GameObject;
				m_Exploder.GetComponent<Hazard>().originatingGameObject = _shooter;
			}
			hitShip = true;
			base.transform.parent = ShipManager.instance.shipVisual.transform;
			ShipManager.instance.shipVisual.ReceiveProjectile();
			InvokeHelper.InvokeSafe(Disable, 1.1f, this);
		}
	}

	private void Disable()
	{
		base.GetComponent<Renderer>().enabled = false;
		InvokeHelper.InvokeSafe(Kill, 1f, this);
	}

	private void Kill()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Cleanup()
	{
		if ((bool)poof)
		{
			UnityEngine.Object.Instantiate(poof, base.transform.position, Quaternion.identity);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		if (m_Exploder != null)
		{
			UnityEngine.Object.Destroy(m_Exploder);
		}
		if (!SafeMonoBehaviour.IsShuttingDown && SoundEventManager.Instance != null)
		{
			SoundEventManager.Instance.Stop(tellSFX, base.gameObject);
		}
	}

	private float LerpTime(float current, float total)
	{
		return (total - (current - Time.time)) / total;
	}
}
