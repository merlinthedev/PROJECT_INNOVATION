using UnityEngine;
using System.Collections.Generic;

public class Spawner : AGuidSource {
    [SerializeField] private InteractableConfiguration interactables;
    [SerializeField] private SpawnerConfiguration configuration;
    [SerializeField] private Transform spawnRoot;

    public Storezone Storezone;

    private float lastPickupTime = 0f;
    private bool hasItem = false;

    public Tier tier;

    private void Start() {
        Storezone.Spawners.Add(this);
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
        var prefab = configuration.GetRandomPrefab();
        var prefabIndex = interactables.GetPrefabIndex(prefab);
        var item = Instantiate(prefab, spawnRoot.position, Quaternion.identity);
        // replace item id
        var networkTransform = item.GetComponent<NetworkTransform>();

        networkTransform.NewKey();

        var itemComponent = item.GetComponent<Item>();

        itemComponent.Storezone = Storezone;
        itemComponent.ItemStats.discount = Storezone.StoreDiscount;

        Item.Items.Add(item as Item);

        NetworkEventBus.Raise(new ItemSpawnedEvent {
            source = key,
            itemID = prefabIndex,
            itemGuid = item.GetComponent<NetworkTransform>().Key,
            itemDiscount = Storezone.StoreDiscount
        });

        item.transform.SetParent(spawnRoot);
        item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y + 0.5f, item.transform.position.z);
        Debug.Log("Spawned at " + transform.position);

        networkTransform.Initialize();

        item.spawner = this;
        hasItem = true;
    }

    public void UpdateItemStats() {
        ItemDiscountUpdateEvent itemDiscountUpdateEvent = new ItemDiscountUpdateEvent();
        itemDiscountUpdateEvent.source = key;
        itemDiscountUpdateEvent.discount = Storezone.StoreDiscount;


        var item = Item.Items.Find(x => x.spawner == this);
        if (item != null) {
            item.ItemStats.discount = Storezone.StoreDiscount;
            itemDiscountUpdateEvent.influencedItems.Add(item.GetComponent<NetworkTransform>().Key);
        }

        NetworkEventBus.Raise(itemDiscountUpdateEvent);
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
