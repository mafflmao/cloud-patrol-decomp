using UnityEngine;

public class QAButton : MonoBehaviour
{
	public const bool SHOW_QA_MENU_ON_TITLE = false;

	private void ShowQaMenu()
	{
		DebugSettingsUI.GetOrCreateInstance();
	}
}
