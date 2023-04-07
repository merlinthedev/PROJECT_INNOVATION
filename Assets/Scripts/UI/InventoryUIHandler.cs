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
        Debug.Log("Populating inventory UI");
        foreach (var item in items) {
            Destroy(item);
        }

        items.Clear();

        Debug.Log("Inventory size: " + inventoryUIEvent.inventorySize);
        for (int i = 0; i < inventoryUIEvent.inventorySize; i++) {
            // create a new image object with the item sprite
            var newItem = Instantiate(itemPrefab, childTransform);
            Debug.Log("Instantiated where did it go ?XD");
            newItem.GetComponent<Image>().sprite = Sprite.Create(itemTexture, new Rect(0, 0, itemTexture.width, itemTexture.height), Vector2.zero);
            newItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(50 * i, 0);
            items.Add(newItem);

            
            // do we need to instantiate?
        }
    }

}
