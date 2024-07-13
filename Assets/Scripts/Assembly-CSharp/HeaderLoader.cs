using UnityEngine;

public class HeaderLoader : MonoBehaviour
{
	private void Start()
	{
		Application.LoadLevelAdditiveAsync("Header");
	}
}
