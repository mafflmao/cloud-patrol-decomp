using System;
using UnityEngine;

public class ActivatorEventArgs : EventArgs
{
	public GameObject LevelRoot { get; private set; }

	public ActivatorEventArgs(GameObject levelRoot)
	{
		LevelRoot = levelRoot;
	}
}
