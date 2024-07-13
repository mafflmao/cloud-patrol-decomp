using UnityEngine;

public class UIBackgroundLoader : MonoBehaviour
{
	public bool destroyBackground;

	private GameObject background;

	private void Awake()
	{
		background = GameObject.Find("UI Background");
		if (destroyBackground)
		{
			Object.Destroy(background);
		}
		else
		{
			LoadBackground();
		}
	}

	public void LoadBackground()
	{
		if (background == null)
		{
			background = (GameObject)Object.Instantiate(Resources.Load("UI Prefabs/Common/ShipBackground"));
			background.name = "UI Background";
		}
	}

	public void DestroyBackground()
	{
		Object.Destroy(background);
	}

	public void StartAnimation()
	{
		background.GetComponentInChildren<Animation>().Play("Title_Screen_Skylands");
	}
}
