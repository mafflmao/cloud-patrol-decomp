using UnityEngine;

public class FireworksSound : MonoBehaviour
{
	public void PlayFireworkSound()
	{
		SoundEventManager.Instance.Play2D(ResultsController.Instance.m_FireWorksSound);
	}
}
