using UnityEngine;
using TMPro;

public class UIScoreHandler : MonoBehaviour {
    [SerializeField] private TMP_Text scoreText;

    private void OnEnable() {
        EventBus<ScoreUIEvent>.Subscribe(onScoreUpdate);
    }

    private void OnDisable() {
        EventBus<ScoreUIEvent>.Unsubscribe(onScoreUpdate);
    }

    private void onScoreUpdate(ScoreUIEvent scoreUIEvent) {
        scoreText.SetText($"Score = {scoreUIEvent.score:0}");
    }
}
