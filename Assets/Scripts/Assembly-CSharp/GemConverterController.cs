public class GemConverterController : StateController
{
	public CoinStoreButton[] coinStoreButtons;

	protected override void ShowState()
	{
		UpdateCoinAmounts();
		base.ShowState();
	}

	private void UpdateCoinAmounts()
	{
		coinStoreButtons[0].Data = SwrveEconomy.GetCoinStoreItemData(SwrveEconomy.CoinPack.Pack1);
		coinStoreButtons[1].Data = SwrveEconomy.GetCoinStoreItemData(SwrveEconomy.CoinPack.Pack2);
		coinStoreButtons[2].Data = SwrveEconomy.GetCoinStoreItemData(SwrveEconomy.CoinPack.Pack3);
		coinStoreButtons[3].Data = SwrveEconomy.GetCoinStoreItemData(SwrveEconomy.CoinPack.Pack4);
	}
}
