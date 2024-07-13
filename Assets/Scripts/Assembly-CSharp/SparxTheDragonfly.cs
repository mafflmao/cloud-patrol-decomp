using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparxTheDragonfly : Powerup
{
	public float rateOfFire = 0.2f;

	public GameObject lightningBolt;

	public GameObject sparxInGame;

	public GameObject sparxMuzzleFlash;

	public GameObject bombImpactVFX;

	public SoundEventData sfxFireSpark;

	public SoundEventData sfxBombImpact;

	private GameObject _sparxModel;

	private GameObject _lightningInstance;

	private int _targetCount;

	private int _maxTargetCount;

	private List<GameObject> _targets = new List<GameObject>();

	private SparxUpgrade _upgrade;

	private void Start()
	{
		Vector3 localScale = new Vector3(0.5f, 0.5f, 0.5f);
		_sparxModel = null;
		_upgrade = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<SparxUpgrade>();
		if (_upgrade != null)
		{
			_sparxModel = _upgrade.GetSparxOverride();
			_lightningInstance = _upgrade.GetLineRenderer();
			rateOfFire = _upgrade.rateOfFire;
			localScale = _upgrade.scale;
		}
		if (_sparxModel == null)
		{
			_sparxModel = Object.Instantiate(sparxInGame) as GameObject;
			localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}
		if (_lightningInstance == null)
		{
			_lightningInstance = Object.Instantiate(lightningBolt) as GameObject;
		}
		_sparxModel.transform.position = base.Holder.transform.position + new Vector3(0f, 0.25f, 0f);
		_sparxModel.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		_sparxModel.transform.parent = ShipManager.instance.shipVisual.transform;
		_sparxModel.transform.localScale = localScale;
		_lightningInstance.transform.position = base.Holder.transform.position + new Vector3(0f, 0.25f, 0f);
		_lightningInstance.transform.rotation = Quaternion.identity;
		_lightningInstance.transform.parent = ShipManager.instance.shipVisual.transform;
		_lightningInstance.GetComponent<LineRenderer>().enabled = false;
		GameObjectUtils.HideObject(_sparxModel);
	}

	public override void SetLevel(int newLevel, float newValue)
	{
		base.Level = newLevel;
		_maxTargetCount = Mathf.RoundToInt(newValue);
		_maxTargetCount += Mathf.RoundToInt(GetUpgradeModifier());
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
		LevelManager.MovingToNextRoom -= HandleLevelManagerMovingToNextRoom;
	}

	private void HandleLevelManagerMovingToNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		StopCoroutine("FireAtTargets");
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		StopCoroutine("FireAtTargets");
		StartCoroutine("FireAtTargets");
	}

	protected override void HandleTriggered()
	{
		base.HandleTriggered();
		GameObjectUtils.ShowObject(_sparxModel);
		StartCoroutine("FireAtTargets");
		base.Holder.UpdateTime(0f);
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
		LevelManager.MovingToNextRoom += HandleLevelManagerMovingToNextRoom;
	}

	protected override void Update()
	{
		if (base.IsTriggered && !(_lightningInstance == null))
		{
			if (GameManager.Instance.IsPaused)
			{
				_lightningInstance.GetComponent<LineRendererLightning>().Pause();
			}
			else
			{
				_lightningInstance.GetComponent<LineRendererLightning>().Unpause();
			}
		}
	}

	private IEnumerator FireAtTargets()
	{
		while (true)
		{
			_targets.AddRange(GameObject.FindGameObjectsWithTag("Bomb"));
			if (_upgrade != null)
			{
				if (_upgrade.affectProjectiles)
				{
					_targets.AddRange(GameObject.FindGameObjectsWithTag("Projectile"));
				}
				if (_upgrade.affectSpikeShields)
				{
					GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
					GameObject[] array = enemies;
					foreach (GameObject enemy in array)
					{
						if (enemy.GetComponent<HazardBombProxy>() != null)
						{
							_targets.Add(enemy);
						}
					}
				}
			}
			yield return new WaitForSeconds(0.25f);
			foreach (GameObject target in _targets)
			{
				if (!(target == null) && _targetCount < _maxTargetCount)
				{
					if (target.CompareTag("Bomb"))
					{
						StartCoroutine(ShootABomb(target.GetComponent<Hazard>()));
					}
					if (target.CompareTag("Projectile"))
					{
						StartCoroutine(ShootAProjectile(target.GetComponent<Health>()));
					}
					if (target.CompareTag("Enemy"))
					{
						StartCoroutine(ShootAShieldTroll(target.GetComponent<Health>()));
					}
					yield return new WaitForSeconds(rateOfFire);
				}
			}
			_targets.Clear();
		}
	}

	private IEnumerator ShootABomb(Hazard bomb)
	{
		if (!(bomb == null))
		{
			_targetCount++;
			base.Holder.UpdateTime(Mathf.Clamp((float)_targetCount / ((float)_maxTargetCount * 1f), 0f, 0.99f));
			if (_targetCount == _maxTargetCount)
			{
				InvokeHelper.InvokeSafe(CommitSuicide, 0.5f, this);
			}
			_lightningInstance.GetComponent<LineRendererLightning>().target = bomb.gameObject;
			_lightningInstance.GetComponent<LineRenderer>().enabled = true;
			Object.Instantiate(sparxMuzzleFlash, _sparxModel.transform.position + new Vector3(0f, 0.03f, -0.1f), _sparxModel.transform.rotation);
			SoundEventManager.Instance.Play(sfxFireSpark, _sparxModel);
			yield return new WaitForSeconds(0.2f);
			_lightningInstance.GetComponent<LineRenderer>().enabled = false;
			if (bomb != null)
			{
				bomb.DropLoot();
				TrySpawnCoin(bomb.transform.position);
				Object.Instantiate(bombImpactVFX, bomb.transform.position, bomb.transform.rotation);
				SoundEventManager.Instance.Play(sfxBombImpact, bomb.gameObject);
				bomb.DefusedHazard(true);
				Object.Destroy(bomb.gameObject);
			}
		}
	}

	private void TrySpawnCoin(Vector3 position)
	{
		if (_upgrade != null)
		{
			GameObject elementalCoin = _upgrade.GetElementalCoin();
			if (elementalCoin != null)
			{
				elementalCoin.transform.position = position;
				elementalCoin.transform.parent = LevelManager.Instance.currentScreenRoot.transform;
			}
		}
	}

	private IEnumerator ShootAProjectile(Health projectile)
	{
		if (!(projectile == null))
		{
			_targetCount++;
			base.Holder.UpdateTime(Mathf.Clamp((float)_targetCount / ((float)_maxTargetCount * 1f), 0f, 0.99f));
			if (_targetCount == _maxTargetCount)
			{
				InvokeHelper.InvokeSafe(CommitSuicide, 0.5f, this);
			}
			_lightningInstance.GetComponent<LineRendererLightning>().target = projectile.gameObject;
			_lightningInstance.GetComponent<LineRenderer>().enabled = true;
			if (_upgrade == null || _upgrade.overrideMuzzleFlash == null)
			{
				Object.Instantiate(sparxMuzzleFlash, _sparxModel.transform.position + new Vector3(0f, 0.03f, -0.1f), _sparxModel.transform.rotation);
			}
			else
			{
				Object.Instantiate(_upgrade.overrideMuzzleFlash, _sparxModel.transform.position + new Vector3(0f, 0.03f, -0.1f), _sparxModel.transform.rotation);
			}
			SoundEventManager.Instance.Play(sfxFireSpark, _sparxModel);
			yield return new WaitForSeconds(0.1f);
			_lightningInstance.GetComponent<LineRenderer>().enabled = false;
			if (projectile != null)
			{
				projectile.Kill();
				TrySpawnCoin(projectile.transform.position);
				Object.Instantiate(bombImpactVFX, projectile.transform.position, projectile.transform.rotation);
				SoundEventManager.Instance.Play(sfxBombImpact, projectile.gameObject);
			}
		}
	}

	private IEnumerator ShootAShieldTroll(Health health)
	{
		if (!(health == null) && !health.isDead)
		{
			_targetCount++;
			base.Holder.UpdateTime(Mathf.Clamp((float)_targetCount / ((float)_maxTargetCount * 1f), 0f, 0.99f));
			if (_targetCount == _maxTargetCount)
			{
				InvokeHelper.InvokeSafe(CommitSuicide, 0.5f, this);
			}
			_lightningInstance.GetComponent<LineRendererLightning>().target = health.gameObject;
			_lightningInstance.GetComponent<LineRenderer>().enabled = true;
			if (_upgrade == null || _upgrade.overrideMuzzleFlash == null)
			{
				Object.Instantiate(sparxMuzzleFlash, _sparxModel.transform.position + new Vector3(0f, 0.03f, -0.1f), _sparxModel.transform.rotation);
			}
			else
			{
				Object.Instantiate(_upgrade.overrideMuzzleFlash, _sparxModel.transform.position + new Vector3(0f, 0.03f, -0.1f), _sparxModel.transform.rotation);
			}
			SoundEventManager.Instance.Play(sfxFireSpark, _sparxModel);
			yield return new WaitForSeconds(0.1f);
			_lightningInstance.GetComponent<LineRenderer>().enabled = false;
			if (health != null && !health.isDead)
			{
				health.isDeflecting = false;
				health.isForceFielded = false;
				health.Kill();
			}
		}
	}

	public override void DestroyAndFinish(bool waitForCutscene)
	{
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
		LevelManager.MovingToNextRoom -= HandleLevelManagerMovingToNextRoom;
		Object.Destroy(_sparxModel);
		Object.Destroy(_lightningInstance);
		StopAllCoroutines();
		base.DestroyAndFinish(waitForCutscene);
	}

	private void CommitSuicide()
	{
		DestroyAndFinish(true);
	}
}
