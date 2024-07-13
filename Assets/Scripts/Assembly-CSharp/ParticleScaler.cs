using UnityEngine;

public class ParticleScaler : MonoBehaviour
{
	private ParticleSystem _particleSystem;

	private float globalScale;

	private float _baseStartSpeed;

	private float _baseStartSize;

	public bool calculateOnUpdate;

	private void Start()
	{
		_particleSystem = base.GetComponent<ParticleSystem>();
		if (_particleSystem == null)
		{
			Debug.LogError("Particle Scaler needs a Particle System to be on this component");
		}
		globalScale = GetGlobalScale();
		Debug.Log("Global scale was " + globalScale);
		_baseStartSpeed = _particleSystem.startSpeed;
		_baseStartSize = _particleSystem.startSize;
		SetParticleScale();
	}

	private void SetParticleScale()
	{
		_particleSystem.startSpeed = _baseStartSpeed * globalScale;
		_particleSystem.startSize = _baseStartSize * globalScale;
	}

	private float GetGlobalScale()
	{
		Transform parent = base.transform;
		float num = 1f;
		while (parent != null)
		{
			num *= GetMaxComponent(parent.localScale);
			parent = parent.parent;
		}
		return num;
	}

	private float GetMaxComponent(Vector3 v)
	{
		float num = v.x;
		if (v.y > num)
		{
			num = v.y;
		}
		if (v.z > num)
		{
			num = v.z;
		}
		return num;
	}

	private void Update()
	{
		if (calculateOnUpdate)
		{
			globalScale = base.transform.lossyScale.magnitude;
			SetParticleScale();
		}
	}
}
