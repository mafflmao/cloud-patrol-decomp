using UnityEngine;

public class LobGrenade : MonoBehaviour
{
	public float fireFreqRangeLow = 3f;

	public float fireFreqRangeHi = 5f;

	public Transform projectile;

	private bool thrownYet;

	private void Start()
	{
	}

	private void Update()
	{
		if ((double)base.transform.position.x <= (double)Camera.main.transform.position.x - 1.0 && !thrownYet)
		{
			int num = Random.Range(0, 2);
			Debug.Log(num + " random");
			if (num == 0)
			{
				Object.Instantiate(projectile, base.transform.position, Quaternion.identity);
			}
			thrownYet = true;
		}
	}
}
