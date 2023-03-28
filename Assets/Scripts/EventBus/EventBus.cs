public abstract class Event { }

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