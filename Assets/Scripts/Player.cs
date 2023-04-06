using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour {
    #region Items
    [SerializeField] Transform itemHolder;
    [SerializeField] private int capacity = 2;
    [SerializeField] private List<Item> items = new List<Item>();
    //current capacity we have
    public int leftoverCapacity { get { return capacity - items.Sum(x => x.Weight); } }

    public List<Item> GetInventory { get { return new List<Item>(items); } }

    public void AddItem(Item item) {
        items.Add(item);
        item.transform.SetParent(itemHolder);
    }

    public void RemoveItem(Item item) {
        items.Remove(item);
    }

    public void DropOffItems() {
        items.Clear();

        NetworkEventBus.Raise(new ItemDroppedOffEvent {
            source = GetComponent<NetworkTransform>().Key,
        });
    }

    #endregion

    #region PowerUps
    //we can only hold one powerup at a time
    [SerializeField] private Transform powerUpHolder;
    [SerializeField] private PowerUp powerUp;
    public PowerUp getPowerUp { get { return powerUp; } }

    public void AddPowerUp(PowerUp powerUp) {
        this.powerUp = powerUp;
        powerUp.transform.SetParent(powerUpHolder);
    }

    public void RemovePowerUp() {
        powerUp = null;
    }

    #endregion

    private void OnEnable() {
        NetworkEventBus.Subscribe<ItemSpawnedEvent>(onItemSpawn);
        NetworkEventBus.Subscribe<ItemPickedUpEvent>(onItemPickup);
        NetworkEventBus.Subscribe<ItemDroppedOffEvent>(onItemDroppedOff);

    }

    private void OnDisable() {
        NetworkEventBus.Unsubscribe<ItemSpawnedEvent>(onItemSpawn);
        NetworkEventBus.Unsubscribe<ItemPickedUpEvent>(onItemPickup);
        NetworkEventBus.Unsubscribe<ItemDroppedOffEvent>(onItemDroppedOff);
    }

    private void onItemSpawn(ItemSpawnedEvent itemSpawnedEvent) {
        Debug.Log("Item spawned received by the server.");
    }

    private void onItemPickup(ItemPickedUpEvent itemPickedUpEvent) {
        Debug.Log("Item picked up received by the server.");
    }

    private void onItemDroppedOff(ItemDroppedOffEvent itemDroppedOffEvent) {
        Debug.Log("Item dropped off received by the server.");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Item") && leftoverCapacity > 0) {
            Item item = other.gameObject.GetComponent<Item>();
            AddItem(item);

            ItemPickedUpEvent itemPickedUpEvent = new ItemPickedUpEvent();
            itemPickedUpEvent.itemGuid = item.GetComponent<NetworkTransform>().key;
            itemPickedUpEvent.source = GetComponent<NetworkTransform>().Key;
            itemPickedUpEvent.inventorySize = items.Count;
            NetworkEventBus.Raise(itemPickedUpEvent);

            item.PickUp();
            NetworkTransform.Transforms.Remove(item.GetComponent<NetworkTransform>().key);

            Debug.Log("Item picked up");
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("PowerUp") && powerUp == null) {
            PowerUp powerUp = other.gameObject.GetComponent<PowerUp>();
            AddPowerUp(powerUp);
        }
    }
}