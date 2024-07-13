using System;
using System.Collections;
using UnityEngine;

public class AnimationUtils
{
	public static AnimationClip PlayClip(Animation anim, string clipName)
	{
		if (anim == null)
		{
			Debug.LogWarning("Animation is null. Can't play clip '" + clipName + "'");
			return null;
		}
		if (anim[clipName] == null)
		{
			Debug.LogWarning("Clip '" + clipName + "' does not exist in animation '" + anim.name + "'.");
			return null;
		}
		anim[clipName].speed = 1f;
		anim.Play(clipName);
		return anim[clipName].clip;
	}

	public static AnimationClip PlayClipBackwards(Animation anim, string clipName)
	{
		if (anim == null)
		{
			Debug.LogWarning("Animation is null. Can't play clip '" + clipName + "'");
			return null;
		}
		if (anim[clipName] == null)
		{
			Debug.LogWarning("Clip '" + clipName + "' does not exist in animation '" + anim.name + "'.");
			return null;
		}
		anim[clipName].speed = -1f;
		anim[clipName].time = anim[clipName].length;
		anim.Play(clipName);
		return anim[clipName].clip;
	}

	public static IEnumerator PlayIgnoringTimescale(Animation animation, string clipName, Action onComplete)
	{
		AnimationState _currState = animation[clipName];
		bool isPlaying = true;
		float _progressTime = 0f;
		float _timeAtLastFrame2 = 0f;
		float _timeAtCurrentFrame2 = 0f;
		float deltaTime2 = 0f;
		animation.Play(clipName);
		_timeAtLastFrame2 = Time.realtimeSinceStartup;
		while (isPlaying)
		{
			_timeAtCurrentFrame2 = Time.realtimeSinceStartup;
			deltaTime2 = _timeAtCurrentFrame2 - _timeAtLastFrame2;
			_timeAtLastFrame2 = _timeAtCurrentFrame2;
			_progressTime += deltaTime2;
			_currState.normalizedTime = _progressTime / _currState.length;
			animation.Sample();
			if (_progressTime >= _currState.length)
			{
				if (_currState.wrapMode != WrapMode.Loop)
				{
					isPlaying = false;
				}
				else
				{
					_progressTime = 0f;
				}
			}
			yield return new WaitForEndOfFrame();
		}
		yield return null;
		if (onComplete != null)
		{
			Debug.Log("Start onComplete");
			onComplete();
		}
	}
}
