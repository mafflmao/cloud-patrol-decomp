using UnityEngine;

public class PresentBoxManager : SingletonMonoBehaviour
{
	public float chanceToSpawnPresent = 0.16f;

	public float chanceToSpawnGem = 0.45f;

	public float chanceToSpawnMagicItem = 0.1f;

	private float _percentOfMagicItemsLocked;

	public GameObject presentBoxPrefab;

	private GameObject _presentBoxInstance;

	public static PresentBoxManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<PresentBoxManager>();
		}
	}

	public int NumCoins
	{
		get
		{
			return Random.Range(SwrveEconomy.GetMinPresentCoins(), SwrveEconomy.GetMaxPresentCoins());
		}
	}

	public int NumGems
	{
		get
		{
			return Random.Range(SwrveEconomy.GetMinPresentGems(), SwrveEconomy.GetMaxPresentGems() + 1);
		}
	}

	private void OnEnable()
	{
		LevelManager.ArrivedAtNextRoom += HandleLevelManagerArrivedAtNextRoom;
		chanceToSpawnPresent = Bedrock.GetRemoteVariableAsFloat("PresentBoxSpawnChance", chanceToSpawnPresent);
		chanceToSpawnGem = Bedrock.GetRemoteVariableAsFloat("PresentBoxChanceToSpawnGem", chanceToSpawnGem);
		chanceToSpawnMagicItem = Bedrock.GetRemoteVariableAsFloat("PresentBoxChanceToSpawnMagicItem", chanceToSpawnMagicItem);
		InvokeHelper.InvokeSafe(SetChanceToSpawn, 0.25f, this);
	}

	private void SetChanceToSpawn()
	{
		float num = MagicItemManager.Instance.NumberOfLockedMagicItems;
		float num2 = MagicItemManager.Instance.NumberOfUnlockedMagicItems;
		_percentOfMagicItemsLocked = num / (num2 + num);
		Debug.Log("Percent of magic items unlocked: " + _percentOfMagicItemsLocked);
		chanceToSpawnMagicItem *= _percentOfMagicItemsLocked;
	}

	private void OnDisable()
	{
		LevelManager.ArrivedAtNextRoom -= HandleLevelManagerArrivedAtNextRoom;
		if (_presentBoxInstance != null)
		{
			Object.Destroy(_presentBoxInstance);
		}
	}

	private void HandleLevelManagerArrivedAtNextRoom(object sender, LevelManager.NextRoomEventArgs e)
	{
		if (RankDataManager.Instance.CurrentRank.Rank.RankNumber > 0 && LevelManager.Instance.FinishedTutorials && (DebugSettingsUI.forcePresentSpawn || Random.value < chanceToSpawnPresent))
		{
			SpawnPresent();
		}
	}

	public void SpawnPresent()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("PresentBoxSpawnPoint");
		if (array.Length > 0 && (bool)presentBoxPrefab)
		{
			Transform transform = array[Random.Range(0, array.Length)].transform;
			_presentBoxInstance = Object.Instantiate(presentBoxPrefab, transform.position, transform.rotation) as GameObject;
		}
	}
}
