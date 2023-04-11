using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour {

    private float score = 0f;
    public bool IsSafeToLeave = false;

    private void Start() {
        this.itemHolder = this.gameObject.transform;
    }

    #region Movement
    [SerializeField] private ShoppingCartMovement movement;
    public ShoppingCartMovement Movement { get; }
    #endregion

    #region Items
    [SerializeField] private Transform itemHolder;
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

    public void DiscardItems() {
        ItemsDiscardedEvent itemsDiscardedEvent = new ItemsDiscardedEvent();
        itemsDiscardedEvent.source = GetComponent<NetworkTransform>().Key;

        foreach (var item in items) {
            // drop them back into the world
            item.transform.SetParent(null);
            // make sure to set the transform to next to the player but not to the point where we pick it up
            item.transform.position = item.Storezone.transform.position + new UnityEngine.Vector3(0f, 1f, 0f);
            item.gameObject.SetActive(true);

            itemsDiscardedEvent.discardedItems.Add(item.GetComponent<NetworkTransform>().key);
        }

        items.Clear();

        NetworkEventBus.Raise(itemsDiscardedEvent);
    }

    public IEnumerator ResetSafeToLeaveFlag() {
        yield return new WaitForSeconds(2f);
        IsSafeToLeave = false;
    }

    public void DropOffItems() {
        ItemsDroppedOffEvent itemDroppedOffEvent = new ItemsDroppedOffEvent();
        itemDroppedOffEvent.source = GetComponent<NetworkTransform>().Key;

        foreach (var item in items) {
            score += (item.discount > 0 ? ((float)item.ItemStats.Tier + 1) * (item.discount * 100) : ((float)item.ItemStats.Tier + 1));
            itemDroppedOffEvent.droppedItems.Add(item.GetComponent<NetworkTransform>().key);
            Destroy(item.gameObject);
        }

        NetworkEventBus.Raise(new ScoreUpdatedEvent {
            source = GetComponent<NetworkTransform>().Key,
            score = score,
        });

        items.Clear();

        NetworkEventBus.Raise(itemDroppedOffEvent);

        // destroy networktransform 
        NetworkTransform.Transforms.Remove(GetComponent<NetworkTransform>().Key);
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

    public void UsePowerUp() {
        if (powerUp != null) {
            powerUp.Use(this);
            RemovePowerUp();
        }
    }

    #endregion

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Item") && leftoverCapacity > 0) {
            Item item = other.gameObject.GetComponent<Item>();
            AddItem(item);

            ItemPickedUpEvent itemPickedUpEvent = new ItemPickedUpEvent();
            itemPickedUpEvent.itemGuid = item.GetComponent<NetworkTransform>().key;
            itemPickedUpEvent.source = GetComponent<NetworkTransform>().Key;
            itemPickedUpEvent.shouldClear = false;
            itemPickedUpEvent.discount = item.discount;
            NetworkEventBus.Raise(itemPickedUpEvent);

            item.PickUp();
            // NetworkTransform.Transforms.Remove(item.GetComponent<NetworkTransform>().key);

            Debug.Log("Item picked up");
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("PowerUp") && powerUp == null) {
            PowerUp powerUp = other.gameObject.GetComponent<PowerUp>();
            AddPowerUp(powerUp);

            PowerUpPickedUpEvent powerUpPickedUpEvent = new PowerUpPickedUpEvent();
            powerUpPickedUpEvent.powerUpGuid = powerUp.GetComponent<NetworkTransform>().key;
            powerUpPickedUpEvent.source = GetComponent<NetworkTransform>().Key;
            powerUpPickedUpEvent.PowerUpID = powerUp.InteractableID;
            NetworkEventBus.Raise(powerUpPickedUpEvent);

            powerUp.PickUp();
            other.gameObject.SetActive(false);
        }
    }

}