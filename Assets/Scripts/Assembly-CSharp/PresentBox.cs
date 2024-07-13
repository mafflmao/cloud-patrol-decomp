using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentBox : MonoBehaviour
{
	private const string animOpen = "slotbox_Open";

	private const string animDisappear = "slotbox_Disappear";

	public List<ParticleSystem> confetti;

	public Transform boxAnimation;

	public GameObject coinSpawn;

	public GameObject gemSpawn;

	public GameObject twoGemSpawn;

	public GameObject threeGemSpawn;

	public GameObject balloonSpawn;

	public GameObject coinBurst;

	public SoundEventData sfxGemSpawn;

	public SoundEventData sfxCoinBurst;

	public SoundEventData sfxPresentOpen;

	private GameObject _gem;

	private Vector3 _spawnLoc;

	private bool _notHit = true;

	private GameObject _balloon;

	private bool _isMoving;

	private Vector3 _endPosition = new Vector3(0f, 0.16f, 0.3f);

	private Vector3 _endScale = new Vector3(0.33f, 0.33f, 0.33f);

	private Vector3 _startPosition = Vector3.zero;

	public GameObject glow;

	private Quaternion _startRotation = Quaternion.identity;

	private Quaternion _endRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));

	private float _totalTime = 0.25f;

	private float _currentTime;

	private void OnEnable()
	{
		glow.GetComponent<Renderer>().enabled = false;
		Health.TookHit += TookHitHandler;
		if (_balloon == null)
		{
			GameObject gameObject = balloonSpawn;
			SpawnerChangeUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<SpawnerChangeUpgrade>();
			if (passiveUpgradeOrDefault != null)
			{
				gameObject = passiveUpgradeOrDefault.ReplaceIfNecessary(gameObject.transform).gameObject;
			}
			_balloon = UnityEngine.Object.Instantiate(gameObject, base.transform.position, base.transform.rotation) as GameObject;
			_balloon.transform.localScale = new Vector3(2f, 2f, 2f);
		}
		base.transform.parent = _balloon.transform;
		base.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
		base.transform.localPosition = new Vector3(0f, -0.21f, 0f);
		_balloon.transform.parent = LevelManager.Instance.currentScreenRoot.transform;
	}

	private void OnDisable()
	{
		Health.TookHit -= TookHitHandler;
		if ((bool)_gem)
		{
			UnityEngine.Object.Destroy(_gem);
		}
		if (_notHit)
		{
			SwrveEventsGameplay.PresentMissed();
		}
	}

	private void LateUpdate()
	{
		if (_isMoving && !GameManager.Instance.IsPaused && !HealingElixirScreen.IsActive && !GameManager.Instance.IsGameOver)
		{
			if (_currentTime <= _totalTime)
			{
				_currentTime += Time.deltaTime;
				float t = Mathf.Clamp01(_currentTime / _totalTime);
				base.transform.localPosition = Vector3.Lerp(_startPosition, _endPosition, t);
				base.transform.localRotation = Quaternion.Lerp(_startRotation, _endRotation, t);
			}
			else
			{
				glow.GetComponent<Renderer>().enabled = true;
				_isMoving = false;
				_currentTime = 0f;
			}
		}
	}

	public void TookHitHandler(object obj, EventArgs args)
	{
		if (_balloon != null && obj == _balloon.GetComponent<Health>())
		{
			base.transform.parent = LevelManager.Instance.currentScreenRoot.transform;
			base.GetComponent<Rigidbody>().isKinematic = false;
			base.GetComponent<Rigidbody>().useGravity = true;
		}
		if (obj == base.gameObject.GetComponent<Health>())
		{
			Health.TookHit -= TookHitHandler;
			base.gameObject.GetComponent<Health>().OnKilled();
			if (_notHit)
			{
				_notHit = false;
				base.gameObject.layer = Layers.EnemiesDontTarget;
				base.GetComponent<Rigidbody>().useGravity = false;
				base.GetComponent<Rigidbody>().isKinematic = true;
				base.transform.parent = ShipManager.instance.enemyProjectileTarget;
				_startRotation = base.transform.localRotation;
				_startPosition = base.transform.localPosition;
				_isMoving = true;
				StartCoroutine(RewardSequence());
			}
		}
	}

	public IEnumerator RewardSequence()
	{
		iTween.ScaleTo(base.gameObject, iTween.Hash("scale", _endScale, "time", 0.25f));
		base.gameObject.layer = Layers.LitHud;
		GameObjectUtils.SetLayerRecursive(base.gameObject, LayerMask.NameToLayer("LitHUD"));
		yield return new WaitForSeconds(0.1f);
		iTween.RotateBy(boxAnimation.gameObject, iTween.Hash("z", 5f, "time", 1f, "isLocal", true));
		yield return new WaitForSeconds(0.75f);
		_spawnLoc = base.GetComponent<Collider>().bounds.center;
		SoundEventManager.Instance.Play(sfxPresentOpen, base.gameObject);
		GameObjectUtils.HideObject(boxAnimation.gameObject);
		UnityEngine.Object.Destroy(glow);
		float randomValue = UnityEngine.Random.value;
		PresentBoxRewardTypes presentTypeToAward = (RankDataManager.Instance.CanAwardGameplayGem ? ((randomValue < PresentBoxManager.Instance.chanceToSpawnMagicItem) ? PresentBoxRewardTypes.MagicItem : ((randomValue < PresentBoxManager.Instance.chanceToSpawnGem + PresentBoxManager.Instance.chanceToSpawnMagicItem) ? PresentBoxRewardTypes.Gem : PresentBoxRewardTypes.Coins)) : ((randomValue < PresentBoxManager.Instance.chanceToSpawnMagicItem) ? PresentBoxRewardTypes.MagicItem : PresentBoxRewardTypes.Coins));
		if (presentTypeToAward == PresentBoxRewardTypes.MagicItem)
		{
			presentTypeToAward = PresentBoxRewardTypes.Coins;
		}
		if (DebugSettingsUI.forcePresentType.HasValue)
		{
			presentTypeToAward = DebugSettingsUI.forcePresentType.Value;
		}
		switch (presentTypeToAward)
		{
		case PresentBoxRewardTypes.Gem:
			SpawnGem();
			break;
		case PresentBoxRewardTypes.MagicItem:
		{
			PowerupData powerupData = MagicItemManager.Instance.ChooseMagicItem(true);
			if (powerupData == null)
			{
				goto default;
			}
			SpawnMagicItem(powerupData);
			break;
		}
		default:
			SpawnCoins();
			break;
		}
		SwrveEventsGameplay.PresentCollected(presentTypeToAward.ToString());
		yield return null;
	}

	private void SpawnMagicItem(PowerupData pd)
	{
		GameObject gameObject = MagicItemManager.Instance.SpawnMagicItem(pd);
		GameObjectUtils.SetLayerRecursive(gameObject, LayerMask.NameToLayer("LitHUD"));
		MagicItemCollectable componentInChildren = gameObject.GetComponentInChildren<MagicItemCollectable>();
		componentInChildren.StopMoving();
		componentInChildren.transform.parent.position = _spawnLoc;
		componentInChildren.transform.parent.parent = ShipManager.instance.enemyProjectileTarget;
		componentInChildren.transform.parent.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		iTween.PunchScale(componentInChildren.transform.parent.gameObject, iTween.Hash("amount", new Vector3(0.25f, 0.25f, 0.25f), "time", 0.25f));
		StartCoroutine(CollectMagicItem(componentInChildren));
		StartCoroutine(Disappear(2f));
	}

	private IEnumerator CollectMagicItem(MagicItemCollectable magicItem)
	{
		yield return new WaitForSeconds(0.25f);
		if ((bool)magicItem)
		{
			magicItem.Collect();
		}
	}

	private IEnumerator Disappear(float delay)
	{
		yield return new WaitForSeconds(delay);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void SpawnCoins()
	{
		SoundEventManager.Instance.Play(sfxCoinBurst, base.gameObject);
		if ((bool)coinBurst)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(coinBurst, base.transform.position, Quaternion.identity) as GameObject;
			gameObject.transform.parent = ShipManager.instance.enemyProjectileTarget;
		}
		int numCoins = PresentBoxManager.Instance.NumCoins;
		for (int i = 0; i < numCoins; i++)
		{
			GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(coinSpawn, _spawnLoc, Quaternion.identity);
			gameObject2.transform.parent = ShipManager.instance.enemyProjectileTarget;
			gameObject2.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
			Loot component = gameObject2.GetComponent<Loot>();
			component.autoCollect = true;
			component.autoCollectTime += (float)i * 0.05f;
			component.maxDistance /= 5f;
			component.maxSpeed = (float)numCoins * 0.005f;
		}
		StartCoroutine(Disappear(2f));
	}

	private void SpawnGem()
	{
		int num = Mathf.Clamp(PresentBoxManager.Instance.NumGems, 1, RankDataManager.Instance.NumGameplayGemsAvailable);
		GameObject gameObject = null;
		switch (num)
		{
		case 1:
			gameObject = gemSpawn;
			break;
		case 2:
			gameObject = twoGemSpawn;
			break;
		case 3:
			gameObject = threeGemSpawn;
			break;
		}
		if (gameObject == null)
		{
			Debug.LogWarning("Cannot spawn " + num + " gems! Spawning coins instead.");
			SpawnCoins();
		}
		else
		{
			SoundEventManager.Instance.Play(sfxGemSpawn, base.gameObject);
			SpawnGemFromPrefab(gameObject);
			StartCoroutine(Disappear(2f));
		}
	}

	private void SpawnGemFromPrefab(GameObject prefab)
	{
		_gem = UnityEngine.Object.Instantiate(prefab, _spawnLoc, Quaternion.identity) as GameObject;
		_gem.transform.parent = ShipManager.instance.enemyProjectileTarget;
		_gem.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
		GameObjectUtils.SetLayerRecursive(_gem, LayerMask.NameToLayer("LitHUD"));
		iTween.PunchScale(_gem, iTween.Hash("amount", new Vector3(0.25f, 0.25f, 0.25f), "time", 1f));
	}
}
