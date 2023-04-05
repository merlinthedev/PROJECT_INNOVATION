using UnityEngine;

public class Spawner : MonoBehaviour {
    [SerializeField] SpawnerConfiguration configuration;
    [SerializeField] Transform spawnRoot;

    private float lastPickupTime = 0f;
    private bool hasItem = false;

    private void Start() {
        spawnItem();
    }

    private void Update() {
        if (!hasItem && Time.time - lastPickupTime > configuration.spawnDelay)
            spawnItem();
    }

    public void OnPickup(AInteractable pickable) {
        lastPickupTime = Time.time;
    }

    private void spawnItem() {
        if (hasItem) return;
        
        var item = Instantiate(configuration.GetRandomPrefab(), spawnRoot.position, Quaternion.identity, spawnRoot);
        item.spawner = this;
        hasItem = true;
    }
}
