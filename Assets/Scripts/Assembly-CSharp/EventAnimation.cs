using System;
using UnityEngine;

public class EventAnimation : MonoBehaviour
{
	[Serializable]
	public class SubAnimation
	{
		public Animation m_Animation;

		public AnimationClip[] m_AnimationClips;
	}

	public SubAnimation[] m_Animations;

	public GameObject[] m_GameObjects;

	public void PlayBySubID(int i_Index)
	{
		int num = i_Index / 10;
		int num2 = i_Index % 10;
		if (m_Animations.Length > 0 && num < m_Animations.Length && m_Animations[num].m_Animation != null && m_Animations[num].m_AnimationClips.Length > 0 && num2 < m_Animations[num].m_AnimationClips.Length)
		{
			m_Animations[num].m_Animation.Stop();
			m_Animations[num].m_Animation.Play(m_Animations[num].m_AnimationClips[num2]);
		}
	}

	private void AnimationEnd()
	{
		base.gameObject.SetActive(false);
	}
}
