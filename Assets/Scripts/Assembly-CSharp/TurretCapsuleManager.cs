using System;
using System.Collections.Generic;
using UnityEngine;

public class TurretCapsuleManager : MonoBehaviour
{
	public TurretCapsule capsulePrefab;

	private Vector3 basePosition;

	public List<TurretCapsule> slotList;

	public float capsuleLengthHack = 0.01f;

	public float loadDuration = 0.5f;

	public float returnDuration = 1.5f;

	public float capsuleSpawnInterval = 0.5f;

	public Transform capsuleSpawnPosition;

	public int permanentCapsuleCount = 6;

	public float insidePositionX;

	public Transform capsuleIntermediatePoint;

	public Transform capsuleEjectPoint;

	public GameObject capsuleEjected;

	public SoundEventData capsuleSpawnSFX;

	public SoundEventData capsuleSpawnMoveASFX;

	public SoundEventData capsuleSpawnMoveBSFX;

	public SoundEventData capsuleAttachSFX;

	private int _lastCount;

	public void Start()
	{
		basePosition = base.transform.localPosition;
		insidePositionX = base.transform.localPosition.x - (float)permanentCapsuleCount * capsuleLengthHack;
		base.transform.localPosition = new Vector3(insidePositionX, base.transform.localPosition.y, base.transform.localPosition.z);
		TargetQueue.TargetAdded += Load;
		TargetQueue.TargetRemoved += Unload;
		for (int i = 0; i < permanentCapsuleCount; i++)
		{
			AddSlotStatic();
		}
		DeactivateAllCapsules();
		slotList[5].Show();
	}

	public void OnDisable()
	{
		TargetQueue.TargetAdded -= Load;
		TargetQueue.TargetRemoved -= Unload;
	}

	private void Load()
	{
		int count = ShipManager.instance.shooter[0].targetQueue.Count;
		if (count == slotList.Count)
		{
			foreach (TurretCapsule slot in slotList)
			{
				slot.MaxOut();
			}
		}
		else
		{
			foreach (TurretCapsule slot2 in slotList)
			{
				slot2.UnMaxOut();
			}
		}
		float x = insidePositionX + (float)ShipManager.instance.shooter[0].targetQueue.Count * capsuleLengthHack + 0.01f;
		Vector3 vector = new Vector3(x, basePosition.y, basePosition.z);
		iTween.MoveTo(base.gameObject, iTween.Hash("position", vector, "time", loadDuration, "islocal", true));
		slotList[permanentCapsuleCount - count].Show();
	}

	private void Unload()
	{
		float x = insidePositionX + (float)ShipManager.instance.shooter[0].targetQueue.Count * capsuleLengthHack + 0.01f;
		Vector3 vector = new Vector3(x, basePosition.y, basePosition.z);
		iTween.MoveTo(base.gameObject, iTween.Hash("position", vector, "time", loadDuration, "islocal", true));
		if (ShipManager.instance.shooter[0].targetQueue.Count < 3)
		{
			slotList[0].Hide();
			slotList[1].Hide();
			slotList[2].Hide();
		}
	}

	private void Load(object sender, EventArgs args)
	{
		Load();
	}

	private void Unload(object sender, EventArgs args)
	{
		Unload();
	}

	private void DeactivateAllCapsules(object sender, EventArgs args)
	{
		DeactivateAllCapsules();
	}

	public void DeactivateAllCapsules()
	{
		foreach (TurretCapsule slot in slotList)
		{
			slot.Hide();
		}
	}

	private void AddSlotStatic()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(capsulePrefab.gameObject, capsuleSpawnPosition.position, Quaternion.identity) as GameObject;
		TurretCapsule component = gameObject.GetComponent<TurretCapsule>();
		slotList.Add(component);
		gameObject.transform.parent = base.transform;
		Vector3 localPosition = new Vector3(0f - capsuleLengthHack * (float)(slotList.Count - 1), 0f, 0f);
		gameObject.transform.localPosition = localPosition;
	}

	public void SetCount(int num)
	{
		_lastCount = num;
	}

	public void Fire(int num)
	{
		float x = basePosition.x - (float)(permanentCapsuleCount - _lastCount + num) * capsuleLengthHack + 0.01f;
		Vector3 vector = new Vector3(x, basePosition.y, basePosition.z);
		iTween.MoveTo(base.gameObject, iTween.Hash("position", vector, "time", loadDuration, "islocal", true));
		InvokeHelper.InvokeSafe(EjectCapsule, 0.2f, this);
		int num2 = permanentCapsuleCount - _lastCount + (num - 3);
		if (num2 < slotList.Count && num2 > 0)
		{
			slotList[num2].Hide();
		}
	}

	public void EjectCapsule()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(capsuleEjected, capsuleEjectPoint.position, capsuleEjectPoint.rotation) as GameObject;
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
		rigidbody.AddForce(new Vector3(UnityEngine.Random.value * 5f, UnityEngine.Random.value * 60f + 50f, 0f));
		rigidbody.AddTorque(UnityEngine.Random.insideUnitSphere * 225f);
		UnityEngine.Object.Destroy(gameObject, 1f);
	}
}
