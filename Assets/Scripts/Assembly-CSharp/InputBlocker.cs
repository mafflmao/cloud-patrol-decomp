using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputBlocker
{
	private class StopInputDisposable : IDisposable
	{
		private string Id { get; set; }

		private InputBlocker Owner { get; set; }

		public StopInputDisposable(string id, InputBlocker owner)
		{
			Id = id;
			Owner = owner;
		}

		public void Dispose()
		{
			Owner.ReleaseStoppedInput(Id);
		}
	}

	private static InputBlocker _instance;

	private Stack<string> _inputDisabledStack = new Stack<string>();

	public static InputBlocker Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new InputBlocker();
			}
			return _instance;
		}
	}

	private InputBlocker()
	{
	}

	public IDisposable BlockInput(string id)
	{
		UIManager.instance.blockInput = false;
		_inputDisabledStack.Push(id);
		return new StopInputDisposable(id, this);
	}

	private void ReleaseStoppedInput(string id)
	{
		string text = _inputDisabledStack.Pop();
		if (text != id)
		{
			Debug.LogWarning("Someone tried to release the input out-of-order. This isn't always an error, but is normally a bad sign.");
		}
		UIManager.instance.blockInput = !_inputDisabledStack.Any();
	}
}
