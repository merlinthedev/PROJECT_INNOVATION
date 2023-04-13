using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardUIHandler : MonoBehaviour {
    [SerializeField] private GameObject scoreboardEntryPrefab;
    [SerializeField] private Transform scoreboardEntryHolder;
    private List<GameObject> scoreboardEntries = new List<GameObject>();

    public void OnEnable() {
        EventBus<ServerScoreboardUpdateEvent>.Subscribe(onServerScoreboardUpdate);
    }
    public void OnDisable() {
        EventBus<ServerScoreboardUpdateEvent>.Unsubscribe(onServerScoreboardUpdate);
    }

    private void onServerScoreboardUpdate(ServerScoreboardUpdateEvent serverScoreboardUpdateEvent) {

        // print list to console
        Debug.Log("Scoreboard:");
        foreach (var score in serverScoreboardUpdateEvent.scores) {
            Debug.Log(score);
        }

        scoreboardEntries.Clear();
        foreach (Transform child in scoreboardEntryHolder) {
            Destroy(child.gameObject);
        }

        foreach (var score in serverScoreboardUpdateEvent.scores) {
            var entry = Instantiate(scoreboardEntryPrefab, scoreboardEntryHolder);
            entry.GetComponent<TMP_Text>().SetText(score);
            entry.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -scoreboardEntries.Count * 30);
            scoreboardEntries.Add(entry);
        }
    }
}
