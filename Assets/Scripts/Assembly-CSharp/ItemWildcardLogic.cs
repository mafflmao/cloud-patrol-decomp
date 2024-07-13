using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemWildcardLogic : MonoBehaviour
{
	public enum ActivationType
	{
		Spawn = 0,
		Queue = 1,
		Trigger = 2
	}

	[Serializable]
	public class WildcardWeightedPowerupData
	{
		public float weight;

		public PowerupData powerupData;
	}

	public Color enabledColor;

	public Color disabledColor;

	public PowerupData RandomizerItem;

	public float UnlockedPowerupWeight;

	public float LockedPowerupWeight;

	public WildcardWeightedPowerupData[] extraRollItems;

	public int RollIterations = 12;

	public float VisualRollUpdateTime = 0.02f;

	public ActivationType WildcardActivationType = ActivationType.Queue;

	public GameObject MagicItemCollectable;

	public GameObject SpawnEffectEmitter;

	public GameObject RollViewer;

	public GameObject ChargeCounter;

	public Texture SpinnerTextureNormal;

	public Texture SpinnerTextureSpinning;

	public SoundEventData spinSound;

	public SoundEventData spinFailSound;

	private Dictionary<string, PowerupData> _tieBreakers = new Dictionary<string, PowerupData>();

	private bool _useLeftSlot;

	private bool ShouldDisplayRandomizer()
	{
		return true;
	}

	private void Start()
	{
		RollViewer.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0f);
		SetChargeCounter(RandomizerItem.consumablesHeld);
		if (!ShouldDisplayRandomizer())
		{
			GameObjectUtils.HideObject(base.gameObject.transform.parent.gameObject);
		}
		else
		{
			GameObjectUtils.ShowObject(base.gameObject.transform.parent.gameObject);
		}
		InitializeTieBreakers();
		InitializePropertiesFromSwrve();
	}

	private void Awake()
	{
		WildcardWeightedPowerupData[] array = extraRollItems;
		foreach (WildcardWeightedPowerupData wildcardWeightedPowerupData in array)
		{
			wildcardWeightedPowerupData.powerupData.LoadPowerupPrefabFromResources();
		}
	}

	private void OnDestroy()
	{
		WildcardWeightedPowerupData[] array = extraRollItems;
		foreach (WildcardWeightedPowerupData wildcardWeightedPowerupData in array)
		{
			wildcardWeightedPowerupData.powerupData.ReleasePowerupPrefab();
		}
	}

	private void OnEnable()
	{
		FingerGestures.OnFingerDown += HandleTouchScreenGesturesOnFingerDown;
	}

	private void OnDisable()
	{
		FingerGestures.OnFingerDown -= HandleTouchScreenGesturesOnFingerDown;
	}

	private void InitializeTieBreakers()
	{
		_tieBreakers.Add("scoremultiplier02x", GetExtraRollItem("scoremultiplier05x"));
		_tieBreakers.Add("scoremultiplier05x", GetExtraRollItem("scoremultiplier10x"));
		_tieBreakers.Add("scoremultiplier10x", GetExtraRollItem("scoremultiplier25x"));
		_tieBreakers.Add("scoremultiplier25x", GetExtraRollItem("scoremultiplier50x"));
		_tieBreakers.Add("scoremultiplier50x", GetExtraRollItem("gemitem"));
		_tieBreakers.Add("rocketBooster", GetExtraRollItem("gemitem"));
		_tieBreakers.Add("gemitem", GetExtraRollItem("scoremultiplier50x"));
		_tieBreakers.Add("unlocked", GetExtraRollItem("scoremultiplier02x"));
		_tieBreakers.Add("locked", GetExtraRollItem("gemitem"));
	}

	private void InitializePropertiesFromSwrve()
	{
		Dictionary<string, string> resourceDictionary;
		Bedrock.GetRemoteUserResources(RandomizerItem.storageKey, out resourceDictionary);
		if (resourceDictionary != null)
		{
			UnlockedPowerupWeight = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "UnlockedPowerupWeight", UnlockedPowerupWeight);
			LockedPowerupWeight = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "LockedPowerupWeight", LockedPowerupWeight);
			SetExtraRollItemWeightFromSwrve(resourceDictionary, "scoremultiplier02x", "ScoreMultiplier02xWeight");
			SetExtraRollItemWeightFromSwrve(resourceDictionary, "scoremultiplier05x", "ScoreMultiplier05xWeight");
			SetExtraRollItemWeightFromSwrve(resourceDictionary, "scoremultiplier10x", "ScoreMultiplier10xWeight");
			SetExtraRollItemWeightFromSwrve(resourceDictionary, "scoremultiplier25x", "ScoreMultiplier25xWeight");
			SetExtraRollItemWeightFromSwrve(resourceDictionary, "scoremultiplier50x", "ScoreMultiplier50xWeight");
			SetExtraRollItemWeightFromSwrve(resourceDictionary, "gemitem", "GemItemWeight");
			SetExtraRollItemWeightFromSwrve(resourceDictionary, "rocketBooster", "RocketBoosterWeight");
		}
	}

	private void SetExtraRollItemWeightFromSwrve(Dictionary<string, string> swrveResources, string swrveItemAttribute, string rollItemStorageKey)
	{
		float extraRollItemWeight = GetExtraRollItemWeight(rollItemStorageKey);
		float fromResourceDictionaryAsFloat = Bedrock.GetFromResourceDictionaryAsFloat(swrveResources, swrveItemAttribute, extraRollItemWeight);
		SetExtraRollItemWeight(rollItemStorageKey, fromResourceDictionaryAsFloat);
	}

	private void SetExtraRollItemWeight(string rollItemStorageKey, float weight)
	{
		WildcardWeightedPowerupData[] array = extraRollItems;
		foreach (WildcardWeightedPowerupData wildcardWeightedPowerupData in array)
		{
			if (wildcardWeightedPowerupData.powerupData.storageKey == rollItemStorageKey)
			{
				wildcardWeightedPowerupData.weight = weight;
				break;
			}
		}
	}

	private float GetExtraRollItemWeight(string rollItemStorageKey)
	{
		WildcardWeightedPowerupData[] array = extraRollItems;
		foreach (WildcardWeightedPowerupData wildcardWeightedPowerupData in array)
		{
			if (wildcardWeightedPowerupData.powerupData.storageKey == rollItemStorageKey)
			{
				return wildcardWeightedPowerupData.weight;
			}
		}
		return 0f;
	}

	private PowerupData GetExtraRollItem(string rollItemStorageKey)
	{
		WildcardWeightedPowerupData[] array = extraRollItems;
		foreach (WildcardWeightedPowerupData wildcardWeightedPowerupData in array)
		{
			if (wildcardWeightedPowerupData.powerupData.storageKey == rollItemStorageKey)
			{
				return wildcardWeightedPowerupData.powerupData;
			}
		}
		return null;
	}

	private bool IsExtraRollItem(PowerupData powerupData)
	{
		WildcardWeightedPowerupData[] array = extraRollItems;
		foreach (WildcardWeightedPowerupData wildcardWeightedPowerupData in array)
		{
			if (wildcardWeightedPowerupData.powerupData.storageKey == powerupData.storageKey)
			{
				return true;
			}
		}
		return false;
	}

	private void SetChargeCounter(int charges)
	{
		SpriteText component = ChargeCounter.GetComponent<SpriteText>();
		if (charges == 0)
		{
			base.GetComponent<Renderer>().material.SetColor("_Color", disabledColor);
		}
		else
		{
			base.GetComponent<Renderer>().material.SetColor("_Color", enabledColor);
		}
		if (charges > 99)
		{
			component.Text = "99+";
		}
		else
		{
			component.Text = charges.ToString();
		}
	}

	public int GetActiveSlotCount()
	{
		int num = Mathf.Min(1, ShipManager.instance.powerupHolders.Length);
		return Mathf.Min(2, ShipManager.instance.powerupHolders.Length);
	}

	private PowerupHolder GetCurrentPowerupHolder()
	{
		PowerupHolder powerupHolder = null;
		PowerupHolder[] powerupHolders = ShipManager.instance.powerupHolders;
		for (int i = 0; i < GetActiveSlotCount(); i++)
		{
			if (powerupHolders[i].State != PowerupStates.active)
			{
				powerupHolder = powerupHolders[i];
				if (powerupHolder.State == PowerupStates.hidden || (!_useLeftSlot && !MagicItemManager.IsMagicItemActive))
				{
					break;
				}
			}
		}
		return powerupHolder;
	}

	private List<WildcardWeightedPowerupData> GetWeightedItemRollSet()
	{
		List<WildcardWeightedPowerupData> list = new List<WildcardWeightedPowerupData>();
		float weight = UnlockedPowerupWeight / (float)MagicItemManager.Instance.UnlockedMagicItems.Count();
		foreach (PowerupData unlockedMagicItem in MagicItemManager.Instance.UnlockedMagicItems)
		{
			list.Add(new WildcardWeightedPowerupData
			{
				weight = weight,
				powerupData = unlockedMagicItem
			});
		}
		if (MagicItemManager.Instance.LockedMagicItems.Count() > 0)
		{
			float weight2 = LockedPowerupWeight / (float)MagicItemManager.Instance.LockedMagicItems.Count();
			foreach (PowerupData lockedMagicItem in MagicItemManager.Instance.LockedMagicItems)
			{
				list.Add(new WildcardWeightedPowerupData
				{
					weight = weight2,
					powerupData = lockedMagicItem
				});
			}
		}
		else
		{
			float num = LockedPowerupWeight / (float)MagicItemManager.Instance.UnlockedMagicItems.Count();
			foreach (WildcardWeightedPowerupData item in list)
			{
				item.weight += num;
			}
		}
		WildcardWeightedPowerupData[] array = extraRollItems;
		foreach (WildcardWeightedPowerupData wildcardWeightedPowerupData in array)
		{
			if (!(wildcardWeightedPowerupData.powerupData.storageKey == "gemitem") || RankDataManager.Instance.NumGameplayGemsAvailable > 0)
			{
				int num2 = 1;
				RandomizerMultiplierChanceBonusUpgrade passiveUpgradeOrDefault = CharacterUpgradeManager.Instance.GetPassiveUpgradeOrDefault<RandomizerMultiplierChanceBonusUpgrade>();
				if (passiveUpgradeOrDefault != null)
				{
					num2 = passiveUpgradeOrDefault.multiplier;
				}
				list.Add(new WildcardWeightedPowerupData
				{
					weight = wildcardWeightedPowerupData.weight * (float)num2,
					powerupData = wildcardWeightedPowerupData.powerupData
				});
			}
		}
		list.Shuffle();
		float num3 = 0f;
		foreach (WildcardWeightedPowerupData item2 in list)
		{
			num3 = (item2.weight = num3 + item2.weight);
		}
		return list;
	}

	private PowerupData DecideItem(List<WildcardWeightedPowerupData> powerups)
	{
		float weight = powerups[powerups.Count - 1].weight;
		float num = UnityEngine.Random.value * weight;
		foreach (WildcardWeightedPowerupData powerup in powerups)
		{
			if (num < powerup.weight)
			{
				return powerup.powerupData;
			}
		}
		return powerups[powerups.Count - 1].powerupData;
	}

	private PowerupData EnsureDifferentItem(PowerupData currentlyDecidedItem)
	{
		PowerupData powerupData = null;
		PowerupHolder currentPowerupHolder = GetCurrentPowerupHolder();
		if (currentPowerupHolder.CurrentPowerup != null && currentPowerupHolder.CurrentPowerup.storageKey == currentlyDecidedItem.storageKey)
		{
			if (!IsExtraRollItem(currentlyDecidedItem))
			{
				powerupData = ((1 == 0) ? _tieBreakers["locked"] : _tieBreakers["unlocked"]);
			}
			else if (_tieBreakers.ContainsKey(currentlyDecidedItem.storageKey))
			{
				powerupData = _tieBreakers[currentlyDecidedItem.storageKey];
			}
		}
		return (!(powerupData != null)) ? currentlyDecidedItem : powerupData;
	}

	private void ActivateBySpawning(PowerupData item)
	{
		SpawnPowerup(item);
	}

	private void ActivateByQueueing(PowerupData item)
	{
		PowerupHolder currentPowerupHolder = GetCurrentPowerupHolder();
		if (currentPowerupHolder != null)
		{
			currentPowerupHolder.QueuePowerup(item, false);
			if (!MagicItemManager.IsMagicItemActive)
			{
				_useLeftSlot = !_useLeftSlot;
			}
		}
		else
		{
			Debug.LogWarning("[ItemWildcardLogic] Could not find any ready-state powerup holders to queue an item!");
		}
	}

	private void ActivateByTriggering(PowerupData item)
	{
		PowerupHolder currentPowerupHolder = GetCurrentPowerupHolder();
		if (currentPowerupHolder != null)
		{
			item.Trigger(currentPowerupHolder, false);
		}
		else
		{
			Debug.LogWarning("[ItemWildcardLogic] Could not find any ready-state powerup holders to trigger an item!");
		}
	}

	private void Activate(PowerupData item)
	{
		SwrveEventsGameplay.MagicItemWildcardTriggered(item.storageKey);
		if (WildcardActivationType == ActivationType.Spawn)
		{
			ActivateBySpawning(item);
		}
		else if (WildcardActivationType == ActivationType.Queue)
		{
			ActivateByQueueing(item);
		}
		else if (WildcardActivationType == ActivationType.Trigger)
		{
			ActivateByTriggering(item);
		}
	}

	private void BeginUpdateUI()
	{
		base.GetComponent<Renderer>().material.mainTexture = SpinnerTextureSpinning;
		PowerupHolder currentPowerupHolder = GetCurrentPowerupHolder();
		if (currentPowerupHolder != null)
		{
			RollViewer.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
			RollViewer.transform.position = currentPowerupHolder.transform.position;
			RollViewer.transform.Translate(0f, 0f, 0.02f);
		}
		else
		{
			Debug.LogWarning("[ItemWildcardLogic] Could not find any ready-state powerup holders to position the roll viewer over!");
		}
		SetChargeCounter(RandomizerItem.consumablesHeld);
	}

	private void UpdateUI(List<WildcardWeightedPowerupData> rollItems, int rollIteration, int rollStartIndex)
	{
		if (rollItems.Count != 0)
		{
			RollViewer.GetComponent<Renderer>().material.mainTexture = rollItems[(rollIteration + rollStartIndex) % rollItems.Count].powerupData.inGameButtonTexture;
		}
		base.transform.Rotate(0f, 0f, 30f);
	}

	private void EndUpdateUI()
	{
		base.GetComponent<Renderer>().material.mainTexture = SpinnerTextureNormal;
		RollViewer.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0f);
	}

	private void ResetUI()
	{
	}

	private IEnumerator TriggerLogicCoroutine()
	{
		RandomizerItem.consumablesHeld--;
		List<WildcardWeightedPowerupData> rollItems = GetWeightedItemRollSet();
		PowerupData item = EnsureDifferentItem(DecideItem(rollItems));
		int rollToIndex = rollItems.FindIndex((WildcardWeightedPowerupData x) => x.powerupData.storageKey == item.storageKey);
		int rollStartIndex = rollToIndex - RollIterations + 1;
		if (rollStartIndex < 0)
		{
			rollStartIndex = rollItems.Count + rollStartIndex;
		}
		BeginUpdateUI();
		SoundEventManager.Instance.Play2D(spinSound, 0f);
		for (int i = 0; i < RollIterations; i++)
		{
			UpdateUI(rollItems, i, rollStartIndex);
			yield return new WaitForSeconds(VisualRollUpdateTime);
		}
		EndUpdateUI();
		Debug.Log("[ItemWildcardLogic] Decided on item '" + item.storageKey + "'");
		Activate(item);
		ResetUI();
	}

	private IEnumerator DenyLogicCoroutine()
	{
		SoundEventManager.Instance.Play2D(spinFailSound, 0f);
		base.GetComponent<Animation>().Play("Randomizer_DenySpin");
		yield return new WaitForSeconds(0f);
	}

	public void Trigger()
	{
		if (ShouldDisplayRandomizer())
		{
			bool flag = false;
			for (int i = 0; i < GetActiveSlotCount(); i++)
			{
				PowerupStates state = ShipManager.instance.powerupHolders[i].State;
				bool flag2 = state == PowerupStates.ready || state == PowerupStates.hidden;
				flag = flag || flag2;
			}
			if (RandomizerItem.consumablesHeld > 0 && flag)
			{
				StartCoroutine(TriggerLogicCoroutine());
			}
			else
			{
				StartCoroutine(DenyLogicCoroutine());
			}
		}
	}

	private void HandleTouchScreenGesturesOnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		if (fingerIndex != 0 || GameManager.gameState != GameManager.GameState.Playing || GameManager.Instance.IsPaused)
		{
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(fingerPos);
		RaycastHit[] array = Physics.RaycastAll(ray);
		RaycastHit[] array2 = array;
		foreach (RaycastHit raycastHit in array2)
		{
			if (raycastHit.collider == base.GetComponent<Collider>())
			{
				Trigger();
			}
		}
	}

	private void ShowSpawnEmitter(GameObject magicItem)
	{
		LineRenderer component = SpawnEffectEmitter.GetComponent<LineRenderer>();
		component.SetPosition(0, base.transform.position);
		component.SetPosition(1, magicItem.transform.position);
		SpawnEffectEmitter.GetComponent<Animation>().Play("WildcardSpawnEffectAnimation");
	}

	private void SpawnPowerup(PowerupData data)
	{
		GameObject gameObject = new GameObject("WildcardSpawnPoint");
		GameObject gameObject2 = UnityEngine.Object.Instantiate(MagicItemCollectable, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
		MoverSmoothPingPong component = gameObject2.GetComponent<MoverSmoothPingPong>();
		component.IsWaveStartUnified = true;
		Mover component2 = gameObject2.GetComponent<Mover>();
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		component2.direction = new Vector3(insideUnitCircle.x, insideUnitCircle.y);
		gameObject2.transform.parent = LevelManager.Instance.currentScreenRoot.transform;
		gameObject2.transform.localPosition = new Vector3(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f, 0f);
		gameObject2.GetComponent<Mover>().direction = Vector3.Normalize(gameObject.transform.localScale);
		MagicItemCollectable componentInChildren = gameObject2.GetComponentInChildren<MagicItemCollectable>();
		componentInChildren.SetMagicItem(data);
		ShowSpawnEmitter(gameObject2);
	}
}
