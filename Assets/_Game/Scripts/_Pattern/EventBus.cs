using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Enum, List<Delegate>> listeners = new();

    public static void On<T>(Enum eventType, Action<T> callback)
    {
        if (!listeners.ContainsKey(eventType))
            listeners[eventType] = new();

        listeners[eventType].Add(callback);
    }

    public static void Off<T>(Enum eventType, Action<T> callback)
    {
        if (!listeners.ContainsKey(eventType))
            return;

        listeners[eventType].Remove(callback);

        if (listeners[eventType].Count == 0)
            listeners.Remove(eventType);
    }

    public static void Emit<T>(Enum eventType, T arg)
    {
        if (!listeners.TryGetValue(eventType, out var list))
            return;

        foreach (var del in list)
        {
            if (del is Action<T> action)
                action(arg);
#if UNITY_EDITOR
            else
                UnityEngine.Debug.LogWarning(
                    $"⚠️ Event {eventType} emit type mismatch"
                );
#endif
        }
    }

    public static void Clear()
    {
        listeners.Clear();
    }
}
