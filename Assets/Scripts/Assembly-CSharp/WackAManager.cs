using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WackAManager : SafeMonoBehaviour
{
	private const int _maxEasyBossRoomEnemies = 7;

	public List<GameObject> mySpawners;

	public GameObject[] bombSpawners;

	public BossWantedPoster bossWantedPosterPrefab;

	public int numToKill = 5;

	public int numToKillEasy;

	public GameObject scoreLoc;

	public GameObject[] objsToDestroy;

	public GameObject[] explodingThings;

	public GameObject explodingFX;

	public GameObject[] removeOnExplode;

	public GameObject comboNumber;

	public ComboCoin smallCoinPrefab;

	public GameObject[] disableUntilStarted;

	private int _enemiesKilledSoFar;

	private BossWantedPoster _wantedPosterInstance;

	private GameObject _bossText;

	private bool _changedMusic;

	private HashSet<GameObject> _spawnedHazards = new HashSet<GameObject>();

	private HashSet<GameObject> _spawnedShootables = new HashSet<GameObject>();

	private bool _doneSpawningForRound;

	private bool _finishedKillingEnemies;

	private float m_TimeLeft;

	private int _waveNumber;

	public static bool IsActive { get; private set; }

	public static event EventHandler<EventArgs> BossRoomVictoryAnimation;

	public void Start()
	{
		_wantedPosterInstance = ((GameObject)UnityEngine.Object.Instantiate(bossWantedPosterPrefab.gameObject, base.transform.position, Quaternion.identity)).GetComponent<BossWantedPoster>();
		_wantedPosterInstance.transform.parent = base.transform.parent.parent;
		_bossText = GameObject.FindWithTag("BossText");
		_bossText.GetComponent<Renderer>().enabled = false;
	}

	private void OnEnable()
	{
		IsActive = true;
		m_TimeLeft = -1f;
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
		PowerupCutscene.CutsceneStarted += HandlePowerupCutsceneCutsceneStarted;
		PeekabooTroll.WantsToHide += HandlePeekaBooTrollWantsToHide;
		PeekabooTroll.PeekabooStateChanged += HandlePeekabooTrollStateChanged;
		GameManager.GameStateChanged += HandleGameManagerGameStateChanged;
		GameObject[] array = disableUntilStarted;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
	}

	private void OnDisable()
	{
		IsActive = false;
		StopAllCoroutines();
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
		PowerupCutscene.CutsceneStarted -= HandlePowerupCutsceneCutsceneStarted;
		PeekabooTroll.WantsToHide -= HandlePeekaBooTrollWantsToHide;
		PeekabooTroll.PeekabooStateChanged -= HandlePeekabooTrollStateChanged;
		GameManager.GameStateChanged -= HandleGameManagerGameStateChanged;
		if (_bossText != null)
		{
			_bossText.GetComponent<Renderer>().enabled = false;
		}
		if (_changedMusic && GameManager.gameState == GameManager.GameState.Playing && !SafeMonoBehaviour.IsShuttingDown && !HealingElixirScreen.IsActive)
		{
			MusicManager.Instance.StopMusic();
			return;
		}
		SoundEventManager.Instance.Stop(GlobalSoundEventData.Instance.Boss_Intro_SFX_Tally, base.gameObject);
		SoundEventManager.Instance.Stop(GlobalSoundEventData.Instance.Boss_Intro_Stinger1, base.gameObject);
		SoundEventManager.Instance.Stop(GlobalSoundEventData.Instance.Bonus_Intro_Tally_Total, base.gameObject);
		SoundEventManager.Instance.Stop2D(GlobalSoundEventData.Instance.Boss_Outro_Stinger1);
		SoundEventManager.Instance.Stop(GlobalSoundEventData.Instance.Coin_SFX_Burst_medium, base.gameObject);
		SoundEventManager.Instance.Stop(GlobalSoundEventData.Instance.Boss_Outro_Explode, base.gameObject);
		SoundEventManager.Instance.Stop(GlobalSoundEventData.Instance.Boss_Coin_Fountain, base.gameObject);
		SoundEventManager.Instance.Stop(GlobalSoundEventData.Instance.Boss_Castle_Explode, base.gameObject);
	}

	private void HandleGameManagerGameStateChanged(object sender, GameManager.GameStateChangedEventArgs e)
	{
		if (e.NewState == GameManager.GameState.Dead)
		{
			Debug.Log(string.Concat("GameState changing: ", e.OldState, " -> ", e.NewState, " --- SHUT. IT. DOWN."));
			StopAllCoroutines();
		}
	}

	private void HandlePeekabooTrollStateChanged(object sender, EventArgs e)
	{
		PeekabooTroll peekabooTroll = (PeekabooTroll)sender;
		if (peekabooTroll.PeekabooState == PeekabooStates.Down)
		{
			ObjectSpawner.DestroySpawnedInstance(peekabooTroll.gameObject);
		}
	}

	private void HandlePeekaBooTrollWantsToHide(object sender, CancellableEventArgs e)
	{
		if (!TryLetTrollsHide((PeekabooTroll)sender))
		{
			e.Cancel();
		}
	}

	private bool TryLetTrollsHide(PeekabooTroll excludedTroll)
	{
		if (!_doneSpawningForRound)
		{
			return false;
		}
		PeekabooTroll[] array = (from shootable in _spawnedShootables
			where shootable != null
			select shootable.GetComponent<PeekabooTroll>() into troll
			where troll != null && troll != excludedTroll
			select troll).ToArray();
		if (array.Where((PeekabooTroll troll) => !troll.IsWaitingToHide || troll.PeekabooState != PeekabooStates.Up).Any())
		{
			return false;
		}
		PeekabooTroll[] array2 = array;
		foreach (PeekabooTroll peekabooTroll in array2)
		{
			peekabooTroll.AllowHiding();
		}
		return true;
	}

	private IEnumerator StartRoundsCoroutine()
	{
		if (WingedBoots.IsActive || RocketBooster.IsActive)
		{
			yield break;
		}
		yield return WaitForGame();
		_wantedPosterInstance.text.Text = numToKill.ToString();
		SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.Boss_Intro_Stinger1, base.gameObject);
		_bossText.GetComponent<Renderer>().enabled = true;
		_wantedPosterInstance.Play();
		yield return WaitForSecondsAndGame(0.5f);
		MusicManager.Instance.StopMusic();
		_changedMusic = true;
		yield return WaitForSecondsAndGame(2.16f);
		m_TimeLeft = numToKill;
		MusicManager.Instance.PlayNextBossMusic();
		iTween.MoveTo(_wantedPosterInstance.gameObject, iTween.Hash("position", scoreLoc.transform.position + new Vector3(0f, 0f, 0.2f), "time", 0.5f));
		SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.Bonus_Intro_Tally_Total, base.gameObject);
		yield return WaitForSecondsAndGame(0.5f);
		GameObject[] array = disableUntilStarted;
		foreach (GameObject bomb in array)
		{
			if (bomb != null)
			{
				bomb.SetActive(true);
			}
		}
		_bossText.GetComponent<Renderer>().enabled = false;
		DifficultyManager.Instance.StartBossRoom();
		while (!_finishedKillingEnemies)
		{
			Debug.Log("Starting new wave...");
			yield return StartCoroutine(SpawnTrollsForRound());
			float timeoutTime = Time.time + 7f;
			bool anyShootablesLeft = true;
			while (anyShootablesLeft && Time.time < timeoutTime)
			{
				yield return WaitForEndOfFrameAndGame();
				anyShootablesLeft = false;
				foreach (GameObject spawnedObject2 in _spawnedShootables)
				{
					if (spawnedObject2 != null)
					{
						anyShootablesLeft = true;
						break;
					}
				}
			}
			Debug.Log("All enemies in wave killed (or timeout)! - " + anyShootablesLeft);
			foreach (GameObject spawnedObject in _spawnedHazards.Concat(_spawnedShootables))
			{
				if (spawnedObject != null)
				{
					ObjectSpawner.DestroySpawnedInstance(spawnedObject);
				}
			}
			Debug.Log("Done Destroying stuff...");
			yield return WaitForSecondsAndGame(0.25f);
		}
	}

	private IEnumerator SpawnTrollsForRound()
	{
		_spawnedHazards.Clear();
		_spawnedShootables.Clear();
		_doneSpawningForRound = false;
		List<GameObject> spawnersRemaining = new List<GameObject>(mySpawners);
		bool isFirstSpawn = true;
		int minimumShootables = 2;
		while (spawnersRemaining.Any() && GameManager.gameState == GameManager.GameState.Playing)
		{
			if (!isFirstSpawn)
			{
				yield return WaitForSecondsAndGame(0.1f);
			}
			int remainingShootablesRequired = minimumShootables - _spawnedShootables.Count;
			bool forceShootable = remainingShootablesRequired > 0 && spawnersRemaining.Count <= remainingShootablesRequired;
			int randomSpawnIndex = UnityEngine.Random.Range(0, spawnersRemaining.Count);
			ObjectSpawner spawner = spawnersRemaining[randomSpawnIndex].GetComponent<ObjectSpawner>();
			GameObject spawnedObject = spawner.SpawnObject(0f, forceShootable);
			if (ObjectSpawner.IsShootable(spawnedObject))
			{
				_spawnedShootables.Add(spawnedObject);
			}
			else
			{
				_spawnedHazards.Add(spawnedObject);
			}
			spawnersRemaining.RemoveAt(randomSpawnIndex);
			isFirstSpawn = false;
		}
		_doneSpawningForRound = true;
		TryLetTrollsHide(null);
		_waveNumber++;
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		StartCoroutine(StartRoundsCoroutine());
	}

	private void Update()
	{
		if (!(m_TimeLeft < 0f))
		{
			m_TimeLeft -= Time.deltaTime;
			_wantedPosterInstance.text.Text = Mathf.Ceil(m_TimeLeft).ToString();
			if (m_TimeLeft < 0f)
			{
				StartCoroutine(FinishRoomAfterShootingDone());
			}
		}
	}

	private void HandleHealthKilled(object sender, EventArgs e)
	{
		Health health = (Health)sender;
		if (health.gameObject.CompareTag("Enemy"))
		{
			_enemiesKilledSoFar++;
			int num = numToKill - _enemiesKilledSoFar;
			if (num < 0)
			{
				num = 0;
			}
			PeekabooTroll component = health.GetComponent<PeekabooTroll>();
			TryLetTrollsHide(component);
			_wantedPosterInstance.text.Text = num.ToString();
			if (num == 0)
			{
				StartCoroutine(FinishRoomAfterShootingDone());
			}
		}
	}

	private void HandlePowerupCutsceneCutsceneStarted(object sender, EventArgs e)
	{
		if (_bossText.GetComponent<Renderer>().enabled)
		{
			_bossText.GetComponent<Renderer>().enabled = false;
		}
	}

	private IEnumerator FinishRoomAfterShootingDone()
	{
		while (ShipManager.instance.isShooting)
		{
			yield return new WaitForEndOfFrame();
		}
		FinishRoom();
	}

	private void FinishRoom()
	{
		if (_finishedKillingEnemies)
		{
			return;
		}
		_finishedKillingEnemies = true;
		StopAllCoroutines();
		DifficultyManager.Instance.EndBossRoom();
		float num = 0.2f;
		float num2 = 0.2f;
		for (int i = 0; i < mySpawners.Count; i++)
		{
			StartCoroutine(SpawnSmallCoin(mySpawners[i].transform.position, num));
			UnityEngine.Object.Destroy(mySpawners[i]);
			num += num2;
		}
		for (int j = 0; j < bombSpawners.Length; j++)
		{
			UnityEngine.Object.Destroy(bombSpawners[j]);
		}
		if (objsToDestroy.Length > 0)
		{
			for (int k = 0; k < objsToDestroy.Length; k++)
			{
				if (objsToDestroy[k] != null)
				{
					UnityEngine.Object.Instantiate(ShipManager.instance.moneyDrop, objsToDestroy[k].transform.position, Quaternion.identity);
					UnityEngine.Object.Destroy(objsToDestroy[k]);
				}
			}
		}
		SoundEventManager.Instance.Play2D(GlobalSoundEventData.Instance.Boss_Outro_Stinger1);
		MusicManager.Instance.StopMusic();
		GameManager.ExplodeBombs();
		Health[] componentsInChildren = LevelManager.Instance.CurrentScreenManager.GetComponentsInChildren<Health>();
		if (componentsInChildren.Length > 0)
		{
			SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.Coin_SFX_Burst_medium, base.gameObject);
		}
		Health[] array = componentsInChildren;
		foreach (Health health in array)
		{
			if (health.poofEffect != null)
			{
				UnityEngine.Object.Instantiate(health.poofEffect, health.transform.position, Quaternion.identity);
			}
			SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.EnemyPoof, base.gameObject);
			UnityEngine.Object.Instantiate(ShipManager.instance.moneyDrop, health.transform.position, Quaternion.identity);
			UnityEngine.Object.Destroy(health.gameObject);
		}
		GameManager.KillAllProjectiles();
		SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.Boss_Coin_Fountain, base.gameObject);
		StartCoroutine(FinishExplosionSequence());
	}

	private IEnumerator SpawnSmallCoin(Vector3 position, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (smallCoinPrefab != null)
		{
			ComboCoin coin = UnityEngine.Object.Instantiate(smallCoinPrefab, position, Quaternion.identity) as ComboCoin;
			coin.transform.parent = LevelManager.Instance.CurrentScreenManager.transform.parent;
			coin.isSheep = false;
			coin.isElemental = true;
			coin.fixedZDepth = false;
		}
	}

	private void SpawnScore(Vector3 position)
	{
		ClearHideoutScoreBonusUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<ClearHideoutScoreBonusUpgrade>();
		if (passiveUpgradeOrDefault != null)
		{
			ScoreKeeper.Instance.AddScore(ScoreData.ScoreType.ROOMCLEARED_UPGRADE, position, true);
		}
		else
		{
			ScoreKeeper.Instance.AddScore(ScoreData.ScoreType.ROOMCLEARED, position, true);
		}
	}

	private IEnumerator FinishExplosionSequence()
	{
		if (_wantedPosterInstance != null)
		{
			UnityEngine.Object.Destroy(_wantedPosterInstance.gameObject);
		}
		if (explodingThings.Any() && explodingFX != null)
		{
			Vector3 explosionOffset = new Vector3(0f, 0.5f, 0.5f);
			GameObject[] array = explodingThings;
			foreach (GameObject explodingThing in array)
			{
				Vector3 explosionPosition = explodingThing.transform.position + explosionOffset;
				SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.Boss_Castle_Explode, base.gameObject);
				UnityEngine.Object.Instantiate(explodingFX, explosionPosition, Quaternion.identity);
				SpawnScore(explosionPosition);
				yield return new WaitForSeconds(0.25f);
			}
		}
		SoundEventManager.Instance.Stop(GlobalSoundEventData.Instance.Boss_Coin_Fountain, base.gameObject);
		SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.Boss_Outro_Explode, base.gameObject);
		SoundEventManager.Instance.Play2D(GlobalSoundEventData.Instance.Boss_Outro_Stinger2);
		GameObject[] array2 = removeOnExplode;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
		if (comboNumber != null)
		{
			GameObject comboNumberInstance = (GameObject)UnityEngine.Object.Instantiate(comboNumber, scoreLoc.transform.position + new Vector3(0f, 0.2f, 0f), Quaternion.identity);
			ComboCoin comboNumberCoin = comboNumberInstance.GetComponent<ComboCoin>();
			comboNumberCoin.number = 6;
			comboNumberCoin.isSheep = true;
		}
		OnBossRoomVictoryAnimation();
		GameObject[] array3 = explodingThings;
		foreach (GameObject explodingObject in array3)
		{
			SoundEventManager.Instance.Play(GlobalSoundEventData.Instance.Coin_SFX_Burst_medium, base.gameObject);
			UnityEngine.Object.Instantiate(ShipManager.instance.moneyDrop, explodingObject.transform.position, Quaternion.identity);
			BossBarrier explodingObjectBarrier = explodingObject.GetComponent<BossBarrier>();
			if (explodingObjectBarrier != null)
			{
				explodingObjectBarrier.Explode();
				continue;
			}
			BoxCollider boxCollider2 = explodingObject.GetComponent<BoxCollider>();
			if (boxCollider2 == null)
			{
				explodingObject.AddComponent<BoxCollider>();
				boxCollider2 = explodingObject.GetComponent<BoxCollider>();
			}
			if (explodingObject.GetComponent<Rigidbody>() == null)
			{
				explodingObject.AddComponent<Rigidbody>();
			}
			explodingObject.GetComponent<Rigidbody>().mass = 2f;
			explodingObject.GetComponent<Rigidbody>().useGravity = true;
			explodingObject.GetComponent<Rigidbody>().AddForce(UnityEngine.Random.insideUnitSphere * 800f);
			explodingObject.GetComponent<Rigidbody>().AddTorque(UnityEngine.Random.insideUnitSphere * 4225f);
		}
		StartCoroutine(DestroySelf(0.33f));
	}

	private IEnumerator DestroySelf(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (GameManager.gameState == GameManager.GameState.Playing)
		{
		}
		_changedMusic = false;
		yield return new WaitForSeconds(2f);
		UnityEngine.Object.Destroy(base.gameObject);
		HealthBar.Instance.Die();
	}

	private void OnBossRoomVictoryAnimation()
	{
		if (WackAManager.BossRoomVictoryAnimation != null)
		{
			WackAManager.BossRoomVictoryAnimation(this, new EventArgs());
		}
	}
}
