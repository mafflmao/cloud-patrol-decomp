using System;
using System.Collections;
using System.Text;
using UnityEngine;

public class PortalManager : SingletonMonoBehaviour
{
	private bool pulseColorDirection = true;

	private int tagIndex = -1;

	private bool readTagData = true;

	private byte pulseColorMin = 25;

	private byte pulseColorMax = 225;

	private int pulseRange;

	private int pulseFrequency = 2;

	public bool ShouldDetectToys;

	public bool BatteriesAreLow { get; private set; }

	public bool PortalConnected { get; private set; }

	public bool TooManyToysDetected { get; private set; }

	public bool ToyDetected { get; private set; }

	public ushort DetectedToy { get; private set; }

	public ushort DetectedToySubType { get; private set; }

	public string DetectedToyWebcode { get; private set; }

	public static PortalManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<PortalManager>();
		}
	}

	protected override void AwakeOnce()
	{
		base.AwakeOnce();
		PortalConnected = false;
		TooManyToysDetected = false;
		ToyDetected = false;
		DetectedToy = 0;
		DetectedToySubType = 0;
		DetectedToyWebcode = string.Empty;
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void OnEnable()
	{
		StateManager.StateDeactivated += HandleStateDeactivated;
		pulseRange = pulseColorMax - pulseColorMin;
		EnablePortal();
	}

	private void OnDisable()
	{
		StateManager.StateDeactivated -= HandleStateDeactivated;
		DisablePortal();
	}

	public void ShutdownPortal()
	{
		DisablePortal();
	}

	private void HandleStateDeactivated(object sender, StateEventArgs e)
	{
		ShutdownPortal();
	}

	private void EnablePortal()
	{
		if (!Application.isEditor)
		{
			LibPortal.pcSet_Enabled(true);
			LibPortal.pcSet_Connect(true);
			LibPortal.pcSet_AntennaEnabled(true);
			LibPortal.pcSet_Verbose(false);
			StartCoroutine(PortalUpdate());
		}
	}

	private void DisablePortal()
	{
		if (!Application.isEditor)
		{
			LibPortal.pcSet_Color(0, 0, 0);
			LibPortal.pcSet_Enabled(false);
			LibPortal.pcSet_Connect(false);
			LibPortal.pcSet_AntennaEnabled(false);
			LibPortal.pcSet_Verbose(false);
		}
	}

	private IEnumerator PortalUpdate()
	{
		while (LibPortal.pcGet_Enabled())
		{
			BatteriesAreLow = false;
			if (LibPortal.pcGet_Connected())
			{
				PortalConnected = true;
				if (LibPortal.pcGet_BatteryStatus() == PortalControllerBatteryStatus.kBatteryStatusRed)
				{
					BatteriesAreLow = true;
					LibPortal.pcSet_Color(100, 0, 0);
				}
				else
				{
					UpdatePortalPulseColor();
				}
			}
			else if (!LibPortal.pcGet_Connected())
			{
				PortalConnected = false;
				LibPortal.pcSet_Connect(true);
				LibPortal.pcSet_AntennaEnabled(true);
				tagIndex = -1;
				ToyDetected = false;
			}
			TooManyToysDetected = false;
			if (LibPortal.pcGet_Connected() && LibPortal.pcGet_AntennaEnabled() && ShouldDetectToys)
			{
				int totalTagsDetected = 0;
				int tagCount = LibPortal.pcGet_PortalTagCount();
				for (int j = 0; j < tagCount; j++)
				{
					if (LibPortal.ptGet_Presence(j) == PortalTagPresence.kJustArrived)
					{
						totalTagsDetected++;
						if (tagIndex == -1)
						{
							tagIndex = j;
							readTagData = true;
						}
					}
				}
				for (int i = 0; i < tagCount; i++)
				{
					if (LibPortal.ptGet_Presence(i) == PortalTagPresence.kPresent)
					{
						totalTagsDetected++;
						if (tagIndex == -1)
						{
							tagIndex = i;
							readTagData = true;
						}
					}
				}
				if (tagIndex != -1 && totalTagsDetected == 1)
				{
					PortalTagPresence tagPresence = LibPortal.ptGet_Presence(tagIndex);
					if (tagPresence == PortalTagPresence.kPresent || tagPresence == PortalTagPresence.kJustArrived)
					{
						if (readTagData)
						{
							LibPortal.ptReadTagData(tagIndex);
							readTagData = false;
						}
						if (LibPortal.ptGet_DoneReading(tagIndex) && !ToyDetected)
						{
							byte[] webcodeData = new byte[11];
							LibPortal.ptGet_WebCode(webcodeData, 11u, tagIndex);
							string webcode = Encoding.UTF8.GetString(webcodeData);
							DetectedToyWebcode = webcode.Insert(5, "-");
							DetectedToy = LibPortal.ptGet_ToyType(tagIndex);
							DetectedToySubType = LibPortal.ptGet_SubType(tagIndex);
							Debug.Log(string.Format("Toy Detected: {0}:{1} - {2}", DetectedToy, DetectedToySubType, DetectedToyWebcode));
							ToyDetected = true;
						}
					}
					else
					{
						tagIndex = -1;
						ToyDetected = false;
					}
				}
				else if (totalTagsDetected > 1)
				{
					TooManyToysDetected = true;
					ToyDetected = false;
					tagIndex = -1;
				}
				else
				{
					tagIndex = -1;
					readTagData = true;
					ToyDetected = false;
				}
			}
			else
			{
				ToyDetected = false;
			}
			LibPortal.pcUpdate(Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void UpdatePortalPulseColor()
	{
		byte red = 0;
		byte green = 0;
		byte blue = 0;
		LibPortal.pcGet_Color(ref red, ref green, ref blue);
		int num = pulseRange * pulseFrequency;
		red = (pulseColorDirection ? ((!((float)(int)red + (float)num * Time.deltaTime >= (float)(int)pulseColorMax)) ? ((byte)(red + Convert.ToByte((float)num * Time.deltaTime))) : pulseColorMax) : ((!((float)(int)red - (float)num * Time.deltaTime <= (float)(int)pulseColorMin)) ? ((byte)(red - Convert.ToByte((float)num * Time.deltaTime))) : pulseColorMin));
		if (red == pulseColorMax)
		{
			pulseColorDirection = false;
		}
		else if (red == pulseColorMin)
		{
			pulseColorDirection = true;
		}
		LibPortal.pcSet_Color(red, red, red);
	}
}
