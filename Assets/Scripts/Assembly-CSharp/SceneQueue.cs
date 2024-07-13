using System.Collections.Generic;
using UnityEngine;

public class SceneQueue : MonoBehaviour
{
	private List<string> mScenesToLoad = new List<string>();

	private bool mLoading;

	private static SceneQueue mInst;

	public static SceneQueue Instance
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
			return mLoading;
		}
	}

	private void Awake()
	{
		if (mInst != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		mInst = this;
		Object.DontDestroyOnLoad(this);
	}

	public void LoadScene(string scene)
	{
		if (mScenesToLoad.Count == 0)
		{
			if (GameObject.Find("GameManager") != null)
			{
				Application.LoadLevel(scene);
			}
			else
			{
				Application.LoadLevelAdditiveAsync(scene);
			}
			mLoading = true;
		}
		else
		{
			mScenesToLoad.Add(scene);
		}
	}

	public void SceneLoaded()
	{
		if (mScenesToLoad.Count > 0)
		{
			Application.LoadLevelAdditiveAsync(mScenesToLoad[0]);
			mScenesToLoad.RemoveAt(0);
			mLoading = true;
		}
		else
		{
			mLoading = false;
		}
	}
}
