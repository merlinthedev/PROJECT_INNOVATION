using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
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

    #endregion

    #region PowerUps
    //we can only hold one powerup at a time
    [SerializeField] Transform powerUpHolder;
    [
    [SerializeField] PowerUp powerUp;
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
        }

        if (other.gameObject.CompareTag("PowerUp") && powerUp == null) {
            PowerUp powerUp = other.gameObject.GetComponent<PowerUp>();
            AddPowerUp(powerUp);
        }
    }
}