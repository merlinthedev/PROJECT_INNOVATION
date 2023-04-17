using UnityEngine;

public class ServerGameManager : MonoBehaviour {

    [SerializeField] private float startTime = 0f;
    [SerializeField] private float gameDuration = 0f;

    private void OnEnable() {
        NetworkEventBus.Subscribe<StartGameEvent>(onStartGame);
    }

    private void OnDisable() {
        NetworkEventBus.Unsubscribe<StartGameEvent>(onStartGame);
    }

    private void onStartGame(StartGameEvent e) {
        startTime = Time.time;
    }

    private void Update() {
        if (startTime == 0f) return;

        // every full second send a time update event
        if (Time.time - startTime > 1f) {
            EventBus<TimeUpdateEvent>.Raise(new TimeUpdateEvent {
                time = gameDuration - (Time.time - startTime),
            });
        }

        if (Time.time - startTime > gameDuration) {
            // game over
            Debug.Log("Game over!");
            startTime = 0f;

            //send game over event
            NetworkEventBus.Raise(new GameOverEvent {
                source = ScoreboardHandler.Instance.GetFirstPlace(),
            });

            GameManager.Instance.SetState("GameOver");
        }

    }
}