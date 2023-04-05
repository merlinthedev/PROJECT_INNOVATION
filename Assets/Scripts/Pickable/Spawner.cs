using UnityEngine;

public class Spawner : MonoBehaviour {
    [SerializeField] private SpawnerConfiguration configuration;
    [SerializeField] private Transform spawnRoot;

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

        var item = Instantiate(configuration.GetRandomPrefab(), spawnRoot.position, Quaternion.identity);
        item.transform.SetParent(spawnRoot);
        item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y + 0.5f, item.transform.position.z);
        Debug.Log("Spawned at " + transform.position);
        var networkTransform = item.GetComponent<NetworkTransform>();

        // networkTransform.Initialize();

        item.spawner = this;
        hasItem = true;
    }


    public enum Tier {
        Common,
        Uncommon,
        Rare,
        Legendary
    }
}
