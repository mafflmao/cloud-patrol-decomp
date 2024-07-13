using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
	public const string DefaultBackStateName = "Loadout";

	private Dictionary<string, StateRoot> mStates = new Dictionary<string, StateRoot>();

	private List<string> mLoadingStates = new List<string>();

	private StateRoot mCurrent;

	private string mNextState = string.Empty;

	public bool locked;

	public bool autoActivate = true;

	public string lastStateName = string.Empty;

	public string backStateName = string.Empty;

	private static StateManager mInst;

	public static StateManager Instance
	{
		get
		{
			return mInst;
		}
	}

	public bool Loading
	{
		get
		{
			return mLoadingStates.Count != 0;
		}
	}

	public int StateCount
	{
		get
		{
			return mStates.Count;
		}
	}

	public StateRoot CurrentState
	{
		get
		{
			return mCurrent;
		}
	}

	public string CurrentStateName
	{
		get
		{
			return mCurrent.stateName;
		}
	}

	public string[] LoadedStates
	{
		get
		{
			Dictionary<string, StateRoot>.KeyCollection keys = mStates.Keys;
			string[] array = new string[keys.Count];
			keys.CopyTo(array, 0);
			return array;
		}
	}

	public static event EventHandler<StateEventArgs> StateDeactivated;

	public static event EventHandler<StateEventArgs> StateActivated;

	private void Awake()
	{
		if (mInst != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			mInst = this;
		}
	}

	private void Start()
	{
		StartCoroutine(SwitchToNextState());
	}

	public void StateLoaded(StateRoot state)
	{
		mLoadingStates.Remove(state.stateName);
		if (mStates.ContainsKey(state.stateName))
		{
			Debug.LogError("State " + state.stateName + " already exists");
		}
		mStates[state.stateName] = state;
		state.BroadcastMessage("OnStateLoaded", SendMessageOptions.DontRequireReceiver);
		if (autoActivate && mCurrent == null)
		{
			mCurrent = state;
			mCurrent.gameObject.BroadcastMessage("OnStateActivate", string.Empty, SendMessageOptions.DontRequireReceiver);
			OnStateActivated(string.Empty);
		}
	}

	public IEnumerator SetCurrentStateCoroutine(StateRoot state)
	{
		if (state != mCurrent || locked)
		{
			if (mCurrent != null)
			{
				string stateName = ((!(state != null)) ? string.Empty : state.stateName);
				mCurrent.gameObject.BroadcastMessage("OnStateDeactivate", stateName, SendMessageOptions.DontRequireReceiver);
				OnStateDeactivated(stateName);
			}
			string oldStateName = (lastStateName = ((!(mCurrent != null)) ? string.Empty : mCurrent.stateName));
			if (mCurrent.storeStateInBackHistory)
			{
				backStateName = lastStateName;
			}
			float animateOutWaitTime = 0.5f;
			StateController currentStateController = mCurrent.gameObject.GetComponent<StateController>();
			if (currentStateController != null)
			{
				animateOutWaitTime = currentStateController.animateOutTime;
			}
			yield return new WaitForSeconds(animateOutWaitTime);
			mCurrent = state;
			if (mCurrent != null)
			{
				mCurrent.gameObject.BroadcastMessage("OnStateActivate", oldStateName, SendMessageOptions.DontRequireReceiver);
				OnStateActivated(oldStateName);
			}
		}
	}

	private void OnStateDeactivated(string stateName)
	{
		if (StateManager.StateDeactivated != null)
		{
			StateManager.StateDeactivated(this, new StateEventArgs(stateName));
		}
	}

	private void OnStateActivated(string oldStateName)
	{
		if (StateManager.StateActivated != null)
		{
			StateManager.StateActivated(this, new StateEventArgs(oldStateName));
		}
	}

	public void SetCurrentState(string name)
	{
		StateRoot stateRoot = mStates[name];
		if (stateRoot != null)
		{
			StartCoroutine(SetCurrentStateCoroutine(stateRoot));
		}
	}

	public void UnloadState(StateRoot state)
	{
		if (state != mCurrent)
		{
			state.gameObject.BroadcastMessage("OnStateUnloaded", SendMessageOptions.DontRequireReceiver);
			UnityEngine.Object.Destroy(state.gameObject);
			mStates.Remove(state.stateName);
		}
	}

	public void UnloadState(string name)
	{
		StateRoot stateRoot = mStates[name];
		if (stateRoot != null)
		{
			UnloadState(stateRoot);
		}
	}

	public void UnloadAll()
	{
		foreach (StateRoot value in mStates.Values)
		{
			value.gameObject.BroadcastMessage("OnStateUnloaded", SendMessageOptions.DontRequireReceiver);
			UnityEngine.Object.Destroy(value.gameObject);
		}
		mStates.Clear();
	}

	public void ClearAll()
	{
		mStates.Clear();
	}

	public bool Exists(string name)
	{
		return mStates.ContainsKey(name);
	}

	public void LoadFromSaveState(string name, string scene)
	{
		mLoadingStates.Add(name);
		SceneQueue.Instance.LoadScene(scene);
	}

	public void LoadFromSaveState(string name)
	{
		LoadFromSaveState(name, name);
	}

	public bool IsLoading(string name)
	{
		return mLoadingStates.Contains(name);
	}

	public void LoadAndActivateState(string name, string scene)
	{
		UIManager.instance.blockInput = true;
		if (mNextState.Length > 0)
		{
			Debug.LogWarning(mNextState + " is already waiting to be loaded");
		}
		if (!Exists(name) && !mLoadingStates.Contains(name))
		{
			LoadFromSaveState(name, scene);
		}
		mNextState = name;
	}

	public void LoadAndActivateState(string name)
	{
		LoadAndActivateState(name, name);
	}

	public void LoadAndActivatePreviousState()
	{
		if (!mCurrent.canGoBack)
		{
			return;
		}
		if (mCurrent.backStateName == string.Empty)
		{
			if (backStateName == string.Empty)
			{
				LoadAndActivateState("Loadout");
			}
			else
			{
				LoadAndActivateState(backStateName);
			}
		}
		else
		{
			LoadAndActivateState(mCurrent.backStateName);
		}
	}

	private IEnumerator SwitchToNextState()
	{
		while (true)
		{
			if (mNextState.Length > 0 && Exists(mNextState))
			{
				SetCurrentState(mNextState);
				mNextState = string.Empty;
			}
			yield return 0;
		}
	}

	private void OnDestroy()
	{
		if (mInst == this)
		{
			mInst = null;
		}
	}
}
