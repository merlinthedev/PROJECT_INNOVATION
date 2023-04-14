using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardPowerup : PowerUp
{
    [SerializeField] private Hazard hazardPrefab;
    [SerializeField] private Vector3 hazardOffset = new Vector3(0, 0, 0);
    [SerializeField] private InteractableConfiguration interactableConfig;

    protected override void OnPickUp() {
    }

    protected override void OnUse(Player player) {
        var hazard = Instantiate(hazardPrefab, player.transform.position + (player.transform.rotation * hazardOffset), Quaternion.identity);
        hazard.GetComponent<NetworkTransform>().NewKey();
        hazard.GetComponent<NetworkTransform>().Initialize();
        int prefabIndex = interactableConfig.GetPrefabIndex(hazardPrefab);
        hazard.InteractableID = prefabIndex;

        NetworkEventBus.Raise(new InteractableSpawnedEvent {
            source = key,
            InteractableID = prefabIndex,
            InteractableGuid = hazard.key
        });
    }
}
