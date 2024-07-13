using UnityEngine;

public class AnimationRandomStart : MonoBehaviour
{
	public float m_MinSpeed = 1f;

	public float m_MaxSpeed = 1f;

	public bool m_RandomFrameStart = true;

	public bool m_PlayAutomaticaly;

	public AnimationClip[] m_Animations;

	private void Awake()
	{
		if (base.GetComponent<Animation>().isPlaying)
		{
			base.GetComponent<Animation>().Stop();
			base.GetComponent<Animation>().Rewind();
		}
		if (m_Animations.Length <= 1)
		{
			float time = Random.Range(0f, base.GetComponent<Animation>().clip.length);
			float speed = Random.Range(m_MinSpeed, m_MaxSpeed);
			base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name].speed = speed;
			if (m_RandomFrameStart)
			{
				base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name].time = time;
			}
		}
		else
		{
			AnimationClip[] animations = m_Animations;
			foreach (AnimationClip animationClip in animations)
			{
				float time = Random.Range(0f, animationClip.length);
				float speed = Random.Range(m_MinSpeed, m_MaxSpeed);
				base.GetComponent<Animation>()[animationClip.name].speed = speed;
				if (m_RandomFrameStart)
				{
					base.GetComponent<Animation>()[animationClip.name].time = time;
				}
			}
		}
		if (m_PlayAutomaticaly)
		{
			base.GetComponent<Animation>().Play();
		}
	}
}
