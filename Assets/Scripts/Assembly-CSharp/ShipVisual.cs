using System;
using System.Collections;
using UnityEngine;

public class ShipVisual : MonoBehaviour
{
	private const string animJump = "SkylanderPartialShow";

	private static readonly ILogger _logger = LogBuilder.Instance.GetLogger(typeof(ShipVisual), LogLevel.Log);

	public static bool fixPosition;

	public Transform lookAtTarget;

	public GameObject staticSkylanderParent;

	public SoundEventData victoryJumpSFX;

	public SoundEventData victoryLandSFX;

	public GameObject activeCharacterLight;

	public GameObject turretGeometry;

	public float fixPositionIncrement = 0.001f;

	public float fixRotationIncrement = 0.01f;

	public float bobMagnitude = 0.03f;

	public GameObject introCameraGO;

	private bool trackingActive;

	private Vector3 myOriginalLocalPosition;

	private Quaternion myOriginalLocalRotation;

	private float yaw;

	private bool alive = true;

	private Transform turretGunBarrel;

	private Transform turretGunBarrelAlt;

	private Transform turretBase;

	private Transform mustacheBoneLeft;

	private Transform mustacheBoneRight;

	private Vector3 mustacheBoneLeftRotation;

	private Vector3 mustacheBoneRightRotation;

	private Vector3 turretGunBarrelOriginalPosition;

	private Vector3 turretGunBarrelAltOriginalPosition;

	private Transform lastLookatTarget;

	private Vector3 lastLookatPoint;

	private Vector3 calculatedLookatPoint;

	private Transform currentLookatTarget;

	private Vector3 currentLookatPoint;

	private float aimTransitionDuration;

	private float changeTargetStartTime;

	private GameObject _staticSkylander;

	private GameObject _riggedSkylander;

	private Vector3 myBobbingPosition;

	private bool _isGiant;

	private AimModifier _aimMod;

	private readonly Quaternion _turretBaseRotationOffset = Quaternion.Euler(270f, 0f, 0f);

	private readonly Quaternion _skylanderRotationOffset = Quaternion.Euler(0f, 180f, 0f);

	public GameObject StaticSkylander
	{
		get
		{
			return _staticSkylander;
		}
	}

	public static event EventHandler<EventArgs> SkylanderSpawnComplete;

	private void Start()
	{
		_isGiant = StartGameSettings.Instance.activeSkylander.isGiant;
		GameObjectUtils.ManageLighting(activeCharacterLight, "HUD");
		GameObject riggedModelPrefab = StartGameSettings.Instance.activeSkylander.GetRiggedModelPrefab();
		if (riggedModelPrefab != null)
		{
			if (!_isGiant)
			{
				_riggedSkylander = UnityEngine.Object.Instantiate(riggedModelPrefab) as GameObject;
				_riggedSkylander.transform.parent = staticSkylanderParent.transform;
				GameObjectUtils.HideObject(_riggedSkylander);
			}
		}
		else
		{
			_logger.LogError("No prefab found for skylander!");
		}
		StartGameSettings.Instance.activeSkylander.LoadFXOverlayPrefabs();
		if (_isGiant)
		{
			GameObjectUtils.HideObject(turretGeometry);
			_staticSkylander = UnityEngine.Object.Instantiate(StartGameSettings.Instance.activeSkylander.GetStaticModelPrefab(), turretGeometry.transform.position, turretGeometry.transform.rotation) as GameObject;
			_staticSkylander.transform.parent = base.transform;
			_staticSkylander.transform.localRotation = Quaternion.identity;
			_aimMod = _staticSkylander.GetComponent<AimModifier>();
			if (_aimMod != null)
			{
				_staticSkylander.transform.localPosition += _aimMod.basePositionOffset;
				StopVFX();
			}
			else
			{
				_logger.LogError("Aim mod is missing on Giant '{0}'", StartGameSettings.Instance.activeSkylander.charName);
			}
			turretBase = _staticSkylander.transform;
			turretGunBarrel = TransformUtil.FindRecursive(_staticSkylander.transform, "TrackingBone");
			turretGunBarrelAlt = TransformUtil.FindRecursive(_staticSkylander.transform, "TrackingBoneAlt");
			if (turretGunBarrel == null)
			{
				_logger.LogError("Gun Barrel bone doesn't exist in Giant");
			}
			else
			{
				turretGunBarrelOriginalPosition = turretGunBarrel.localPosition;
				if (turretGunBarrelAlt != null)
				{
					turretGunBarrelAltOriginalPosition = turretGunBarrelAlt.localPosition;
				}
			}
			mustacheBoneLeft = TransformUtil.FindRecursive(_staticSkylander.transform, "Bone L Mustache02");
			mustacheBoneRight = TransformUtil.FindRecursive(_staticSkylander.transform, "Bone R Mustache02");
			if (mustacheBoneLeft != null && mustacheBoneRight != null)
			{
				mustacheBoneLeftRotation = mustacheBoneLeft.localRotation.eulerAngles;
				mustacheBoneRightRotation = mustacheBoneRight.localRotation.eulerAngles;
			}
		}
		else
		{
			turretBase = TransformUtil.FindRecursive(base.transform, "TurretBall");
			turretGunBarrel = TransformUtil.FindRecursive(base.transform, "TurretBarrel");
			turretGunBarrelOriginalPosition = turretGunBarrel.localPosition;
		}
		trackingActive = false;
		myOriginalLocalPosition = base.transform.localPosition;
		myBobbingPosition = myOriginalLocalPosition;
		myOriginalLocalRotation = base.transform.localRotation;
		ChangeLookatTarget(lookAtTarget, 0f);
		StartCoroutine(SpawnInSequence());
	}

	private void OnEnable()
	{
		BonusManager.BonusRoomVictoryAnimation += OnGiantVictory;
		WackAManager.BossRoomVictoryAnimation += OnGiantVictory;
	}

	private void OnDisable()
	{
		BonusManager.BonusRoomVictoryAnimation -= OnGiantVictory;
		WackAManager.BossRoomVictoryAnimation -= OnGiantVictory;
	}

	private void OnGiantVictory(object sender, EventArgs args)
	{
		if (_isGiant)
		{
			StartCoroutine(VictorySequence());
		}
	}

	private void Update()
	{
		if (trackingActive && alive && UpdateLookatTarget())
		{
			AimTurret();
		}
	}

	public void ChangeLookatTarget(Transform newTarget, float transitionDuration)
	{
		lastLookatTarget = currentLookatTarget;
		currentLookatTarget = newTarget;
		aimTransitionDuration = transitionDuration;
		changeTargetStartTime = Time.time;
	}

	public void ResetLookatTarget(Vector3 positionOfLastTarget, float transitionDuration)
	{
		lookAtTarget.transform.position = positionOfLastTarget;
		ChangeLookatTarget(lookAtTarget, transitionDuration);
	}

	public bool UpdateLookatTarget()
	{
		if (lastLookatTarget != null)
		{
			lastLookatPoint = lastLookatTarget.position;
		}
		if (currentLookatTarget != null)
		{
			currentLookatPoint = currentLookatTarget.position;
		}
		Vector3 vector = calculatedLookatPoint;
		if (Time.time - changeTargetStartTime > aimTransitionDuration)
		{
			calculatedLookatPoint = currentLookatPoint;
		}
		else
		{
			float t = ((aimTransitionDuration != 0f) ? ((Time.time - changeTargetStartTime) / aimTransitionDuration) : 1f);
			calculatedLookatPoint = Vector3.Lerp(lastLookatPoint, currentLookatPoint, t);
		}
		return vector != currentLookatPoint;
	}

	public void AimTurret()
	{
		yaw = 57.29578f * Mathf.Atan((0f - calculatedLookatPoint.x + turretBase.position.x) / (0f - calculatedLookatPoint.z + turretBase.position.z));
		Quaternion quaternion = Quaternion.AngleAxis(yaw, Vector3.up);
		turretBase.rotation = ((!_isGiant) ? (quaternion * _turretBaseRotationOffset) : (quaternion * _skylanderRotationOffset));
		staticSkylanderParent.transform.rotation = quaternion * _skylanderRotationOffset;
		Vector3 eulerAngles = Vector3.zero;
		Vector3 eulerAngles2 = Vector3.zero;
		AimModifier component = turretBase.GetComponent<AimModifier>();
		if (component != null)
		{
			eulerAngles = component.rotationOffset;
			eulerAngles2 = component.altRotationOffset;
		}
		if (turretGunBarrel != null)
		{
			turretGunBarrel.LookAt(calculatedLookatPoint, Vector3.up);
			turretGunBarrel.Rotate(eulerAngles);
		}
		if (turretGunBarrelAlt != null)
		{
			turretGunBarrelAlt.LookAt(calculatedLookatPoint, Vector3.up);
			turretGunBarrelAlt.Rotate(eulerAngles2);
		}
		if (mustacheBoneLeft != null && mustacheBoneRight != null)
		{
			float num = (turretGunBarrel.localEulerAngles.x - 300f) / 60f;
			if (num < 0f)
			{
				num = 1f;
			}
			float num2 = 1f - Mathf.Sin(Mathf.Max(0f, num) * (float)Math.PI);
			mustacheBoneLeft.localScale = new Vector3(0.5f + num2 * 0.5f, 1f, 1f);
			mustacheBoneLeft.localRotation = Quaternion.Euler(mustacheBoneLeftRotation.x, mustacheBoneLeftRotation.y, mustacheBoneLeftRotation.z + (1f - num2) * -45f);
			mustacheBoneRight.localScale = new Vector3(0.5f + num2 * 0.5f, 1f, 1f);
			mustacheBoneRight.localRotation = Quaternion.Euler(mustacheBoneRightRotation.x, mustacheBoneRightRotation.y, mustacheBoneRightRotation.z + (1f - num2) * -45f);
		}
	}

	private void AnimateMoveToNextRoomStart(object sender, LevelManager.NextRoomEventArgs args)
	{
		if (!fixPosition)
		{
			switch (args.MoveDirection)
			{
			case LevelManager.MoveDirections.Right:
				iTween.PunchRotation(base.gameObject, new Vector3(0f, 0f, -5f), 6f);
				iTween.PunchPosition(base.gameObject, new Vector3(-0.08f, -0.04f, 0f), 4f);
				break;
			case LevelManager.MoveDirections.Up:
				iTween.PunchPosition(base.gameObject, new Vector3(0f, -0.08f, 0f), 4f);
				break;
			case LevelManager.MoveDirections.Down:
				iTween.PunchPosition(base.gameObject, new Vector3(0f, 0.07f, 0f), 4f);
				break;
			}
		}
	}

	private void AnimateMoveToNextRoomStop(object sender, LevelManager.NextRoomEventArgs args)
	{
		if (!fixPosition)
		{
			switch (args.MoveDirection)
			{
			case LevelManager.MoveDirections.Right:
				iTween.PunchRotation(base.gameObject, new Vector3(0f, 0f, 5f), 4f);
				iTween.PunchPosition(base.gameObject, new Vector3(0.08f, 0.04f, 0f), 2f);
				break;
			case LevelManager.MoveDirections.Up:
				iTween.PunchPosition(base.gameObject, new Vector3(0f, 0.08f, 0f), 2f);
				break;
			case LevelManager.MoveDirections.Down:
				iTween.PunchPosition(base.gameObject, new Vector3(0f, -0.07f, 0f), 2f);
				break;
			}
		}
	}

	private void LateUpdate()
	{
		if (alive)
		{
			if (!fixPosition)
			{
				myBobbingPosition = myOriginalLocalPosition + new Vector3(0f, bobMagnitude * Mathf.Sin(Time.time), 0f);
			}
			else
			{
				myBobbingPosition = myOriginalLocalPosition;
			}
			base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, myBobbingPosition, fixPositionIncrement);
			base.transform.localRotation = Quaternion.RotateTowards(base.transform.localRotation, myOriginalLocalRotation, fixRotationIncrement);
		}
	}

	public void Recoil()
	{
		if (!HealthBar.Instance.IsDead)
		{
			iTween.PunchRotation(base.gameObject, new Vector3(-10f, 0f, 0f), 1f);
		}
	}

	public void BarrelRecoil()
	{
		if (HealthBar.Instance.IsDead)
		{
			return;
		}
		if (_isGiant)
		{
			if (_aimMod.isAlt && turretGunBarrelAlt != null)
			{
				turretGunBarrelAlt.Translate(_aimMod.altRecoilTranslation);
				InvokeHelper.InvokeSafe(BarrelRecoilReturnAlt, 0.02f, this);
			}
			else if (turretGunBarrel != null)
			{
				turretGunBarrel.Translate(_aimMod.recoilTranslation);
				InvokeHelper.InvokeSafe(BarrelRecoilReturn, 0.02f, this);
			}
		}
		else
		{
			turretGunBarrel.Translate(new Vector3(0f, 0f, -0.08f));
			InvokeHelper.InvokeSafe(BarrelRecoilReturn, 0.02f, this);
		}
	}

	private void BarrelRecoilReturn()
	{
		iTween.MoveTo(turretGunBarrel.gameObject, iTween.Hash("position", turretGunBarrelOriginalPosition, "time", 0.5f, "islocal", true));
	}

	private void BarrelRecoilReturnAlt()
	{
		iTween.MoveTo(turretGunBarrelAlt.gameObject, iTween.Hash("position", turretGunBarrelAltOriginalPosition, "time", 0.5f, "islocal", true));
	}

	public void ReceiveProjectile()
	{
		iTween.PunchPosition(base.gameObject, new Vector3(0f, 0f, -0.3f), 3f);
		iTween.PunchRotation(base.gameObject, new Vector3(-20f, 0f, 0f), 3f);
	}

	public void ReceiveHit()
	{
		iTween.PunchRotation(base.gameObject, new Vector3(-60f, 0f, 0f), 3f);
		iTween.MoveBy(base.gameObject, iTween.Hash("y", -0.6f, "easetype", iTween.EaseType.easeInBack, "time", 0.75f, "delay", 0.3f));
		alive = false;
	}

	public void Revive()
	{
		iTween.MoveBy(base.gameObject, iTween.Hash("y", 0.6f, "easetype", iTween.EaseType.easeOutBack, "time", 0.75f, "oncomplete", "ReviveComplete", "oncompletetarget", base.gameObject));
	}

	private void ReviveComplete()
	{
		alive = true;
	}

	public void HideTurret()
	{
		ParentSkylanderToTurret();
		iTween.MoveBy(turretGeometry, iTween.Hash("z", -0.8f, "easetype", iTween.EaseType.easeInBack, "time", 0.75f, "ignoretimescale", true));
		alive = false;
	}

	public void ShowTurret()
	{
		iTween.MoveBy(turretGeometry, iTween.Hash("z", 0.8f, "easetype", iTween.EaseType.easeOutBack, "time", 0.75f, "oncomplete", "ShowTurretComplete", "oncompletetarget", base.gameObject, "ignoretimescale", true));
	}

	private void ShowTurretComplete()
	{
		ParentSkylanderToShip();
		alive = true;
	}

	public IEnumerator SpawnInSequence()
	{
		if (!_isGiant)
		{
			GameObject prefab = StartGameSettings.Instance.activeSkylander.GetStaticModelPrefab();
			_staticSkylander = UnityEngine.Object.Instantiate(prefab) as GameObject;
			for (int i = 0; i < _staticSkylander.transform.childCount; i++)
			{
				_staticSkylander.transform.GetChild(i).gameObject.layer = Layers.LitHud;
			}
			_staticSkylander.layer = Layers.LitHud;
			_staticSkylander.transform.parent = staticSkylanderParent.transform;
			_staticSkylander.transform.localScale = Vector3.one;
			_staticSkylander.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			_staticSkylander.transform.localPosition = Vector3.zero;
			turretGeometry.GetComponent<Renderer>().material.SetTexture("_MainTex", StartGameSettings.Instance.activeSkylander.elementData.LoadTurretTexture());
		}
		trackingActive = true;
		yield return new WaitForSeconds(1f);
		LevelManager.Instance.ActivateFirstRoom();
		OnSkylanderSpawnComplete();
	}

	public void SkylanderJumpIn()
	{
		ShowSkylander();
		AnimationUtils.PlayClip(staticSkylanderParent.GetComponent<Animation>(), "SkylanderPartialShow");
	}

	public void SkylanderJumpOut()
	{
		AnimationClip animationClip = AnimationUtils.PlayClipBackwards(staticSkylanderParent.GetComponent<Animation>(), "SkylanderPartialShow");
		InvokeHelper.InvokeSafe(HideSkylander, animationClip.length, this);
	}

	public void HideSkylander()
	{
		GameObjectUtils.HideObject(_staticSkylander);
	}

	public void ShowSkylander()
	{
		GameObjectUtils.ShowObject(_staticSkylander);
	}

	public void ParentSkylanderToTurret()
	{
		staticSkylanderParent.transform.parent = turretGeometry.transform;
	}

	public void ParentSkylanderToShip()
	{
		staticSkylanderParent.transform.parent = base.transform;
	}

	public IEnumerator VictorySequence()
	{
		_logger.Log("Victory Sequence started!");
		Vector3 partialLocalScale = Vector3.one;
		partialLocalScale = ((!_isGiant) ? _staticSkylander.transform.GetChild(0).localScale : new Vector3(0.5f, 0.5f, 0.5f));
		if (_staticSkylander.GetComponentInChildren<Renderer>().isVisible)
		{
			_logger.Log("Static Skylander is being rendered.");
			AnimationState stateVictory = null;
			AnimationState stateStatic = null;
			if (_isGiant)
			{
				trackingActive = false;
				stateVictory = _staticSkylander.GetComponent<Animation>()["victory"];
				stateStatic = _staticSkylander.GetComponent<Animation>()["static"];
				if (stateStatic == null)
				{
					stateStatic = _staticSkylander.GetComponent<Animation>()["idle"];
				}
				if (stateStatic == null)
				{
					stateStatic = _staticSkylander.GetComponent<Animation>()["Take 001"];
				}
			}
			else
			{
				stateVictory = _riggedSkylander.GetComponent<Animation>()["victory"];
				stateStatic = _riggedSkylander.GetComponent<Animation>()["static"];
				if (stateStatic == null)
				{
					stateStatic = _riggedSkylander.GetComponent<Animation>()["idle"];
				}
				if (stateStatic == null)
				{
					stateStatic = _riggedSkylander.GetComponent<Animation>()["Take 001"];
				}
			}
			Quaternion initialRotation = _staticSkylander.transform.rotation;
			if (stateVictory != null)
			{
				if (_isGiant)
				{
					if (stateStatic != null)
					{
						_staticSkylander.GetComponent<Animation>().Play(stateStatic.clip.name);
					}
					StartVFX();
					Vector3 fixedXZ = new Vector3(0f, Quaternion.LookRotation(Camera.main.transform.position - _staticSkylander.transform.position).eulerAngles.y, 0f);
					fixedXZ += _aimMod.rotationOffsetForVictoryAnimation;
					iTween.RotateTo(_staticSkylander, iTween.Hash("rotation", fixedXZ, "time", _aimMod.timeToRotateForVictoryAnimation));
					_staticSkylander.GetComponent<Animation>().Play(stateVictory.clip.name);
				}
				else
				{
					GameObjectUtils.HideObject(_staticSkylander);
					GameObjectUtils.ShowCharacterWithoutAccessories(_riggedSkylander);
					_riggedSkylander.transform.localScale = partialLocalScale;
					_riggedSkylander.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
					_riggedSkylander.transform.localPosition = Vector3.zero;
					_riggedSkylander.GetComponent<Animation>().Play(stateVictory.clip.name);
				}
				SoundEventData victoryOverrideSfx = StartGameSettings.Instance.activeSkylander.AudioResources.victoryOverrideSfx;
				if (victoryOverrideSfx == null)
				{
					SoundEventManager.Instance.Play2D(victoryJumpSFX);
					yield return new WaitForSeconds(stateVictory.clip.length - 0.25f);
					SoundEventManager.Instance.Play2D(victoryLandSFX);
					yield return new WaitForSeconds(0.25f);
				}
				else
				{
					SoundEventManager.Instance.Play2D(victoryOverrideSfx);
					yield return new WaitForSeconds(stateVictory.clip.length);
				}
				if (stateStatic != null)
				{
					if (_isGiant)
					{
						_staticSkylander.GetComponent<Animation>().Play(stateStatic.clip.name);
						StopVFX();
						iTween.RotateTo(_staticSkylander, iTween.Hash("rotation", initialRotation.eulerAngles, "time", _aimMod.timeToRotateForVictoryAnimation));
					}
					else
					{
						_riggedSkylander.GetComponent<Animation>().Play(stateStatic.clip.name);
						GameObjectUtils.ShowObject(_staticSkylander);
						GameObjectUtils.HideObject(_riggedSkylander);
					}
				}
				else
				{
					_logger.LogError("No static state found");
				}
			}
			else
			{
				_logger.LogError("No victory state found");
			}
		}
		trackingActive = true;
	}

	public IEnumerator SpawnOutSequence()
	{
		yield return new WaitForSeconds(0.5f);
	}

	public void OnSkylanderSpawnComplete()
	{
		if (ShipVisual.SkylanderSpawnComplete != null)
		{
			ShipVisual.SkylanderSpawnComplete(this, new EventArgs());
		}
	}

	private void StartVFX()
	{
		GameObject victoryFXRef = _aimMod.victoryFXRef;
		if (victoryFXRef != null)
		{
			GameObjectUtils.ShowObject(victoryFXRef);
			GameObjectUtils.EmitObject(victoryFXRef);
			if (victoryFXRef.GetComponent<Animation>() != null)
			{
				victoryFXRef.GetComponent<Animation>().Rewind();
				victoryFXRef.GetComponent<Animation>().Play();
			}
		}
	}

	private void StopVFX()
	{
		GameObject victoryFXRef = _aimMod.victoryFXRef;
		if (victoryFXRef != null)
		{
			if (victoryFXRef.GetComponent<Animation>() != null)
			{
				victoryFXRef.GetComponent<Animation>().Stop();
			}
			GameObjectUtils.HideObject(victoryFXRef);
			GameObjectUtils.DontEmitObject(victoryFXRef);
		}
	}
}
