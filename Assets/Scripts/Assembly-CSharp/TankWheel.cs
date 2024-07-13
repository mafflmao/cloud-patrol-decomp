using UnityEngine;

public class TankWheel : MonoBehaviour
{
	private int startingHP;

	private Health myHealth;

	public Color firstColor = new Color(1f, 1f, 1f, 1f);

	public Color secondColor = new Color(1f, 0.5f, 0.5f, 1f);

	public Color lastColor = new Color(0.2f, 0.2f, 0.2f, 1f);

	public GameObject flame;

	private void Start()
	{
		myHealth = GetComponent<Health>();
		startingHP = myHealth.hitPoints;
		flame.SetActive(false);
	}

	private void Update()
	{
		if ((float)myHealth.hitPoints <= 0f)
		{
			base.GetComponent<Renderer>().material.SetColor("_Color", lastColor);
			base.gameObject.layer = 1;
			flame.SetActive(true);
		}
		else if ((float)myHealth.hitPoints < (float)startingHP / 2f)
		{
			base.GetComponent<Renderer>().material.SetColor("_Color", secondColor);
		}
		else
		{
			base.GetComponent<Renderer>().material.SetColor("_Color", firstColor);
		}
	}

	private void OnDisable()
	{
		if (flame != null)
		{
			flame.SetActive(false);
		}
	}
}
