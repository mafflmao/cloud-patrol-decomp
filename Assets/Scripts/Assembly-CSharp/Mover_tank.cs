using System.Collections.Generic;
using UnityEngine;

public class Mover_tank : MonoBehaviour
{
	public Transform endOfLevel;

	private bool isEnded;

	private void Start()
	{
	}

	private void Update()
	{
		List<TankWheel> list = new List<TankWheel>();
		list.AddRange(GetComponentsInChildren<TankWheel>(true));
		for (int num = list.Count - 1; num >= 0; num--)
		{
			TankWheel tankWheel = list[num];
			Health component = tankWheel.gameObject.GetComponent<Health>();
			if (component.hitPoints <= 0)
			{
				list.Remove(tankWheel);
			}
		}
		if (list.Count == 0 && !isEnded)
		{
			EndLevel();
			isEnded = true;
		}
	}

	private void EndLevel()
	{
		StopParticles();
		StopAnimation();
		StopAudio();
		KillEnemies();
	}

	private void StopParticles()
	{

	}

	private void StopAnimation()
	{
		List<Animation> list = new List<Animation>(GetComponentsInChildren<Animation>());
		foreach (Animation item in list)
		{
			item.Stop();
		}
	}

	private void StopAudio()
	{
		GameObject gameObject = GameObject.Find("Enemy_GoblinTank/Audio");
		if (gameObject != null)
		{
			gameObject.GetComponent<AudioSource>().Stop();
		}
	}

	private void PlayFXWithSound(GameObject _flipbook, AudioClip _audio)
	{
		if (_flipbook != null)
		{
			Flipbook component = _flipbook.GetComponent<Flipbook>();
			component.Play();
		}
	}

	private void KillEnemies()
	{
		List<Enemy> list = new List<Enemy>(GetComponentsInChildren<Enemy>());
		foreach (Enemy item in list)
		{
			item.SendMessage("Kill");
		}
	}
}
