using shared;
using System;
using System.Collections.Generic;

public abstract class Event { }

public abstract class NetworkEvent : ISerializable {
    public abstract void Serialize(Packet packet);
    public abstract void Deserialize(Packet packet);

    public Guid source;
}

public class NetworkEventBus {
    private static Dictionary<Type, System.Action<NetworkEvent>> onEventRaised = new Dictionary<Type, System.Action<NetworkEvent>>();
    private static System.Action<NetworkEvent> onAnyRaised;

    public static readonly Type[] EventTypes = {
        typeof(NetworkEvent),
        typeof(TestNetworkEvent),
        typeof(JumpEvent),
        typeof(ItemPickedUpEvent),
        typeof(ItemSpawnedEvent),
    };

    public static void Subscribe<T>(System.Action<T> handler) where T : NetworkEvent {
        if (!onEventRaised.ContainsKey(typeof(T))) {
            onEventRaised.Add(typeof(T), null);
        }
        onEventRaised[typeof(T)] += (e) => handler((T)e);
        // onEventRaised[typeof(T)] += (System.Action<NetworkEvent>)handler;
    }

    public static void SubscribeToType(Type eventType, Action<NetworkEvent> handler) {
        if (!onEventRaised.ContainsKey(eventType)) {
            onEventRaised.Add(eventType, null);
        }
        onEventRaised[eventType] += handler;
    }

    public static void SubscribeAll(System.Action<NetworkEvent> handler) {
        onAnyRaised += handler;
    }

    public static void Unsubscribe<T>(System.Action<T> handler) where T : NetworkEvent {
        if (!onEventRaised.ContainsKey(typeof(T))) {
            return;
        }
        // onEventRaised[typeof(T)] -= (System.Action<NetworkEvent>)handler;
        onEventRaised[typeof(T)] -= (e) => handler((T)e);
    }

    public static void UnsubscribeFromType(Type eventType, Action<NetworkEvent> handler) {
        if (!onEventRaised.ContainsKey(eventType)) {
            return;
        }
        onEventRaised[eventType] -= handler;
    }

    public static void UnsubscribeAll(System.Action<NetworkEvent> handler) {
        onAnyRaised -= handler;
    }

    public static void Raise(NetworkEvent e) {
        onAnyRaised?.Invoke(e);
        if (!onEventRaised.ContainsKey(e.GetType())) {
            return;
        }
        onEventRaised[e.GetType()]?.Invoke(e);
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

public class InventoryUIEvent : Event {
    public Item item { get; set; }
    public ActionType actionType { get; set; }
    public Guid itemGuid { get; set; }
    public float discount { get; set; }

    public enum ActionType {
        Add,
        Remove,
        Clear
    }
}


public class PowerUpUIEvent : Event {
    public int PowerUpID { get; set; }
}

public class ScoreUIEvent : Event {
    public float score { get; private set; }

    public ScoreUIEvent(float score) {
        this.score = score;
    }
}

public class PregameUIListEvent : Event {
    public List<Guid> names { get; set; } = new List<Guid>();
}

public class PlayerDisconnectEventUI : Event {
    public Guid playerGuid { get; set; }
}

public class ServerScoreboardUpdateEvent : Event {
    // public List<string> scores { get; set; }
    public Dictionary<UnityEngine.Color, string> scores { get; set; } = new Dictionary<UnityEngine.Color, string>();
}

/*
    ==================================
    ======== NETWORK EVENTS ==========
    ==================================
*/

public class TestNetworkEvent : NetworkEvent {

    public override void Serialize(Packet packet) {
        packet.Write(source);
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
    }
}

public class JumpEvent : NetworkEvent {
    public override void Serialize(Packet packet) {
        packet.Write(source);
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
    }
}

public class InteractableSpawnedEvent : NetworkEvent {
    public int InteractableID { get; set; }
    public Guid InteractableGuid { get; set; }

    public override void Serialize(Packet packet) {
        packet.Write(source);
        packet.Write(InteractableID);
        packet.Write(InteractableGuid);
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
        InteractableID = packet.ReadInt();
        InteractableGuid = packet.ReadGuid();
    }
}

#region items

public class ItemSpawnedEvent : NetworkEvent {
    public int itemID { get; set; }
    public Guid itemGuid { get; set; }
    public float itemDiscount { get; set; }

    public override void Serialize(Packet packet) {
        packet.Write(source);
        packet.Write(itemID);
        packet.Write(itemGuid);
        packet.Write(itemDiscount);
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
        itemID = packet.ReadInt();
        itemGuid = packet.ReadGuid();
        itemDiscount = packet.ReadFloat();
    }
}

public class ItemPickedUpEvent : NetworkEvent {
    public Guid itemGuid { get; set; }
    public int itemInteractableID { get; set; }
    public bool shouldClear { get; set; }
    public float discount;

    public override void Serialize(Packet packet) {
        packet.Write(source);
        packet.Write(itemGuid);
        packet.Write(itemInteractableID);
        packet.Write(shouldClear);
        packet.Write(discount);
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
        itemGuid = packet.ReadGuid();
        itemInteractableID = packet.ReadInt();
        shouldClear = packet.ReadBool();
        discount = packet.ReadFloat();
    }
}

public class ItemsDroppedOffEvent : NetworkEvent {
    public List<Guid> droppedItems { get; set; } = new List<Guid>();
    public override void Serialize(Packet packet) {
        packet.Write(source);
        packet.Write(droppedItems.Count);
        foreach (Guid itemGuid in droppedItems) {
            packet.Write(itemGuid);
        }
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
        int count = packet.ReadInt();
        for (int i = 0; i < count; i++) {
            droppedItems.Add(packet.ReadGuid());
        }
    }
}

public class ItemsDiscardedEvent : NetworkEvent {
    public List<System.Guid> discardedItems { get; set; } = new List<System.Guid>();
    public override void Serialize(Packet packet) {
        packet.Write(source);
        packet.Write(discardedItems.Count);
        foreach (Guid itemGuid in discardedItems) {
            packet.Write(itemGuid);
        }
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
        int count = packet.ReadInt();
        for (int i = 0; i < count; i++) {
            discardedItems.Add(packet.ReadGuid());
        }
    }
}

public class ItemDiscountUpdateEvent : NetworkEvent {
    public float discount { get; set; }
    public List<Guid> influencedItems { get; set; } = new List<Guid>();

    public override void Serialize(Packet packet) {
        packet.Write(source);
        packet.Write(discount);
        packet.Write(influencedItems.Count);
        foreach (Guid itemGuid in influencedItems) {
            packet.Write(itemGuid);
        }
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
        discount = packet.ReadFloat();
        int count = packet.ReadInt();
        for (int i = 0; i < count; i++) {
            influencedItems.Add(packet.ReadGuid());
        }
    }
}

#endregion


#region powerups

public class PowerupSpawnedEvent : NetworkEvent {
    public int powerupID { get; set; }
    public Guid powerupGuid { get; set; }

    public override void Serialize(Packet packet) {
        packet.Write(source);
        packet.Write(powerupID);
        packet.Write(powerupGuid);
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
        powerupID = packet.ReadInt();
        powerupGuid = packet.ReadGuid();
    }
}

public class PowerUpPickedUpEvent : NetworkEvent {
    public Guid powerUpGuid { get; set; }
    public int PowerUpID { get; set; }

    public override void Serialize(Packet packet) {
        packet.Write(source);
        packet.Write(powerUpGuid);
        packet.Write(PowerUpID);
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
        powerUpGuid = packet.ReadGuid();
        PowerUpID = packet.ReadInt();
    }
}

public class PowerupUsedEvent : NetworkEvent {
    public override void Serialize(Packet packet) {
        packet.Write(source);
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
    }
}

#endregion


public class ScoreUpdatedEvent : NetworkEvent {
    public float score { get; set; }

    public override void Serialize(Packet packet) {
        packet.Write(source);
        packet.Write(score);
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
        score = packet.ReadFloat();
    }
}

public class NetworkTransformDestroyedEvent : NetworkEvent {
    public Guid destroyedTransformGuid { get; set; }

    public override void Serialize(Packet packet) {
        packet.Write(source);
        packet.Write(destroyedTransformGuid);
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
        destroyedTransformGuid = packet.ReadGuid();
    }
}

public class PlayerJoinedEvent : NetworkEvent {
    public override void Serialize(Packet packet) {
        packet.Write(source);
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
    }
}

public class GameHostChangedEvent : NetworkEvent {
    public override void Serialize(Packet packet) {
        packet.Write(source);
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
    }
}

public class StartGameEvent : NetworkEvent {
    public override void Serialize(Packet packet) {
        packet.Write(source);
    }

    public override void Deserialize(Packet packet) {
        source = packet.ReadGuid();
    }
}
