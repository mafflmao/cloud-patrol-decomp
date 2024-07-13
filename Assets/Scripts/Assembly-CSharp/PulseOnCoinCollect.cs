using System;
using UnityEngine;

public class PulseOnCoinCollect : MonoBehaviour
{
	public float scaleToPulseTo = 1000f;

	public float time = 0.2f;

	private float _currentTime = -1f;

	private Vector3 _originalScale;

	private void Start()
	{
		GameManager.MoneyCollected += HandleGameManagerMoneyCollected;
		_originalScale = base.transform.localScale;
	}

	private void OnDestroy()
	{
		GameManager.MoneyCollected -= HandleGameManagerMoneyCollected;
	}

	private void HandleGameManagerMoneyCollected(object sender, EventArgs e)
	{
		base.transform.localScale = scaleToPulseTo * Vector3.one;
		_currentTime = time;
	}

	private void LateUpdate()
	{
		if (_currentTime >= 0f)
		{
			_currentTime -= Time.deltaTime;
			float t = Mathf.Clamp01(_currentTime / time);
			base.transform.localScale = Vector3.Slerp(_originalScale, scaleToPulseTo * Vector3.one, t);
		}
	}
}
