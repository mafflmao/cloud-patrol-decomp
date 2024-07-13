using UnityEngine;

public class Instantiate_Debug : MonoBehaviour
{
	public GameObject noRoot;

	public GameObject root;

	private GameObject _root;

	private void Start()
	{
		_root = (GameObject)Object.Instantiate(root);
		AnimationUtils.PlayClip(_root.GetComponent<Animation>(), "Clouds_Intro");
		Object.Instantiate(noRoot);
	}
}
