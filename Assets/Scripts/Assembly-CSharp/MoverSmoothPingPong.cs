using UnityEngine;

public class MoverSmoothPingPong : MonoBehaviour
{
	private float _frequencyFactor;

	private float _amplitudeFactor;

	private float _baseY;

	private float _startTime;

	public bool IsWaveStartUnified;

	private void Start()
	{
		if (IsWaveStartUnified)
		{
			_startTime = Time.time;
		}
		else
		{
			_startTime = 0f;
		}
		_amplitudeFactor = Random.Range(0.65f, 1.25f);
		_frequencyFactor = Random.Range(1f, 2f);
		_baseY = base.transform.position.y;
	}

	private void Update()
	{
		if (!GameManager.Instance.IsPaused)
		{
			float num = Mathf.Sin((Time.time - _startTime) * _frequencyFactor) * _amplitudeFactor;
			base.transform.position = new Vector3(base.transform.position.x, _baseY + num, base.transform.position.z);
		}
	}
}
