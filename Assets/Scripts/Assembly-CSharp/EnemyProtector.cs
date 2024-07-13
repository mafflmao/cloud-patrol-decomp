using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyProtector : SafeMonoBehaviour
{
	public TrollProtectorBeam linePrefab;

	public GameObject forceFieldPrefab;

	public List<Health> targetList;

	public GameObject deathFXPrefab;

	public Transform beamStartingPoint;

	private List<Health> _protectees;

	private List<GameObject> _forceFields;

	private List<TrollProtectorBeam> _lines;

	private Health _myHealth;

	private AnimationStates _anim;

	public SoundEventData sfxBeamStart;

	public SoundEventData sfxBeamEnd;

	private void OnEnable()
	{
		_myHealth = GetComponent<Health>();
		_anim = GetComponent<AnimationStates>();
		_protectees = new List<Health>();
		_forceFields = new List<GameObject>();
		_lines = new List<TrollProtectorBeam>();
		Health.Killed += OnKilled;
		_anim.PlayAnim("idle");
		_anim.speed = 1f;
		InvokeHelper.InvokeSafe(TurnOnForcefield, 0.25f, this);
	}

	private void OnDisable()
	{
		Health.Killed -= OnKilled;
		DisarmForcefield();
	}

	private void TurnOnForcefield()
	{
		if (targetList.Any())
		{
			_protectees.AddRange(targetList);
		}
		if (!_protectees.Any())
		{
			GameObject[] array = EnemyUtils.GetEnemies(LevelManager.Instance.currentScreenRoot).ToArray();
			if (array.Any())
			{
				_protectees.Add(array[UnityEngine.Random.Range(0, array.Length)].GetComponent<Health>());
			}
		}
		if (!_protectees.Any())
		{
			return;
		}
		if (sfxBeamStart != null)
		{
			SoundEventManager.Instance.Play(sfxBeamStart, base.gameObject);
		}
		foreach (Health protectee in _protectees)
		{
			if (protectee != null)
			{
				TrollProtectorBeam trollProtectorBeam = UnityEngine.Object.Instantiate(linePrefab, base.GetComponent<Collider>().bounds.center, Quaternion.identity) as TrollProtectorBeam;
				trollProtectorBeam.transform.parent = beamStartingPoint;
				trollProtectorBeam.transform.localPosition = Vector3.zero;
				trollProtectorBeam.target = protectee.gameObject;
				trollProtectorBeam.targetColliderCenter = true;
				trollProtectorBeam.GetComponent<Renderer>().enabled = true;
				_lines.Add(trollProtectorBeam);
				StartCoroutine(CreateForceField(trollProtectorBeam, protectee));
			}
		}
	}

	private IEnumerator CreateForceField(TrollProtectorBeam line, Health protectee)
	{
		while (!line.hasArrivedAtTarget)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (protectee.isDead)
		{
			UnityEngine.Object.Destroy(line.gameObject);
		}
		else if (forceFieldPrefab != null)
		{
			GameObject forceField = (GameObject)UnityEngine.Object.Instantiate(forceFieldPrefab);
			forceField.transform.position = protectee.GetComponent<Collider>().bounds.center;
			forceField.transform.parent = protectee.transform;
			if (forceField.GetComponent<ForceField>() != null)
			{
				forceField.GetComponent<ForceField>().owner = protectee;
				forceField.GetComponent<ForceField>().beam = line;
			}
			_forceFields.Add(forceField);
			protectee.isForceFielded = true;
			ToggleForceFieldOnHat(protectee, true);
		}
	}

	private void OnKilled(object sender, EventArgs args)
	{
		if ((Health)sender == _myHealth)
		{
			_anim.speed = 1f;
			DisarmForcefield();
			UnityEngine.Object.Instantiate(deathFXPrefab, base.GetComponent<Collider>().bounds.center, Quaternion.identity);
			GameManager.CameraShake();
			if (sfxBeamEnd != null)
			{
				SoundEventManager.Instance.Play(sfxBeamEnd, base.gameObject);
			}
		}
	}

	private void DisarmForcefield()
	{
		foreach (TrollProtectorBeam line in _lines)
		{
			if (line != null)
			{
				UnityEngine.Object.Destroy(line.gameObject);
			}
		}
		_lines.Clear();
		foreach (Health protectee in _protectees)
		{
			if (protectee != null)
			{
				protectee.isForceFielded = false;
				ToggleForceFieldOnHat(protectee, false);
			}
		}
		foreach (GameObject forceField in _forceFields)
		{
			if (forceField != null)
			{
				forceField.GetComponent<ForceField>().Remove();
			}
		}
		_forceFields.Clear();
	}

	private void ToggleForceFieldOnHat(Health protectee, bool fieldOn)
	{
		Hat componentInChildren = protectee.GetComponentInChildren<Hat>();
		if (componentInChildren != null)
		{
			componentInChildren.GetComponent<Health>().isForceFielded = fieldOn;
		}
	}
}
