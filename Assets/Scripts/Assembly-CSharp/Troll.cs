using System.Collections;
using UnityEngine;

public class Troll : PeekabooTroll
{
	private const float TaunterPercent = 0.75f;

	public float initialWaitTime = 2.5f;

	public SoundEventData sfxTrollTauntDanceVO;

	public SoundEventData sfxTrollTauntVO;

	public SoundEventData sfxTrollLaughVO;

	public SoundEventData sfxJetpack;

	public bool hasJetpack;

	private float timer;

	private bool _isTaunter;

	protected override void Start()
	{
		base.Start();
		if (hasJetpack)
		{
			_animationStates.Jetpack_Idle_NoGun();
			SoundEventManager.Instance.Play(sfxJetpack, base.gameObject);
		}
		else
		{
			_animationStates.Idle();
			_animationStates.Offset(Random.Range(0f, 1f));
		}
		_isTaunter = !hasJetpack && Random.value <= 0.75f;
	}

	public override void StartTrollBehaviour()
	{
		StartCoroutine(IdleSequenceCoroutine());
		StartCoroutine(PeekabooCoroutine());
	}

	private IEnumerator PeekabooCoroutine()
	{
		if (!peekABoo)
		{
			yield break;
		}
		bool isFirstTime = true;
		while (true)
		{
			float timeToWait = ((!isFirstTime) ? 2.5f : initialWaitTime);
			yield return new WaitForSeconds(timeToWait);
			yield return StartCoroutine(PeekabooMoveUp());
			yield return new WaitForSeconds(2.5f);
			yield return StartCoroutine(PeekabooMoveDown());
			isFirstTime = false;
		}
	}

	private IEnumerator IdleSequenceCoroutine()
	{
		if (hasJetpack)
		{
			_animationStates.Jetpack_Idle_NoGun();
			yield break;
		}
		while (true)
		{
			_animationStates.Idle();
			float waitTime = Random.Range(2f, 5f);
			yield return new WaitForSeconds(waitTime);
			if (_isTaunter)
			{
				_animationStates.Taunt();
				SoundEventManager.Instance.Play(sfxTrollTauntDanceVO, base.gameObject);
				yield return new WaitForSeconds(_animationStates.anim.clip.length);
			}
			else
			{
				_animationStates.Jump();
				yield return new WaitForSeconds(_animationStates.anim.clip.length);
			}
		}
	}

	protected override IEnumerator VictoryDanceCoroutine()
	{
		SoundEventManager.Instance.Stop(sfxTrollTauntDanceVO, base.gameObject);
		int rand = Random.Range(1, 4);
		if (rand <= 2 && !hasJetpack)
		{
			yield return new WaitForSeconds(Random.Range(0.75f, 1.25f));
			switch (rand)
			{
			case 1:
				StartCoroutine(_animationStates.VictoryToIdle());
				SoundEventManager.Instance.Play(sfxTrollLaughVO, base.gameObject);
				break;
			case 2:
				StartCoroutine(_animationStates.VictorySpinToIdle());
				SoundEventManager.Instance.Play(sfxTrollLaughVO, base.gameObject);
				break;
			}
			yield return new WaitForSeconds(_animationStates.anim.clip.length);
			if (!GameManager.Instance.IsGameOver && !HealingElixirScreen.IsActive)
			{
				StartCoroutine(IdleSequenceCoroutine());
			}
		}
	}
}
