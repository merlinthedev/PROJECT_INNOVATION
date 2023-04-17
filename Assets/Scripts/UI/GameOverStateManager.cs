using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverStateManager : MonoBehaviour {

    [SerializeField] private TMPro.TMP_Text m_Text;


    private void OnEnable() {
        NetworkEventBus.Subscribe<GameOverEvent>(onGameOver);
    }

    private void OnDisable() {
        NetworkEventBus.Unsubscribe<GameOverEvent>(onGameOver);
    }

    private void onGameOver(GameOverEvent gameOverEvent) {
        if (gameOverEvent.source == GameClient.getInstance().GetGuid()) {
            m_Text.SetText($"You have won! Score: {gameOverEvent.score}");
        } else {
            m_Text.SetText($"Player with GUID {gameOverEvent.source} has won with score: {gameOverEvent.score}");
        }
    }
}
