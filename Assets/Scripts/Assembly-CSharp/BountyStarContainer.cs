using UnityEngine;

public class BountyStarContainer : MonoBehaviour
{
	public float Width = 500f;

	public GameObject starPrefab;

	public Vector2 starPrefabDimensions = new Vector2(72f, 72f);

	private BountyStar[] _stars;

	public void SetNumberOfStars(int stars)
	{
		if (_stars != null)
		{
			BountyStar[] stars2 = _stars;
			foreach (BountyStar bountyStar in stars2)
			{
				if (bountyStar != null)
				{
					Object.Destroy(bountyStar.gameObject);
				}
			}
		}
		_stars = new BountyStar[stars];
		float num = Width / (float)(stars + 1);
		float num2 = base.transform.position.x - Width / 2f;
		for (int j = 0; j < stars; j++)
		{
			float x = num2 + (float)(j + 1) * num;
			float z = base.transform.position.z - (float)(1 + (stars - j));
			Vector3 position = new Vector3(x, base.transform.position.y, z);
			GameObject gameObject = (GameObject)Object.Instantiate(starPrefab, position, Quaternion.identity);
			gameObject.transform.parent = base.transform;
			_stars[j] = gameObject.GetComponent<BountyStar>();
		}
	}

	public BountyStar GetStar(int i)
	{
		return _stars[i];
	}

	public void SetStarsEnabled(int enabledCount)
	{
		int num = 0;
		BountyStar[] stars = _stars;
		foreach (BountyStar bountyStar in stars)
		{
			bountyStar.IsEnabled = enabledCount > num;
			num++;
		}
	}

	public void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(base.transform.position, new Vector3(Width, starPrefabDimensions.y, 0f));
		Vector3 size = new Vector3(starPrefabDimensions.x, starPrefabDimensions.y, 0f);
		if (_stars == null)
		{
			return;
		}
		BountyStar[] stars = _stars;
		foreach (BountyStar bountyStar in stars)
		{
			if (bountyStar != null)
			{
				Gizmos.DrawWireCube(bountyStar.transform.position, size);
			}
		}
	}
}
