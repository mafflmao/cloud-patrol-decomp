using UnityEngine;

public class NavBar : MonoBehaviour
{
	public NavBarButton[] buttons;

	public void UpdateGraphics()
	{
		NavBarButton[] array = buttons;
		foreach (NavBarButton navBarButton in array)
		{
			navBarButton.UpdateGraphics();
		}
	}
}
