using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private int capacity = 2;
    [SerializeField] private List<Item> items = new List<Item>();
    //current capacity we have
    public int leftoverCapacity { get { return capacity - items.Sum(x => x.Weight); } }

    public List<Item> GetInventory { get { return new List<Item>(items); } }

    public void AddItem(Item item) {
        items.Add(item);
    }

    public void RemoveItem(Item item) {
        items.Remove(item);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Item")) {
            Item item = other.gameObject.GetComponent<Item>();
            AddItem(item);
            Destroy(other.gameObject);
        }
    }
}