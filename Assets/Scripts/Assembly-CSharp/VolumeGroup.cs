using System;
using System.Collections.Generic;
using UnityEngine;

public class VolumeGroup : ScriptableObject
{
	public class VolumeChangedEventArgs : EventArgs
	{
		public float OldVolumeLevel { get; private set; }

		public VolumeChangedEventArgs(float oldVolumeLevel)
		{
			OldVolumeLevel = oldVolumeLevel;
		}
	}

	public VolumeGroup parent;

	public float volume = 1f;

	[NonSerialized]
	private float _runtimeVolume = 1f;

	[NonSerialized]
	private bool _initialized;

	private static Dictionary<string, VolumeGroup> _groupNameToGroupMap = new Dictionary<string, VolumeGroup>();

	private static Dictionary<string, float> _groupNameToInitialVolumeMap = new Dictionary<string, float>();

	public float RuntimeVolume
	{
		get
		{
			return _runtimeVolume;
		}
		set
		{
			if (_runtimeVolume != value)
			{
				float runtimeVolume = _runtimeVolume;
				_runtimeVolume = value;
				OnVolumeChanged(runtimeVolume);
			}
		}
	}

	public event EventHandler<VolumeChangedEventArgs> CombinedVolumeChanged;

	public float GetCurrentCombinedRuntimeVolume()
	{
		float num = ((!(parent == null)) ? parent.GetCurrentCombinedRuntimeVolume() : 1f);
		return volume * _runtimeVolume * num;
	}

	private void OnVolumeChanged(float delta)
	{
		if (this.CombinedVolumeChanged != null)
		{
			this.CombinedVolumeChanged(this, new VolumeChangedEventArgs(delta));
		}
	}

	public void Initialize()
	{
		if (!_initialized)
		{
			_initialized = true;
			float value;
			if (!_groupNameToInitialVolumeMap.TryGetValue(base.name, out value))
			{
				value = 1f;
			}
			else
			{
				_groupNameToInitialVolumeMap.Remove(base.name);
			}
			RuntimeVolume = value;
			_groupNameToGroupMap.Add(base.name, this);
		}
	}

	public static void SetVolume(string volumeGroupName, float volume)
	{
		VolumeGroup value;
		if (_groupNameToGroupMap.TryGetValue(volumeGroupName, out value))
		{
			value.RuntimeVolume = volume;
		}
		else
		{
			_groupNameToInitialVolumeMap[volumeGroupName] = volume;
		}
	}
}
