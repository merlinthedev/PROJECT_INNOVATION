using UnityEngine;

public class GameOverStateManager : MonoBehaviour {

    [SerializeField] private TMPro.TMP_Text m_Text;


    private void OnEnable() {
        EventBus<GameOverUIEvent>.Subscribe(onGameOverUI);
    }

    private void OnDisable() {
        EventBus<GameOverUIEvent>.Unsubscribe(onGameOverUI);
    }

    private void onGameOverUI(GameOverUIEvent gameOverEvent) {
        Debug.Log("RECEIVED GAME OVER UI EVENT");

        if (gameOverEvent.winner == GameClient.getInstance().GetGuid()) {
            m_Text.SetText($"You have won! Score: {gameOverEvent.score}");
        } else {
            m_Text.SetText($"Player with GUID {gameOverEvent.winner.ToString().Substring(0, 3)} has won with score: {gameOverEvent.score}");
        }
    }
}
