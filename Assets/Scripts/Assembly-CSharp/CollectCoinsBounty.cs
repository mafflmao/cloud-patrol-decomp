public class CollectCoinsBounty : Bounty
{
	private void OnEnable()
	{
		GameManager.MoneyCollected += HandleGameManagerMoneyCollected;
	}

	private void OnDisable()
	{
		GameManager.MoneyCollected -= HandleGameManagerMoneyCollected;
	}

	private void HandleGameManagerMoneyCollected(object sender, IntegerChangeEventArgs args)
	{
		TryIncrementProgress(args.Delta);
	}
}
