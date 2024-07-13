using System.Collections.Generic;
using UnityEngine;

public class FooterStarContainer : MonoBehaviour
{
	public PackedSprite starPrefab;

	public int starSpacing;

	private List<PackedSprite> stars = new List<PackedSprite>();

	private RankAndStars currentRank;

	private void Awake()
	{
		UpdateData();
	}

	private void Start()
	{
		UpdateData();
		UpdateGraphics();
	}

	public void UpdateData()
	{
		currentRank = RankDataManager.Instance.CurrentRank;
	}

	public void UpdateGraphics()
	{
		foreach (PackedSprite star in stars)
		{
			Object.Destroy(star);
		}
		stars.Clear();
		for (int i = 0; i < currentRank.Rank.StarsForNextRank; i++)
		{
			PackedSprite packedSprite = (PackedSprite)Object.Instantiate(starPrefab);
			stars.Add(packedSprite);
			if (i < currentRank.Stars)
			{
				packedSprite.PlayAnim("Active");
			}
			else
			{
				packedSprite.PlayAnim("Inactive");
			}
		}
		RepositionStars();
	}

	public void RepositionStars()
	{
		for (int i = 0; i < stars.Count; i++)
		{
			stars[i].transform.parent = base.transform;
			stars[i].transform.localPosition = new Vector3(starSpacing * i, 0f, -1f);
		}
	}
}
