using System.Collections.Generic;
using UnityEngine;

public class WorldToMinimapHelper : MonoBehaviour {

    [SerializeField] private Camera worldCamera;
    [SerializeField] private Dictionary<Player, PlayerMinimapComponent> playerMinimapComponentMap = new Dictionary<Player, PlayerMinimapComponent>();

    private void Update() {
        foreach (var kvp in playerMinimapComponentMap) {
            kvp.Value.UpdatePosition(worldCamera.WorldToScreenPoint(kvp.Key.transform.position));
        }
    }

    public void AddPlayer(Player player) {
        if (!playerMinimapComponentMap.ContainsKey(player)) playerMinimapComponentMap.Add(player, player.playerMinimapComponent);
    }

    public void RemovePlayer(Player player) {
        playerMinimapComponentMap[player].PrepareForRemoval();

        if (playerMinimapComponentMap.ContainsKey(player)) playerMinimapComponentMap.Remove(player);


    }
}
