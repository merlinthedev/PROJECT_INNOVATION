using shared;
using System;
using System.Collections.Generic;

public abstract class Event { }

public abstract class NetworkEvent : ISerializable {
    public abstract void Deserialize(Packet packet);
    public abstract void Serialize(Packet packet);
}

public class NetworkEventBus {
    private static Dictionary<Type, System.Action<NetworkEvent>> onEventRaised = new Dictionary<Type, System.Action<NetworkEvent>>();
    private static System.Action<NetworkEvent> onAnyRaised;

    public static void Subscribe<T>(System.Action<T> handler) where T : NetworkEvent {
        if (!onEventRaised.ContainsKey(typeof(T))) {
            onEventRaised.Add(typeof(T), null);
        }
        onEventRaised[typeof(T)] += (System.Action<NetworkEvent>)handler;
    }

    public static void SubscribeAll(System.Action<NetworkEvent> handler) {
        onAnyRaised += handler;
    }

    public static void Unsubscribe<T>(System.Action<T> handler) where T : NetworkEvent {
        if (!onEventRaised.ContainsKey(typeof(T))) {
            return;
        }
        onEventRaised[typeof(T)] -= (System.Action<NetworkEvent>)handler;
    }

    public static void UnsubscribeAll(System.Action<NetworkEvent> handler) {
        onAnyRaised -= handler;
    }

    public static void Raise(NetworkEvent e) {
        if (!onEventRaised.ContainsKey(e.GetType())) {
            return;
        }
        onEventRaised[e.GetType()]?.Invoke(e);
        onAnyRaised?.Invoke(e);
    }
}

public class EventBus<T> where T : Event {
    private static event System.Action<T> onEventRaised;
    public static void Subscribe(System.Action<T> handler) {
        onEventRaised += handler;
    }

    public static void Unsubscribe(System.Action<T> handler) {
        onEventRaised -= handler;
    }

    public static void Raise(T e) {
        onEventRaised?.Invoke(e);
    }
}

/*
    ==================================
    ============ EVENTS ==============
    ==================================
*/

public class JoinQuitEvent : Event {
    public int amountOfClients { get; private set; }

    public JoinQuitEvent(int amountOfClients) {
        this.amountOfClients = amountOfClients;
    }

}

public class OnStateQuit : Event {
    public string stateToQuit { get; private set; }

    public OnStateQuit(string stateToQuit) {
        this.stateToQuit = stateToQuit;
    }
}

public class OnStateEnter : Event {
    public string stateToEnter { get; private set; }

    public OnStateEnter(string stateToEnter) {
        this.stateToEnter = stateToEnter;
    }

}

/*
    ==================================
    ======== NETWORK EVENTS ==========
    ==================================
*/

