using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ItemUI : MonoBehaviour {
    [SerializeField] private Image defaultImage;
    [SerializeField] private TMP_Text discountText;

    public bool HasItem = false;
    public Guid ItemGuid { get; private set; }

    public void Start() {
        RemoveItem();
    }

    public void SetItem(Item item, Guid itemGuid) {
        if (item == null) {
            Debug.LogError("Item is null");
            return;
        }
        defaultImage.sprite = item.ItemStats.ItemSprite;
        defaultImage.enabled = true;
        discountText.text = item.discount.ToString();

        HasItem = true;
        ItemGuid = itemGuid;
    }

    public void RemoveItem() {
        defaultImage.sprite = null;
        defaultImage.enabled = false;
        discountText.text = "";

        HasItem = false;
        ItemGuid = Guid.Empty;
    }

}
