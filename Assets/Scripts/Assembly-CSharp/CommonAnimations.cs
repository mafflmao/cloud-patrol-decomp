using UnityEngine;

public class CommonAnimations : MonoBehaviour
{
	public static void AnimateButton(GameObject button)
	{
		RestoreMaterial restoreMaterial = button.AddComponent<RestoreMaterial>();
		restoreMaterial.mat = button.GetComponent<Renderer>().sharedMaterial;
		iTween.ColorFrom(button, iTween.Hash("color", Color.white, "time", 0.3f, "easetype", iTween.EaseType.easeInBounce));
		iTween.PunchScale(button, iTween.Hash("amount", new Vector3(0.15f, 0.15f, 0f), "time", 0.5f));
	}

	public static void AnimateButtonRestore(GameObject button)
	{
		RestoreMaterial restoreMaterial = button.AddComponent<RestoreMaterial>();
		restoreMaterial.mat = button.GetComponent<Renderer>().sharedMaterial;
		iTween.ColorFrom(button, iTween.Hash("color", Color.white, "time", 0.3f, "easetype", iTween.EaseType.easeInBounce, "oncompletetarget", button, "oncomplete", "RestoreMaterialTweenComplete"));
		iTween.PunchScale(button, iTween.Hash("amount", new Vector3(0.15f, 0.15f, 0f), "time", 0.5f));
	}

	public static void AnimateButtonElement(GameObject button)
	{
		RestoreMaterial restoreMaterial = button.AddComponent<RestoreMaterial>();
		restoreMaterial.mat = button.GetComponent<Renderer>().sharedMaterial;
		iTween.ColorFrom(button, iTween.Hash("color", new Color(0.78f, 0.78f, 0.78f, 1f), "time", 0.3f, "easetype", iTween.EaseType.easeInBounce, "oncompletetarget", button, "oncomplete", "RestoreMaterialTweenComplete"));
		iTween.PunchScale(button, iTween.Hash("amount", new Vector3(0.2f, 0.2f, 0f), "time", 0.5f));
	}
}
