using System.Collections;
using UnityEngine;

public class EnemyStationaryShooter : PeekabooTroll
{
	private const string firingCoroutine = "FiringCoroutine";

	public float initWaitTime = 2f;

	public float waitTime;

	public float chargeupTime = 2f;

	public int ammoCount = 5;

	public Flipbook fireFX;

	public bool fireThruMessage;

	public bool hasJetpack;

	public bool isRapeller;

	public Transform projectile;

	public SoundEventData SFX_Telegraph_Shoot_Fire;

	public SoundEventData VO_Taunt;

	public SoundEventData SFX_Attack_Shoot_Fire;

	public SoundEventData SFX_Copter;

	public SoundEventData VO_Laugh;

	public Renderer meshRenderer;

	private Transform _projectileSpawnPoint;

	private GameObject _projectileTarget;

	private bool _trackingTarget = true;

	private bool _isRed;

	private bool _waitingUntilVisibleToShoot;

	protected override void Start()
	{
		base.Start();
		if (_animationStates != null)
		{
			if (hasJetpack)
			{
				_animationStates.Jetpack_Idle_Init();
				_animationStates.Offset(Random.Range(0f, 0.5f));
			}
			else
			{
				_animationStates.Gunner_Idle_Init();
				_animationStates.Offset(Random.Range(0f, 0.5f));
			}
			if (GetComponent<TripleShooterModifier>() != null || DifficultyManager.ShouldSpawnRedTrolls)
			{
				TurnRed();
			}
		}
		_projectileSpawnPoint = TransformUtil.FindRecursive(base.transform, "GoblinCharge2Shoot_Quad");
		if (waitTime == 0f)
		{
			waitTime = initWaitTime;
		}
		chargeupTime = DifficultyManager.Instance.ShooterChargeTime;
		fireFX.fps = Mathf.RoundToInt((float)fireFX.fps * (2f / chargeupTime));
		waitTime = DifficultyManager.Instance.ShooterWaitTime;
		_projectileTarget = GameObject.FindGameObjectWithTag("ProjectileTarget");
		_trackingTarget = true;
	}

	protected void TurnRed()
	{
		_isRed = true;
		if (GetComponent<TripleShooterModifier>() == null)
		{
			base.gameObject.AddComponent<TripleShooterModifier>();
		}
		Transform transform = TransformUtil.FindRecursive(_animationStates.anim.transform, "TrollWarrior");
		if (transform != null)
		{
			transform.GetComponent<Renderer>().material.color = TripleShooterModifier.TrollColor;
		}
		Health component = GetComponent<Health>();
		if (component != null)
		{
			component.scoreType = ScoreData.ScoreType.ENEMY_SHOOTER_RED;
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		_trackingTarget = false;
	}

	public override void StartTrollBehaviour()
	{
		StopCoroutine("FiringCoroutine");
		if (!fireThruMessage)
		{
			StartCoroutine("FiringCoroutine");
		}
	}

	private void Update()
	{
		if (_trackingTarget && _projectileTarget != null)
		{
			Vector3 position = _projectileTarget.transform.position;
			float y = 57.29578f * Mathf.Atan((0f - position.x + base.transform.position.x) / (0f - position.z + base.transform.position.z));
			base.transform.rotation = Quaternion.Euler(new Vector3(0f, y, 0f));
		}
	}

	private IEnumerator FiringCoroutine()
	{
		waitTime = DifficultyManager.Instance.ShooterWaitTime;
		Debug.Log("Shooter Init Wait Time: " + initWaitTime + " / Wait Time: " + waitTime);
		if (hasJetpack && !isRapeller)
		{
			InvokeHelper.InvokeSafe(PlayCopterSFX, initWaitTime / 3f, this);
		}
		yield return new WaitForSeconds(TripleShooterModifier.GetTime(initWaitTime, _isRed));
		if (hasJetpack)
		{
			_animationStates.Jetpack_Idle();
		}
		else
		{
			_animationStates.Gunner_Idle();
		}
		bool firstTime = true;
		while (true)
		{
			if (!firstTime)
			{
				yield return new WaitForSeconds(TripleShooterModifier.GetTime(waitTime, _isRed));
			}
			if (peekABoo)
			{
				yield return StartCoroutine(PeekabooMoveUp());
			}
			yield return WaitForGame();
			if (ammoCount > 0 && !_waitingUntilVisibleToShoot)
			{
				yield return StartCoroutine(FireProjectileCoroutine());
			}
			else if (!peekABoo)
			{
				break;
			}
			if (peekABoo)
			{
				yield return new WaitForSeconds(2.5f);
				yield return StartCoroutine(PeekabooMoveDown());
			}
			firstTime = false;
		}
	}

	public void FireProjectileNow()
	{
		StartCoroutine(FireProjectileCoroutine());
	}

	private IEnumerator FireProjectileCoroutine()
	{
		Vector2 screenCoordinates = Camera.main.WorldToScreenPoint(base.transform.position);
		while (screenCoordinates.x < 0f || screenCoordinates.y < 0f || screenCoordinates.x > (float)Screen.width || screenCoordinates.y > (float)Screen.height)
		{
			screenCoordinates = Camera.main.WorldToScreenPoint(base.transform.position);
			_waitingUntilVisibleToShoot = true;
			yield return new WaitForSeconds(initWaitTime / 4f);
		}
		_waitingUntilVisibleToShoot = false;
		SoundEventManager.Instance.Play(VO_Taunt, base.gameObject);
		if (ammoCount > 0)
		{
			ammoCount--;
			SoundEventManager.Instance.Play(SFX_Telegraph_Shoot_Fire, base.gameObject);
			if (hasJetpack)
			{
				_animationStates.StartCoroutine(_animationStates.JetpackGunner_Ready());
			}
			else
			{
				_animationStates.StartCoroutine(_animationStates.Gunner_Ready());
			}
			if (fireFX != null)
			{
				InvokeHelper.InvokeSafe(time: TripleShooterModifier.GetTime(0.3f, _isRed), action: fireFX.Play, behaviour: fireFX);
			}
			yield return new WaitForSeconds(TripleShooterModifier.GetTime(chargeupTime, _isRed));
			if (hasJetpack)
			{
				_animationStates.JetpackGunner_Shoot();
			}
			else
			{
				_animationStates.Gunner_Shoot();
			}
			if (_isRed)
			{
				SpawnRedProjectile();
				InvokeHelper.InvokeSafe(SpawnRedProjectile, 0.1f, this);
				InvokeHelper.InvokeSafe(SpawnRedProjectile, 0.2f, this);
			}
			else
			{
				SpawnNormalProjectile();
			}
			yield return new WaitForSeconds(TripleShooterModifier.GetTime(0.6f, _isRed));
			if (hasJetpack)
			{
				_animationStates.Jetpack_Idle();
			}
			else
			{
				_animationStates.Gunner_Idle();
			}
		}
	}

	private Transform SpawnNormalProjectile()
	{
		if (_projectileSpawnPoint == null)
		{
			return null;
		}
		SoundEventManager.Instance.Play(SFX_Attack_Shoot_Fire, base.gameObject);
		GameObject currentScreenRoot = LevelManager.Instance.currentScreenRoot;
		ScreenManager componentInChildren = currentScreenRoot.GetComponentInChildren<ScreenManager>();
		if (componentInChildren == null)
		{
			return null;
		}
		Transform transform = (Transform)Object.Instantiate(projectile, _projectileSpawnPoint.position, base.transform.rotation);
		if (currentScreenRoot != null)
		{
			transform.parent = componentInChildren.transform;
		}
		transform.GetComponent<ArcProjectile>().SetShooter(base.gameObject.name);
		return transform.transform;
	}

	private void SpawnRedProjectile()
	{
		Transform transform = SpawnNormalProjectile();
		if (transform != null)
		{
			transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			transform.GetComponent<Health>().scoreType = ScoreData.ScoreType.PROJECTILE_CORKSCREW;
			transform.GetComponent<Renderer>().material.color = Color.red;
		}
	}

	public override void Disable()
	{
		base.Disable();
		_trackingTarget = false;
		_animationStates.StopAllCoroutines();
		if (fireFX != null && fireFX.gameObject != null)
		{
			Object.Destroy(fireFX.gameObject);
		}
		if (hasJetpack)
		{
			_animationStates.Jetpack_Idle();
		}
		else
		{
			_animationStates.Gunner_Idle();
		}
	}

	private void PlayCopterSFX()
	{
		SoundEventManager.Instance.Play(SFX_Copter, base.gameObject);
	}

	private void OnDestroy()
	{
		if (!SafeMonoBehaviour.IsShuttingDown && SoundEventManager.Instance != null && hasJetpack)
		{
			SoundEventManager.Instance.Stop(SFX_Copter, base.gameObject);
		}
	}

	protected override IEnumerator VictoryDanceCoroutine()
	{
		if (fireFX != null && fireFX.gameObject != null)
		{
			Object.Destroy(fireFX.gameObject);
		}
		if (hasJetpack)
		{
			_animationStates.Jetpack_Idle();
			yield break;
		}
		int rand = Random.Range(1, 4);
		_animationStates.StopAllCoroutines();
		if (rand <= 2)
		{
			yield return new WaitForSeconds(Random.Range(0f, 0.5f));
			SoundEventManager.Instance.Play(VO_Laugh, base.gameObject);
			switch (rand)
			{
			case 1:
				_animationStates.Victory();
				yield return new WaitForSeconds(_animationStates.anim.clip.length);
				_animationStates.Idle();
				break;
			case 2:
				_animationStates.Gunner_Victory();
				yield return new WaitForSeconds(_animationStates.anim.clip.length);
				_animationStates.Gunner_Idle();
				break;
			}
		}
		else
		{
			_animationStates.Gunner_Idle();
		}
	}
}
