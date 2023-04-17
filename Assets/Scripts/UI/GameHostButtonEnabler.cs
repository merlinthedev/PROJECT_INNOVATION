using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHostButtonEnabler : MonoBehaviour {
    [SerializeField] private Button button;

    private void Start() {
        button.interactable = GameClient.getInstance().GetGuid() == GameClient.getInstance().gameHostGuid;
    }

    void OnEnable() {
        NetworkEventBus.Subscribe<GameHostChangedEvent>(onGameHostChanged);
    }

    void OnDisable() {
        NetworkEventBus.Unsubscribe<GameHostChangedEvent>(onGameHostChanged);
    }

    private void onGameHostChanged(GameHostChangedEvent e) {
        button.interactable = GameClient.getInstance().GetGuid() == e.source;
    }
}
