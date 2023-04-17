using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayerListUIHandler : MonoBehaviour {

    [SerializeField] private TMP_Text playerListUIPrefab;
    //list of players in the lobby
    private Dictionary<string, TMP_Text> playerList = new Dictionary<string, TMP_Text>();

    private void OnEnable() {
        EventBus<PregameUIListEvent>.Subscribe(onListReceived);
        EventBus<PlayerDisconnectEventUI>.Subscribe(onPlayerDisconnect);
        NetworkEventBus.Subscribe<PlayerJoinedEvent>(onPlayerJoined);
    }

    private void OnDisable() {
        EventBus<PregameUIListEvent>.Unsubscribe(onListReceived);
        EventBus<PlayerDisconnectEventUI>.Unsubscribe(onPlayerDisconnect);
        NetworkEventBus.Unsubscribe<PlayerJoinedEvent>(onPlayerJoined);
    }

    private void onPlayerDisconnect(PlayerDisconnectEventUI e) {
        var uiText = e.playerGuid.ToString().Substring(0, 3);
        if (!playerList.ContainsKey(uiText)) {
            return;
        }
        Destroy(playerList[uiText].gameObject);
        playerList.Remove(uiText);
    }

    private void onListReceived(PregameUIListEvent e) {
        playerList.Clear();
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        foreach (var player in e.names) {
            var uiText = player.ToString().Substring(0, 3);
            if (playerList.ContainsKey(uiText)) {
                continue;
            }
            var playerListUI = Instantiate(playerListUIPrefab, transform);
            playerListUI.text = player.ToString().Substring(0, 3);
            playerList.Add(playerListUI.text, playerListUI);
        }
    }

    private void onPlayerJoined(PlayerJoinedEvent e) {
        var uiText = e.source.ToString().Substring(0, 3);
        if (playerList.ContainsKey(uiText)) {
            return;
        }
        var playerListUI = Instantiate(playerListUIPrefab, transform);
        playerListUI.text = uiText;
        playerList.Add(playerListUI.text, playerListUI);
    }


}
