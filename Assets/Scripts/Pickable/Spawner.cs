using UnityEngine;
using System.Collections.Generic;

public class Spawner : AGuidSource {

    public static List<Spawner> Spawners = new List<Spawner>();

    [SerializeField] private SpawnerConfiguration configuration;
    [SerializeField] private Transform spawnRoot;

    public Storezone Storezone;

    private float lastPickupTime = 0f;
    private bool hasItem = false;

    public Tier tier;

    private void Start() {
        Spawners.Add(this);
        if (NetworkManager.IsServer) spawnItem();
    }

    private void Update() {
        if (!hasItem && Time.time - lastPickupTime > configuration.spawnDelay && NetworkManager.IsServer)
            spawnItem();
    }

    public void OnPickup(AInteractable pickable) {
        lastPickupTime = Time.time;
        hasItem = false;
    }

    private void spawnItem() {
        if (hasItem) return;

        var item = Instantiate(configuration.GetRandomPrefab(), spawnRoot.position, Quaternion.identity);
        // replace item id
        item.GetComponent<NetworkTransform>().NewKey();

        var itemComponent = item.GetComponent<Item>();

        itemComponent.itemStats.discount = Storezone.StoreDiscount;
        itemComponent.Storezone = Storezone;

        Item.Items.Add((Item)item);

        NetworkEventBus.Raise(new ItemSpawnedEvent {
            source = key,
            itemID = 0,
            itemGuid = item.GetComponent<NetworkTransform>().Key,
            itemDiscount = Storezone.StoreDiscount
        });

        item.transform.SetParent(spawnRoot);
        item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y + 0.5f, item.transform.position.z);
        Debug.Log("Spawned at " + transform.position);
        var networkTransform = item.GetComponent<NetworkTransform>();

        networkTransform.Initialize();

        item.spawner = this;
        hasItem = true;
    }

    public void SetHasItem(bool hasItem) {
        this.hasItem = hasItem;
    }


    public enum Tier {
        Common,
        Uncommon,
        Rare,
        Legendary
    }
}
