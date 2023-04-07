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
        
    }

}
