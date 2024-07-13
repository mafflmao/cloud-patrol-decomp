public class CoinStoreItemData
{
	public enum Graphic
	{
		Small = 0,
		Medium = 1,
		Large = 2,
		XLarge = 3
	}

	public int coins;

	public int gemCost;

	public Graphic graphic;

	public string saleText = "0";

	public bool IsOnSale
	{
		get
		{
			return saleText != "0";
		}
	}

	public string SaleText
	{
		get
		{
			return saleText;
		}
	}

	public override string ToString()
	{
		return string.Format("[CoinStoreItemData:coins={0},gemCost={1},graphic={2},saleText={3}]", coins, gemCost, graphic, saleText);
	}
}
