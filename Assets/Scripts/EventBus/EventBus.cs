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