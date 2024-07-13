using UnityEngine;

public class Balloon : MonoBehaviour
{
	private float _liftAmount = 0.5f;

	private float _initialTimeOffset;

	private float _frequencyFactor;

	private float _amplitudeFactor;

	private float _swayAmplitude = 5f;

	private float _rotationRate;

	private float _yRotation;

	private float? _spawnHeight;

	private void Start()
	{
		_amplitudeFactor = Random.Range(0.001f, 0.01f);
		_frequencyFactor = Random.Range(1f, 1.5f);
		_initialTimeOffset = 0f;
		Random.Range(0f, 1f);
		_liftAmount = Random.Range(0.4f, 0.6f);
		_rotationRate = Random.Range(60f, 80f);
	}

	private void Update()
	{
		if (!GameManager.Instance.IsPaused)
		{
			if (!_spawnHeight.HasValue)
			{
				_spawnHeight = base.transform.position.y;
			}
			else if (base.transform.position.y - _spawnHeight.Value > 3.75f)
			{
				Object.Destroy(base.gameObject);
			}
			float x = Mathf.Sin((Time.time + _initialTimeOffset) * _frequencyFactor) * _amplitudeFactor;
			float y = _liftAmount * Time.deltaTime;
			Vector3 vector = new Vector3(x, y, 0f);
			float num = Mathf.Sin((Time.time + _initialTimeOffset) * _frequencyFactor * 4f);
			base.transform.position += vector;
			_yRotation += _rotationRate * Time.deltaTime;
			_yRotation %= 360f;
			base.transform.rotation = Quaternion.Euler(0f, _yRotation, (0f - num) * _swayAmplitude * _frequencyFactor);
		}
	}
}
