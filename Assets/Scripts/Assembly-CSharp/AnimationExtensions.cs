using UnityEngine;

public static class AnimationExtensions
{
	public static void Play(this Animation i_Animation, AnimationClip i_AnimationClip)
	{
		if (i_Animation != null && i_AnimationClip != null)
		{
			i_Animation.Play(i_AnimationClip.name);
		}
	}

	public static void PlayQueued(this Animation i_Animation, AnimationClip i_AnimationClip)
	{
		if (i_Animation != null && i_AnimationClip != null)
		{
			i_Animation.PlayQueued(i_AnimationClip.name);
		}
	}

	public static void CrossFade(this Animation i_Animation, AnimationClip i_AnimationClip, float i_FadeLength)
	{
		if (i_Animation != null && i_AnimationClip != null)
		{
			i_Animation.CrossFade(i_AnimationClip.name, i_FadeLength);
		}
	}
}
