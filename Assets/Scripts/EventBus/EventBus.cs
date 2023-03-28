public abstract class Event { }

public class EventBus<T> where T : Event {
    private static event System.Action<T> OnEventRaised;
    public static void Subscribe(System.Action<T> handler) {
        OnEventRaised += handler;
    }

    public static void Unsubscribe(System.Action<T> handler) {
        OnEventRaised -= handler;
    }

    public static void Raise(T e) {
        OnEventRaised?.Invoke(e);
    }
}