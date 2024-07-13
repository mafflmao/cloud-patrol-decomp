using UnityEngine;

public class GamecenterButton : MonoBehaviour
{
	public Texture2D AndroidButtonPage;

	private void Awake()
	{
	}

	private void LaunchAchievements()
	{
		SwrveEventsUI.GCAchievementButtonTouched();
	}

	private void LaunchLeaderboards()
	{
		SwrveEventsUI.GCLeaderboardButtonTouched();
	}
}
