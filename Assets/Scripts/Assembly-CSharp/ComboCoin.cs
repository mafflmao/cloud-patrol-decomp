using System;
using UnityEngine;

public class ComboCoin : MonoBehaviour
{
	public const int MAX_SPAWNABLE_COINS = 30;

	public const int LOW_QUALITY_MAX_SPAWNABLE_COINS = 20;

	public Material[] comboMaterials;

	public Material sheepMaterial;

	public GameObject comboNumberVisual;

	public int number;

	public float lootSpawnTime;

	public GameObject lootToSpawn;

	public bool autoCollectLoot;

	public bool max;

	public bool isSheep;

	public bool isElemental;

	public float torque;

	public float upwardsForce;

	public GameObject fx;

	public GameObject maxFX;

	public bool fixedZDepth = true;

	public SoundEventData smallSpawn;

	public SoundEventData mediumSpawn;

	public SoundEventData largeSpawn;

	public SoundEventData smallPop;

	public SoundEventData mediumPop;

	public SoundEventData largePop;

	public float lifeTime = 10f;

	public static event EventHandler<CancellableEventArgs> CoinTimeout;

	public static event EventHandler ComboCoinSpawned;

	public static event EventHandler<EventArgs> Collected;

	public void Pop()
	{
		SpawnLoot();
		OnCollected();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Start()
	{
		if (isElemental)
		{
			base.transform.Translate(new Vector3(0f, 0.2f, 0f));
		}
		base.GetComponent<Rigidbody>().AddTorque(new Vector3(0f, torque, 0f));
		base.GetComponent<Rigidbody>().AddForce(new Vector3(0f, upwardsForce, 0f));
		if (isElemental)
		{
			comboNumberVisual.GetComponent<Renderer>().material = comboMaterials[(int)StartGameSettings.Instance.activeSkylander.elementData.elementType];
		}
		else if (isSheep)
		{
			comboNumberVisual.GetComponent<Renderer>().material = sheepMaterial;
		}
		else if (number < comboMaterials.Length)
		{
			comboNumberVisual.GetComponent<Renderer>().material = comboMaterials[number];
		}
		if (number <= 3)
		{
			SoundEventManager.Instance.Play(smallSpawn, base.gameObject);
		}
		if (number == 4 || number == 5)
		{
			SoundEventManager.Instance.Play(mediumSpawn, base.gameObject);
		}
		if (number == 6)
		{
			SoundEventManager.Instance.Play(largeSpawn, base.gameObject);
		}
		GameManager.sessionStats.IncrementComboCoinSpawn(number);
		GameObject currentScreenRoot = LevelManager.Instance.currentScreenRoot;
		if (currentScreenRoot != null)
		{
			base.transform.parent = currentScreenRoot.transform;
		}
		if (fixedZDepth)
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
			Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(vector.x, vector.y, 4f));
			base.transform.position = position;
		}
		OnComboCoinSpawned();
		InvokeHelper.InvokeSafe(DestroyAfterTimeout, lifeTime, this);
		LevelManager.MovingToNextRoom += HandleLevelManagerMovingToNextRoom;
	}

	private void OnDestroy()
	{
		LevelManager.MovingToNextRoom -= HandleLevelManagerMovingToNextRoom;
	}

	private void HandleLevelManagerMovingToNextRoom(object sender, EventArgs e)
	{
		Pop();
	}

	private void SpawnLoot()
	{
		GameManager.sessionStats.IncrementComboCoinCollect(number);
		ScoreKeeper.Instance.AddScore(number * LevelManager.Instance.m_CoinBaseScore, base.transform.position, true);
		if (number <= 3)
		{
			UnityEngine.Object.Instantiate(fx, base.transform.position, Quaternion.identity);
			SoundEventManager.Instance.Play(smallPop, base.gameObject);
		}
		if (number == 4 || number == 5)
		{
			UnityEngine.Object.Instantiate(fx, base.transform.position, Quaternion.identity);
			SoundEventManager.Instance.Play(mediumPop, base.gameObject);
		}
		if (number == 6)
		{
			GameManager.CameraShake();
			UnityEngine.Object.Instantiate(maxFX, base.transform.position, Quaternion.identity);
			SoundEventManager.Instance.Play(largePop, base.gameObject);
			BombsExplodeOnComboNumberUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<BombsExplodeOnComboNumberUpgrade>();
			if (passiveUpgradeOrDefault != null)
			{
				passiveUpgradeOrDefault.Explode();
			}
		}
		int comboCoinPayout = SwrveEconomy.GetComboCoinPayout(number);
		int num = ((!PlatformUtils.IsLowQualityPlatform) ? 30 : 20);
		float num2 = (float)num / (float)SwrveEconomy.GetComboCoinPayout(6);
		int num3 = Mathf.RoundToInt((float)comboCoinPayout * num2);
		int delta = comboCoinPayout - num3;
		float num4 = 0.5f / (float)num3;
		for (int i = 0; i < num3; i++)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(lootToSpawn, base.transform.position, Quaternion.identity);
			Loot component = gameObject.GetComponent<Loot>();
			if (autoCollectLoot)
			{
				component.autoCollect = true;
				component.autoCollectTime += (float)i * num4;
			}
		}
		GameManager.GotMoney(delta);
	}

	private void DestroyAfterTimeout()
	{
		CancellableEventArgs cancellableEventArgs = new CancellableEventArgs();
		OnCoinTimeout(cancellableEventArgs);
		if (!cancellableEventArgs.IsCancelled)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnComboCoinSpawned()
	{
		if (ComboCoin.ComboCoinSpawned != null)
		{
			ComboCoin.ComboCoinSpawned(this, new EventArgs());
		}
	}

	private void OnCoinTimeout(CancellableEventArgs args)
	{
		if (ComboCoin.CoinTimeout != null)
		{
			ComboCoin.CoinTimeout(this, args);
		}
	}

	private void OnCollected()
	{
		if (ComboCoin.Collected != null)
		{
			ComboCoin.Collected(this, new EventArgs());
		}
		LevelManager.MovingToNextRoom -= HandleLevelManagerMovingToNextRoom;
	}
}
