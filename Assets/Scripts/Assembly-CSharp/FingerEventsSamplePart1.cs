using UnityEngine;

public class FingerEventsSamplePart1 : SampleBase
{
	public GameObject fingerDownObject;

	public GameObject fingerStationaryObject;

	public GameObject fingerUpObject;

	public float chargeDelay = 0.5f;

	public float chargeTime = 5f;

	public float minSationaryParticleEmissionCount = 5f;

	public float maxSationaryParticleEmissionCount = 50f;

	public Material stationaryMaterial;

	public int requiredTapCount = 2;

	private ParticleSystem stationaryParticleEmitter;

	private int stationaryFingerIndex = -1;

	private Material originalMaterial;

	protected override string GetHelpText()
	{
		return "This sample lets you visualize and understand the FingerDown, FingerStationary and FingerUp events.\r\n\r\nINSTRUCTIONS:\r\n- Press, hold and release the red and blue spheres\r\n- Press & hold the green sphere without moving for a few seconds";
	}

	protected override void Start()
	{
		base.Start();
	}

	private void StopStationaryParticleEmitter()
	{
		base.UI.StatusText = string.Empty;
	}

	private void OnEnable()
	{
		Debug.Log("Registering finger gesture events from C# script");
		FingerGestures.OnFingerDown += FingerGestures_OnFingerDown;
		FingerGestures.OnFingerStationaryBegin += FingerGestures_OnFingerStationaryBegin;
		FingerGestures.OnFingerStationary += FingerGestures_OnFingerStationary;
		FingerGestures.OnFingerStationaryEnd += FingerGestures_OnFingerStationaryEnd;
		FingerGestures.OnFingerUp += FingerGestures_OnFingerUp;
	}

	private void OnDisable()
	{
		FingerGestures.OnFingerDown -= FingerGestures_OnFingerDown;
		FingerGestures.OnFingerStationaryBegin -= FingerGestures_OnFingerStationaryBegin;
		FingerGestures.OnFingerStationary -= FingerGestures_OnFingerStationary;
		FingerGestures.OnFingerStationaryEnd -= FingerGestures_OnFingerStationaryEnd;
		FingerGestures.OnFingerUp -= FingerGestures_OnFingerUp;
	}

	private void FingerGestures_OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		CheckSpawnParticles(fingerPos, fingerDownObject);
	}

	private void FingerGestures_OnFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
	{
		CheckSpawnParticles(fingerPos, fingerUpObject);
	}

	private void FingerGestures_OnFingerStationaryBegin(int fingerIndex, Vector2 fingerPos)
	{
		if (stationaryFingerIndex == -1)
		{
			GameObject gameObject = SampleBase.PickObject(fingerPos);
			if (gameObject == fingerStationaryObject)
			{
				base.UI.StatusText = "Begin stationary on finger " + fingerIndex;
				stationaryFingerIndex = fingerIndex;
				originalMaterial = gameObject.GetComponent<Renderer>().sharedMaterial;
				gameObject.GetComponent<Renderer>().sharedMaterial = stationaryMaterial;
			}
		}
	}

	private void FingerGestures_OnFingerStationary(int fingerIndex, Vector2 fingerPos, float elapsedTime)
	{
	}

	private void FingerGestures_OnFingerStationaryEnd(int fingerIndex, Vector2 fingerPos, float elapsedTime)
	{
		if (fingerIndex == stationaryFingerIndex)
		{
			base.UI.StatusText = "Stationary ended on finger " + fingerIndex + " - " + elapsedTime.ToString("N1") + " seconds elapsed";
			StopStationaryParticleEmitter();
			fingerStationaryObject.GetComponent<Renderer>().sharedMaterial = originalMaterial;
			stationaryFingerIndex = -1;
		}
	}

	private bool CheckSpawnParticles(Vector2 fingerPos, GameObject requiredObject)
	{
		GameObject gameObject = SampleBase.PickObject(fingerPos);
		if (!gameObject || gameObject != requiredObject)
		{
			return false;
		}
		SpawnParticles(gameObject);
		return true;
	}

	private void SpawnParticles(GameObject obj)
	{
	}
}
