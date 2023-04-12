using System.Collections.Generic;
using UnityEngine;


public class InventoryUIHandler : MonoBehaviour {

    private List<ItemUI> itemUIs = new List<ItemUI>();

    private void OnEnable() {
        EventBus<InventoryUIEvent>.Subscribe(onInventoryUpdatedEvent);
    }

    private void OnDisable() {
        EventBus<InventoryUIEvent>.Unsubscribe(onInventoryUpdatedEvent);
    }


    private void onInventoryUpdatedEvent(InventoryUIEvent inventoryUIEvent) {
        switch (inventoryUIEvent.actionType) {
            case InventoryUIEvent.ActionType.Add:
                addItem(inventoryUIEvent.item);
                break;
            case InventoryUIEvent.ActionType.Remove:
                removeItem(inventoryUIEvent.item);
                break;
            case InventoryUIEvent.ActionType.Clear:
                clearInventory();
                break;
        }



    }

    private void clearInventory() {
        foreach (ItemUI itemUI in itemUIs) {
            itemUI.RemoveItem();
        }
    }

    private void addItem(Item item) {
        // add item to first empty slot

        if (itemUIs.Count == 2) {
            Debug.LogError("Inventory is full");
            return;
        }

        foreach (ItemUI itemUI in itemUIs) {
            if (itemUI.Key == System.Guid.Empty) {
                itemUI.SetItem(item);
                break;
            }
        }
    }

    private void removeItem(Item item) {
        foreach (ItemUI itemUI in itemUIs) {
            if (itemUI.Key == item.key) {
                itemUI.RemoveItem();
                break;
            }
        }
    }

}
