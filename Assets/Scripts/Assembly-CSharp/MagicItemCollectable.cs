using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MagicItemCollectable : MonoBehaviour
{
	private const float TimeToCollect = 0.3f;

	public GameObject touchVFX;

	public SoundEventData touchSFX;

	public SoundEventData activeLoop;

	public bool changeAfterSpawn;

	public Mover horizontalMover;

	public MoverSmoothPingPong verticalMover;

	private PowerupData _selecteditem;

	private bool _collected;

	private bool _isMoving;

	private Vector3 _endPosition = Vector3.zero;

	private Vector3 _startPosition = Vector3.zero;

	private float _currentTime;

	public static event EventHandler<PowerupEventArgs> PowerupCollected;

	private void OnEnable()
	{
		if (changeAfterSpawn)
		{
			StartCoroutine("ChangeMagicItem");
		}
		if ((bool)activeLoop)
		{
			SoundEventManager.Instance.Play(activeLoop, base.gameObject);
		}
	}

	private void OnDisable()
	{
		iTween.Stop(base.gameObject);
		StopAllCoroutines();
		if ((bool)activeLoop)
		{
			SoundEventManager.Instance.Stop(activeLoop, base.gameObject);
		}
		if (!_collected)
		{
			SwrveEventsGameplay.MagicItemMissed(_selecteditem.readableName);
		}
	}

	private IEnumerator ChangeColor()
	{
		while (base.gameObject != null && !_collected && changeAfterSpawn && !DebugSettingsUI.preventChangeAfterSpawn)
		{
			iTween.ColorTo(base.gameObject, iTween.Hash("time", 0.1f, "color", RandomColor()));
			yield return new WaitForSeconds(0.2f);
		}
	}

	private IEnumerator ChangeMagicItem()
	{
		while (base.gameObject != null && !_collected && changeAfterSpawn && !DebugSettingsUI.preventChangeAfterSpawn)
		{
			SetMagicItem(MagicItemManager.Instance.ChooseMagicItem());
			yield return new WaitForSeconds(MagicItemManager.Instance.magicItemChangeTime);
		}
	}

	private Color RandomColor()
	{
		return new Color(UnityEngine.Random.value * 0.6f + 0.4f, UnityEngine.Random.value * 0.6f + 0.4f, UnityEngine.Random.value * 0.6f + 0.4f, 1f);
	}

	public void SetMagicItem(PowerupData powerupData)
	{
		_selecteditem = powerupData;
		base.GetComponent<Renderer>().material.mainTexture = _selecteditem.inGameButtonTexture;
	}

	private void LateUpdate()
	{
		if (_isMoving && !GameManager.Instance.IsPaused && !HealingElixirScreen.IsActive && !GameManager.Instance.IsGameOver)
		{
			if (_currentTime <= 0.3f)
			{
				_currentTime += Time.deltaTime;
				float t = Mathf.Clamp01(_currentTime / 0.3f);
				base.transform.parent.localPosition = Vector3.Lerp(_startPosition, _endPosition, t);
			}
			else
			{
				_isMoving = false;
				_currentTime = 0f;
			}
		}
	}

	public void Collect()
	{
		if (_selecteditem.PowerupPrefab != null && !_collected)
		{
			_collected = true;
			PowerupHolder availablePowerupHolder = ShipManager.instance.GetAvailablePowerupHolder();
			Transform transform = availablePowerupHolder.transform;
			base.transform.parent.parent = transform;
			_startPosition = base.transform.parent.localPosition;
			_isMoving = true;
			iTween.ScaleTo(base.transform.parent.gameObject, iTween.Hash("scale", transform.localScale * 0.5f, "time", 0.3f, "isLocal", true));
			if ((bool)touchSFX)
			{
				SoundEventManager.Instance.Play(touchSFX, base.gameObject);
			}
			StopMoving();
			StartCoroutine(DelayedCollect(availablePowerupHolder));
			SwrveEventsGameplay.MagicItemCollected(_selecteditem.readableName);
			OnPowerupCollected(_selecteditem);
		}
	}

	public void StopMoving()
	{
		horizontalMover.enabled = false;
		verticalMover.enabled = false;
	}

	private IEnumerator DelayedCollect(PowerupHolder destinationPowerupHolder)
	{
		yield return new WaitForSeconds(0.3f);
		if (_selecteditem.PowerupPrefab != null && destinationPowerupHolder != null)
		{
			destinationPowerupHolder.QueuePowerup(_selecteditem, false);
			destinationPowerupHolder.ActivatePowerup();
		}
		UnityEngine.Object.Destroy(base.transform.parent.gameObject);
	}

	protected void OnPowerupCollected(PowerupData data)
	{
		if (MagicItemCollectable.PowerupCollected != null)
		{
			MagicItemCollectable.PowerupCollected(this, new PowerupEventArgs(data));
		}
	}
}
