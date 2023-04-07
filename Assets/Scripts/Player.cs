using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour {

    private float score = 0f;
    public bool IsSafeToLeave { get; set; } = false;

    private void Start() {
        this.itemHolder = this.gameObject.transform;
    }


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
        foreach (var item in items) {
            // drop them back into the world
            item.transform.SetParent(null);
            // make sure to set the transform to next to the player but not to the point where we pick it up
            item.transform.position = new UnityEngine.Vector3(transform.position.x + 10f, transform.position.y + 1f, transform.position.z);
            item.gameObject.SetActive(true);
        }

        items.Clear();

        NetworkEventBus.Raise(new ItemDroppedOffEvent {
            source = GetComponent<NetworkTransform>().Key,
        });
    }

    public IEnumerator ResetSafeToLeaveFlag() {
        yield return new WaitForSeconds(2f);
        IsSafeToLeave = false;
    }

    public void DropOffItems() {
        foreach (var item in items) {
            score += (item.itemStats.discount > 0 ? ((float)item.itemStats.Tier + 1) * (item.itemStats.discount * 100) : ((float)item.itemStats.Tier + 1));

            Destroy(item.gameObject);
        }

        NetworkEventBus.Raise(new ScoreUpdatedEvent {
            source = GetComponent<NetworkTransform>().Key,
            score = score,
        });

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