using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryUIHandler : MonoBehaviour {

    private List<GameObject> items = new List<GameObject>();

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private RectTransform childTransform;
    [SerializeField] private Texture2D itemTexture;


    private void OnEnable() {
        EventBus<InventoryUIEvent>.Subscribe(onInventoryUpdatedEvent);
    }

    private void OnDisable() {
        EventBus<InventoryUIEvent>.Unsubscribe(onInventoryUpdatedEvent);
    }


    private void onInventoryUpdatedEvent(InventoryUIEvent inventoryUIEvent) {
        if (inventoryUIEvent.shouldClear) {
            foreach (var existingItem in items) {
                Destroy(existingItem);
            }
            items.Clear();

            return;
        }

        var item = Instantiate(itemPrefab, childTransform);
        item.GetComponent<Image>().sprite = Sprite.Create(itemTexture, new Rect(0, 0, itemTexture.width, itemTexture.height), Vector2.zero);
        items.Add(item);

        // visualize the item in the UI inventory, if there is one already, add it next to it
        var itemRectTransform = item.GetComponent<RectTransform>();
        itemRectTransform.anchoredPosition = new Vector2(-300 + (items.Count * 100), 0);

        // for the new one, get the InventoryItemUIHandler and set the discount only for the new item
        var itemUIHandler = item.GetComponentInChildren<InventoryItemUIHandler>();
        itemUIHandler.SetDiscount(inventoryUIEvent.discount);


    }

}
