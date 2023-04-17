using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Player : AGuidListener {

    private float score = 0f;
    public bool IsSafeToLeave = false;

    private void Awake() {
        this.itemHolder = this.gameObject.transform;

        // find mask transform which is a child of server.TCPGameServer.Instance.worldToMinimapHelper.gameObject
        var maskTransform = server.TCPGameServer.Instance.worldToMinimapHelper.gameObject.transform.Find("Mask");

        // add ourselves to the minimap
        var returned = Instantiate(server.TCPGameServer.Instance.playerMinimapPrefab, new Vector3(0, 0, 0), Quaternion.identity, maskTransform);

        if (returned.GetComponent<PlayerMinimapComponent>() == null) {
            Debug.LogError("PlayerMinimapComponent is null");
            return;
        }

        playerMinimapComponent = returned.GetComponent<PlayerMinimapComponent>();

    }

    #region UI
    public PlayerMinimapComponent playerMinimapComponent { get; private set; }
    public UnityEngine.Color playerColor { get; set; }
    #endregion

    #region Movement
    [SerializeField] private ShoppingCartMovement movement;
    public ShoppingCartMovement Movement { get => movement; }
    #endregion

    #region Items
    [SerializeField] private Transform itemHolder;
    [SerializeField] private int capacity = 2;
    [SerializeField] private List<Item> items = new List<Item>();
    public DropoffZone dropoffZone = null;
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
        itemsDiscardedEvent.source = key;

        for (int i = items.Count - 1; i >= 0; i--) {
            if (items[i].PaidFor) continue;
            //// drop them back into the world
            //items[i].transform.SetParent(null);
            //// make sure to set the transform to next to the player but not to the point where we pick it up
            //items[i].transform.position = items[i].Storezone.transform.position + new UnityEngine.Vector3(1f, 1f, 0f);
            //items[i].gameObject.SetActive(true);

            itemsDiscardedEvent.discardedItems.Add(items[i].GetComponent<NetworkTransform>().key);
            Item.Items.Remove(items[i].GetComponent<NetworkTransform>().key);
            Destroy(items[i].gameObject);
            items.Remove(items[i]);


        }

        // items.Clear();
        if (itemsDiscardedEvent.discardedItems.Count > 0) NetworkEventBus.Raise(itemsDiscardedEvent);

        NetworkEventBus.Raise(new PlayOneShotEvent {
            source = key,
            audioClipID = 3,
            position = transform.position,
            onServer = false,
        });
    }

    public IEnumerator ResetSafeToLeaveFlag() {
        yield return new WaitForSeconds(2f);
        IsSafeToLeave = false;
    }

    public void FlagSafeItems() {

        ItemsPaidForEvent itemsPaidForEvent = new ItemsPaidForEvent();
        itemsPaidForEvent.source = key;

        foreach (var item in items) {
            item.PaidFor = true;
            itemsPaidForEvent.itemGuids.Add(item.key);
        }

        NetworkEventBus.Raise(itemsPaidForEvent);

        NetworkEventBus.Raise(new PlayOneShotEvent {
            source = key,
            audioClipID = 4,
            position = transform.position,
            onServer = false,

        });
    }

    public void DropOffItems() {
        ItemsDroppedOffEvent itemDroppedOffEvent = new ItemsDroppedOffEvent();
        itemDroppedOffEvent.source = key;

        foreach (var item in items) {
            score += (item.discount > 0 ? ((float)item.ItemStats.Tier + 1) * (item.discount * 100) : ((float)item.ItemStats.Tier + 1));
            itemDroppedOffEvent.droppedItems.Add(item.GetComponent<NetworkTransform>().key);
            Destroy(item.gameObject);
        }

        NetworkEventBus.Raise(new ScoreUpdatedEvent {
            source = key,
            score = score,
        });

        items.Clear();

        NetworkEventBus.Raise(itemDroppedOffEvent);
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

            PowerupUsedEvent powerUpUsedEvent = new PowerupUsedEvent();
            powerUpUsedEvent.source = key;

            NetworkEventBus.Raise(powerUpUsedEvent);

            NetworkEventBus.Raise(new PlayOneShotEvent {
                source = key,
                audioClipID = 1,
                position = transform.position,
                onServer = false
            });
        }
    }

    #endregion

    private void OnTriggerEnter(Collider other) {
        //items
        if (other.gameObject.CompareTag("Item") && leftoverCapacity > 0) {
            Item item = other.gameObject.GetComponent<Item>();
            AddItem(item);

            ItemPickedUpEvent itemPickedUpEvent = new ItemPickedUpEvent();
            itemPickedUpEvent.itemGuid = item.GetComponent<NetworkTransform>().key;
            itemPickedUpEvent.itemInteractableID = item.InteractableID;
            itemPickedUpEvent.source = key;
            itemPickedUpEvent.shouldClear = false;
            itemPickedUpEvent.discount = item.discount;
            NetworkEventBus.Raise(itemPickedUpEvent);

            item.PickUp();
            // NetworkTransform.Transforms.Remove(item.GetComponent<NetworkTransform>().key);

            Debug.Log("Item picked up");
            other.gameObject.SetActive(false);
        }

        //powerups
        if (other.gameObject.CompareTag("PowerUp") && powerUp == null) {
            PowerUp powerUp = other.gameObject.GetComponent<PowerUp>();
            AddPowerUp(powerUp);

            PowerUpPickedUpEvent powerUpPickedUpEvent = new PowerUpPickedUpEvent();
            powerUpPickedUpEvent.powerUpGuid = powerUp.GetComponent<NetworkTransform>().key;
            powerUpPickedUpEvent.source = key;
            powerUpPickedUpEvent.PowerUpID = powerUp.InteractableID;
            NetworkEventBus.Raise(powerUpPickedUpEvent);

            powerUp.PickUp();
            other.gameObject.SetActive(false);
        }

        //hazards
        if (other.gameObject.CompareTag("Hazard")) {
            Hazard hazard = other.gameObject.GetComponent<Hazard>();
            hazard.Activate(this);
        }
    }

    public void ApplyCoupon(float discountMultiplier) {
        //apply discount to all items
        foreach (var item in items) {
            item.discount *= discountMultiplier;

            UIDiscountUpdateEvent uiDiscountUpdateEvent = new UIDiscountUpdateEvent();
            uiDiscountUpdateEvent.source = key;
            uiDiscountUpdateEvent.itemGuid = item.key;
            uiDiscountUpdateEvent.newDiscount = item.discount;
            NetworkEventBus.Raise(uiDiscountUpdateEvent);
        }
    }


    /*
    * Getters and Setters
    */

    public float GetScore() {
        return this.score;
    }

    public void SetScore(float score) {
        this.score = score;
    }
}