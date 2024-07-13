using UnityEngine;

public class RewardFountain : MonoBehaviour
{
	private float coinSpawnRate = 0.08f;

	private int numReward = 30;

	public bool giveRewardCoins;

	private bool rewardAudioPlaying;

	private float lastCoinSpawnTime;

	private int numSpawned;

	public SoundEventData spawn_Reward_SFX;

	private void Update()
	{
		if (giveRewardCoins && numSpawned < numReward)
		{
			if (!rewardAudioPlaying)
			{
				SoundEventManager.Instance.Play(spawn_Reward_SFX, base.gameObject);
				rewardAudioPlaying = true;
			}
			if (lastCoinSpawnTime + coinSpawnRate < Time.time)
			{
				GameObject gameObject = Object.Instantiate(ShipManager.instance.moneyDrop, base.transform.position, Quaternion.identity) as GameObject;
				Loot component = gameObject.GetComponent<Loot>();
				component.autoCollect = true;
				lastCoinSpawnTime = Time.time;
				numSpawned++;
			}
		}
		else if (numSpawned != numReward)
		{
			rewardAudioPlaying = false;
		}
	}
}
