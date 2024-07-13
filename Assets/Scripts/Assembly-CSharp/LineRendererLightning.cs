using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class LineRendererLightning : MonoBehaviour
{
	private LineRenderer _lr;

	public GameObject target;

	public float arcLength = 2f;

	public float arcVariation = 2f;

	public float inaccuracy = 1f;

	public bool IsPaused { get; private set; }

	private void Awake()
	{
		if (_lr == null)
		{
			_lr = GetComponent<LineRenderer>();
		}
		IsPaused = false;
	}

	private void Update()
	{
		if (!(target == null) && !IsPaused)
		{
			Vector3 vector = base.transform.position;
			int num = 1;
			_lr.SetPosition(0, base.transform.position);
			while ((double)Vector3.Distance(target.transform.position, vector) > 0.04 && num <= 100)
			{
				_lr.SetVertexCount(num + 1);
				Vector3 v = target.transform.position - vector;
				v.Normalize();
				v = Randomize(v, inaccuracy);
				v *= Random.Range(arcLength * arcVariation, arcLength);
				v += vector;
				_lr.SetPosition(num, v);
				num++;
				vector = v;
			}
		}
	}

	private Vector3 Randomize(Vector3 v3, float inaccuracy2)
	{
		v3 += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * inaccuracy2;
		v3.Normalize();
		return v3;
	}

	public void Pause()
	{
		IsPaused = true;
	}

	public void Unpause()
	{
		IsPaused = false;
	}
}
