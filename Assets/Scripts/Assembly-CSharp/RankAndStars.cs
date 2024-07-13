public class RankAndStars
{
	public RankData Rank { get; set; }

	public int Stars { get; set; }

	public RankAndStars(RankData rank, int stars)
	{
		Rank = rank;
		Stars = stars;
	}

	public override string ToString()
	{
		return string.Format("[RankAndStars: Rank={0}, Stars={1}]", Rank, Stars);
	}
}
