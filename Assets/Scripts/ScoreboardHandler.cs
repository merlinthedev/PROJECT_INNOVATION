using System.Collections.Generic;
using UnityEngine;

public class ScoreboardHandler : MonoBehaviour {

    public static ScoreboardHandler Instance { get; private set; }

    [SerializeField] private List<Player> scoreboardList = new List<Player>();

    private void Awake() {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("Instance already exists, abandoning object!");
            return;
        }

        Instance = this;
    }

    private void OnEnable() {
        NetworkEventBus.Subscribe<ScoreUpdatedEvent>(onScoreUpdated);
    }

    private void OnDisable() {
        NetworkEventBus.Unsubscribe<ScoreUpdatedEvent>(onScoreUpdated);
    }

    private void onScoreUpdated(ScoreUpdatedEvent scoreUpdatedEvent) {
        var updatedPlayer = scoreboardList.Find(player => player.key == scoreUpdatedEvent.source);
        if (updatedPlayer == null) return;

        Debug.Log("Score updated for player " + updatedPlayer.key + " to " + scoreUpdatedEvent.score);
        updatedPlayer.SetScore(scoreUpdatedEvent.score);

        sortScoreList();
        updateScoreboardUIHandler();
    }

    private void updateScoreboardUIHandler() {
        List<string> parsedScores = new List<string>();
        foreach (var player in scoreboardList) {
            // get the first 3 characters of the player key
            var key = player.key.ToString().Substring(0, 3);
            parsedScores.Add(key + ": " + Mathf.RoundToInt(player.GetScore()).ToString());
        }

        EventBus<ServerScoreboardUpdateEvent>.Raise(new ServerScoreboardUpdateEvent {
            scores = parsedScores
        });
    }

    private void sortScoreList() {
        scoreboardList.Sort((x, y) => y.GetScore().CompareTo(x.GetScore()));
    }

    public void AddPlayer(Player player) {
        scoreboardList.Add(player);
        sortScoreList();
        updateScoreboardUIHandler();
    }

    public void RemovePlayer(Player player) {
        scoreboardList.Remove(player);
        sortScoreList();
        updateScoreboardUIHandler();
    }


}
