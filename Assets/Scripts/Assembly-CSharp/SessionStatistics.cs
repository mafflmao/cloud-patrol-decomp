using UnityEngine;

public class SessionStatistics
{
	public class DeathTypes
	{
		public const string None = "None";

		public const string Projectile = "Projectile";

		public const string Hazard = "Hazard";
	}

	public int singleCoinsSpawned;

	public int singleCoinsCollected;

	public int combo2CoinsSpawned;

	public int combo2CoinsCollected;

	public int combo3CoinsSpawned;

	public int combo3CoinsCollected;

	public int combo4CoinsSpawned;

	public int combo4CoinsCollected;

	public int combo5CoinsSpawned;

	public int combo5CoinsCollected;

	public int combo6CoinsSpawned;

	public int combo6CoinsCollected;

	public int totalAreaSkipsUsed;

	public int totalElixirsUsed;

	public string deathType = "None";

	public Vector2 deathScreenLocation = Vector3.zero;

	public string deathAI = "None";

	public void IncrementComboCoinSpawn(int comboNumber)
	{
		switch (comboNumber)
		{
		case 2:
			combo2CoinsSpawned++;
			break;
		case 3:
			combo3CoinsSpawned++;
			break;
		case 4:
			combo4CoinsSpawned++;
			break;
		case 5:
			combo5CoinsSpawned++;
			break;
		case 6:
			combo6CoinsSpawned++;
			break;
		}
	}

	public void IncrementComboCoinCollect(int comboNumber)
	{
		switch (comboNumber)
		{
		case 2:
			combo2CoinsCollected++;
			break;
		case 3:
			combo3CoinsCollected++;
			break;
		case 4:
			combo4CoinsCollected++;
			break;
		case 5:
			combo5CoinsCollected++;
			break;
		case 6:
			combo6CoinsCollected++;
			break;
		}
	}
}
