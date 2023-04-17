using UnityEngine;
using System.Linq;

public class Spawner : AGuidSource {
    [SerializeField] private InteractableConfiguration interactables;
    [SerializeField] private SpawnerConfiguration configuration;
    [SerializeField] private Transform spawnRoot;

    public Storezone Storezone;

    private float lastPickupTime = 0f;
    [SerializeField] private bool hasItem = false;

    public Tier tier;

    private void Start() {
        //Storezone.Spawners.Add(this);
        if (NetworkManager.IsServer) spawnInteractable();
    }

    private void Update() {
        if (!hasItem && Time.time - lastPickupTime > configuration.spawnDelay && NetworkManager.IsServer)
            spawnInteractable();
    }

    public void OnPickup(AInteractable pickable) {
        lastPickupTime = Time.time;
        hasItem = false;
    }

    private void spawnInteractable() {
        if (hasItem) return;
        var prefab = configuration.GetRandomPrefab();
        var prefabIndex = interactables.GetPrefabIndex(prefab);
        var interactable = Instantiate(prefab, spawnRoot.position, Quaternion.identity);
        interactable.InteractableID = prefabIndex;
        // replace item id
        var networkTransform = interactable.GetComponent<NetworkTransform>();

        networkTransform.NewKey();

        AInteractable.interactables.Add(interactable);

        NetworkEventBus.Raise(new InteractableSpawnedEvent {
            source = key,
            InteractableID = prefabIndex,
            InteractableGuid = interactable.GetComponent<NetworkTransform>().key
        });
        
        if (interactable is Item) {
            var itemComponent = interactable.GetComponent<Item>();

            itemComponent.Storezone = Storezone;
            itemComponent.discount = Storezone.StoreDiscount;


            Item.Items.Add(networkTransform.key, interactable as Item);


            NetworkEventBus.Raise(new ItemDiscountUpdateEvent {
                source = key,
                discount = Storezone.StoreDiscount,
                influencedItems = new System.Collections.Generic.List<System.Guid> { interactable.GetComponent<NetworkTransform>().key }
            });
        }

        interactable.transform.SetParent(spawnRoot);
        interactable.transform.position = new Vector3(interactable.transform.position.x, interactable.transform.position.y + 1, interactable.transform.position.z);
        
        // Debug.Log("Spawned at " + transform.position);

        networkTransform.Initialize();

        interactable.spawner = this;
        hasItem = true;
    }

    public void UpdateItemStats() {
        ItemDiscountUpdateEvent itemDiscountUpdateEvent = new ItemDiscountUpdateEvent();
        itemDiscountUpdateEvent.source = key;
        itemDiscountUpdateEvent.discount = Storezone.StoreDiscount;


        // var item = Item.Items.Find(x => x.spawner == this);
        var item = Item.Items.Values.ToList().FirstOrDefault(x => x.spawner == this);
        if (item != null) {
            item.discount = Storezone.StoreDiscount;
            itemDiscountUpdateEvent.influencedItems.Add(item.GetComponent<NetworkTransform>().key);
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
