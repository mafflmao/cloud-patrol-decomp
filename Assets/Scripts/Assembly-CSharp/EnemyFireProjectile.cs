using System.Collections;
using UnityEngine;

public class EnemyFireProjectile : PeekabooTroll
{
	public Transform projectile;

	public float initWaitTime;

	public int ammoCount = 5;

	public SoundEventData SFX_Telegraph_Shoot_Fire;

	public SoundEventData VO_Taunt;

	public SoundEventData SFX_Attack_Shoot_Fire;

	public GameObject cannonBall;

	public GameObject cannon;

	public GameObject fireFX;

	private GameObject _projectileTarget;

	private Transform _projectileSpawnPoint;

	private bool _trackingTarget = true;

	private BoxCollider _trollCollider;

	protected override void Start()
	{
		base.Start();
		_projectileTarget = GameObject.FindGameObjectWithTag("ProjectileTarget");
		if (_projectileTarget == null)
		{
			Debug.LogError("Unable to find projectile target");
		}
		_trackingTarget = true;
		_projectileSpawnPoint = TransformUtil.FindRecursive(cannon.transform, "Bone06");
		_animationStates.Lobber_Idle();
		_trollCollider = GetComponent<BoxCollider>();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		_trackingTarget = false;
	}

	public override void Disable()
	{
		base.Disable();
		_trackingTarget = false;
		InvokeHelper.InvokeSafe(DetachAccess, 0.2f, this);
	}

	public override void StartTrollBehaviour()
	{
		StartCoroutine(FiringCoroutine());
	}

	private void Update()
	{
		if (_trackingTarget && _projectileTarget != null)
		{
			float y = 57.29578f * Mathf.Atan((0f - _projectileTarget.transform.position.x + base.transform.position.x) / (0f - _projectileTarget.transform.position.z + base.transform.position.z));
			base.transform.rotation = Quaternion.Euler(new Vector3(0f, y, 0f));
		}
	}

	private IEnumerator FiringCoroutine()
	{
		yield return new WaitForSeconds(initWaitTime);
		_animationStates.Lobber_Idle();
		bool firstTime = true;
		while (true)
		{
			if (peekABoo)
			{
				if (!firstTime)
				{
					yield return new WaitForSeconds(2.5f);
				}
				yield return StartCoroutine(PeekabooMoveUp());
			}
			if (ammoCount > 0)
			{
				StartCoroutine(_animationStates.Ready());
				ammoCount--;
				_animationStates.Lobber_Ready();
				SoundEventManager.Instance.Play(VO_Taunt, base.gameObject);
				SoundEventManager.Instance.Play(SFX_Telegraph_Shoot_Fire, base.gameObject, 0.4f);
				InvokeHelper.InvokeSafe(ShiftCollision, 0.25f, this);
				InvokeHelper.InvokeSafe(HideCannonball, 0.5f, this);
				yield return new WaitForSeconds(0.8f);
				_animationStates.Lobber_Shoot();
				if (cannon != null)
				{
					cannon.GetComponent<Animation>().Play("shoot");
				}
				InvokeHelper.InvokeSafe(InstantiateProjectile, 0.35f, this);
				yield return new WaitForSeconds(0.8f);
				_animationStates.Lobber_Reload();
				InvokeHelper.InvokeSafe(ShiftCollision, 0.4f, this);
				if (cannonBall != null)
				{
					cannonBall.GetComponent<Renderer>().enabled = true;
				}
				yield return new WaitForSeconds(_animationStates.CurrentClipLength);
			}
			else if (!peekABoo)
			{
				break;
			}
			_animationStates.Lobber_Idle();
			yield return new WaitForSeconds(_animationStates.CurrentClipLength);
			if (peekABoo)
			{
				yield return new WaitForSeconds(2.5f);
				yield return StartCoroutine(PeekabooMoveDown());
			}
			else
			{
				yield return new WaitForSeconds(2f);
			}
			firstTime = false;
		}
	}

	private void HideCannonball()
	{
		cannonBall.GetComponent<Renderer>().enabled = false;
	}

	private void InstantiateProjectile()
	{
		SoundEventManager.Instance.Play(SFX_Attack_Shoot_Fire, base.gameObject);
		if (fireFX != null)
		{
			Object.Instantiate(fireFX, _projectileSpawnPoint.position, base.transform.rotation);
		}
		Transform transform = (Transform)Object.Instantiate(projectile, _projectileSpawnPoint.position, base.transform.rotation);
		transform.parent = base.transform.parent;
		if ((bool)transform.GetComponent<ArcProjectile>())
		{
			transform.GetComponent<ArcProjectile>().SetShooter(base.gameObject.name);
		}
	}

	private void DetachAccess()
	{
		Accessory[] components = GetComponents<Accessory>();
		Accessory[] array = components;
		foreach (Accessory accessory in array)
		{
			accessory.Detach();
		}
	}

	private void ShiftCollision()
	{
		if (_trollCollider.center.x == 0f)
		{
			_trollCollider.center = new Vector3(0.3f, _trollCollider.center.y, _trollCollider.center.z);
		}
		else
		{
			_trollCollider.center = new Vector3(0f, _trollCollider.center.y, _trollCollider.center.z);
		}
	}

	protected override IEnumerator VictoryDanceCoroutine()
	{
		_animationStates.Lobber_Idle();
		yield break;
	}
}
