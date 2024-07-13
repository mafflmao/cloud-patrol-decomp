using System;
using System.Collections.Generic;
using UnityEngine;

internal static class Messenger
{
	public class BroadcastException : Exception
	{
		public BroadcastException(string msg)
			: base(msg)
		{
		}
	}

	public class ListenerException : Exception
	{
		public ListenerException(string msg)
			: base(msg)
		{
		}
	}

	private static MessengerHelper messengerHelper = new GameObject("MessengerHelper").AddComponent<MessengerHelper>();

	public static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

	public static List<string> permanentMessages = new List<string>();

	public static void MarkAsPermanent(string eventType)
	{
		permanentMessages.Add(eventType);
	}

	public static void Cleanup()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, Delegate> item in eventTable)
		{
			bool flag = false;
			foreach (string permanentMessage in permanentMessages)
			{
				if (item.Key == permanentMessage)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(item.Key);
			}
		}
		foreach (string item2 in list)
		{
			eventTable.Remove(item2);
		}
	}

	public static void PrintEventTable()
	{
		Debug.Log("\t\t\t=== MESSENGER PrintEventTable ===");
		foreach (KeyValuePair<string, Delegate> item in eventTable)
		{
			Debug.Log("\t\t\t" + item.Key + "\t\t" + item.Value);
		}
		Debug.Log("\n");
	}

	public static void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
	{
		if (!eventTable.ContainsKey(eventType))
		{
			eventTable.Add(eventType, null);
		}
		Delegate @delegate = eventTable[eventType];
		if ((object)@delegate != null && @delegate.GetType() != listenerBeingAdded.GetType())
		{
			throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, @delegate.GetType().Name, listenerBeingAdded.GetType().Name));
		}
	}

	public static void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
	{
		if (eventTable.ContainsKey(eventType))
		{
			Delegate @delegate = eventTable[eventType];
			if ((object)@delegate == null)
			{
				throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
			}
			if (@delegate.GetType() != listenerBeingRemoved.GetType())
			{
				throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, @delegate.GetType().Name, listenerBeingRemoved.GetType().Name));
			}
			return;
		}
		throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
	}

	public static void OnListenerRemoved(string eventType)
	{
		if ((object)eventTable[eventType] == null)
		{
			eventTable.Remove(eventType);
		}
	}

	public static void OnBroadcasting(string eventType)
	{
	}

	public static BroadcastException CreateBroadcastSignatureException(string eventType)
	{
		return new BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
	}

	public static void AddListener(string eventType, Callback handler)
	{
		OnListenerAdding(eventType, handler);
		eventTable[eventType] = (Callback)Delegate.Combine((Callback)eventTable[eventType], handler);
	}

	public static void AddListener<T>(string eventType, Callback<T> handler)
	{
		OnListenerAdding(eventType, handler);
		eventTable[eventType] = (Callback<T>)Delegate.Combine((Callback<T>)eventTable[eventType], handler);
	}

	public static void AddListener<T, U>(string eventType, Callback<T, U> handler)
	{
		OnListenerAdding(eventType, handler);
		eventTable[eventType] = (Callback<T, U>)Delegate.Combine((Callback<T, U>)eventTable[eventType], handler);
	}

	public static void AddListener<T, U, V>(string eventType, Callback<T, U, V> handler)
	{
		OnListenerAdding(eventType, handler);
		eventTable[eventType] = (Callback<T, U, V>)Delegate.Combine((Callback<T, U, V>)eventTable[eventType], handler);
	}

	public static void RemoveListener(string eventType, Callback handler)
	{
		OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (Callback)Delegate.Remove((Callback)eventTable[eventType], handler);
		OnListenerRemoved(eventType);
	}

	public static void RemoveListener<T>(string eventType, Callback<T> handler)
	{
		OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (Callback<T>)Delegate.Remove((Callback<T>)eventTable[eventType], handler);
		OnListenerRemoved(eventType);
	}

	public static void RemoveListener<T, U>(string eventType, Callback<T, U> handler)
	{
		OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (Callback<T, U>)Delegate.Remove((Callback<T, U>)eventTable[eventType], handler);
		OnListenerRemoved(eventType);
	}

	public static void RemoveListener<T, U, V>(string eventType, Callback<T, U, V> handler)
	{
		OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (Callback<T, U, V>)Delegate.Remove((Callback<T, U, V>)eventTable[eventType], handler);
		OnListenerRemoved(eventType);
	}

	private static bool Dictionary_TryGetValue(Dictionary<string, Delegate> aDict, string aKey, out Delegate aValue)
	{
		return aDict.TryGetValue(aKey, out aValue);
	}

	public static void Broadcast(string eventType)
	{
		OnBroadcasting(eventType);
		Delegate aValue;
		if (Dictionary_TryGetValue(eventTable, eventType, out aValue))
		{
			Callback callback = aValue as Callback;
			if (callback == null)
			{
				throw CreateBroadcastSignatureException(eventType);
			}
			callback();
		}
	}

	public static void Broadcast<T>(string eventType, T arg1)
	{
		OnBroadcasting(eventType);
		Delegate aValue;
		if (Dictionary_TryGetValue(eventTable, eventType, out aValue))
		{
			Callback<T> callback = aValue as Callback<T>;
			if (callback == null)
			{
				throw CreateBroadcastSignatureException(eventType);
			}
			callback(arg1);
		}
	}

	public static void Broadcast<T, U>(string eventType, T arg1, U arg2)
	{
		OnBroadcasting(eventType);
		Delegate aValue;
		if (Dictionary_TryGetValue(eventTable, eventType, out aValue))
		{
			Callback<T, U> callback = aValue as Callback<T, U>;
			if (callback == null)
			{
				throw CreateBroadcastSignatureException(eventType);
			}
			callback(arg1, arg2);
		}
	}

	public static void Broadcast<T, U, V>(string eventType, T arg1, U arg2, V arg3)
	{
		OnBroadcasting(eventType);
		Delegate aValue;
		if (Dictionary_TryGetValue(eventTable, eventType, out aValue))
		{
			Callback<T, U, V> callback = aValue as Callback<T, U, V>;
			if (callback == null)
			{
				throw CreateBroadcastSignatureException(eventType);
			}
			callback(arg1, arg2, arg3);
		}
	}
}
