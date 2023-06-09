using System.Collections.Generic;
using UnityEngine;
using System;


public class InventoryUIHandler : MonoBehaviour {

    public List<ItemUI> itemUIs = new List<ItemUI>();

    private void OnEnable() {
        EventBus<InventoryUIEvent>.Subscribe(onInventoryUpdatedEvent);
    }

    private void OnDisable() {
        EventBus<InventoryUIEvent>.Unsubscribe(onInventoryUpdatedEvent);
    }


    private void onInventoryUpdatedEvent(InventoryUIEvent inventoryUIEvent) {
        switch (inventoryUIEvent.actionType) {
            case InventoryUIEvent.ActionType.Add:
                addItem(inventoryUIEvent.item, inventoryUIEvent.discount, inventoryUIEvent.itemGuid);
                break;
            case InventoryUIEvent.ActionType.Remove:
                removeItem(inventoryUIEvent.itemGuid);
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

    private void addItem(Item item, float discount, Guid itemGuid) {
        // add item to first empty slot
        foreach (ItemUI itemUI in itemUIs) {
            if (!itemUI.HasItem) {
                itemUI.SetItem(item, discount, itemGuid);
                break;
            }
        }
    }

    private void removeItem(Guid itemGuid) {
        foreach (ItemUI itemUI in itemUIs) {
            if (itemUI.HasItem && itemUI.ItemGuid == itemGuid) {
                itemUI.RemoveItem();
                break;
            }
        }
    }

}
