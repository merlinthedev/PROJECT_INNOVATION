using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ItemUI : MonoBehaviour {
    [SerializeField] private Image defaultImage;
    [SerializeField] private TMP_Text discountText;

    public Sprite emptySprite;

    public bool HasItem = false;
    public Guid ItemGuid { get; private set; }

    public void Start() {
        RemoveItem();
    }

    public void SetItem(Item item, float discount, Guid itemGuid) {
        if (item == null) {
            Debug.LogError("Item is null");
            return;
        }
        defaultImage.sprite = item.ItemStats.ItemSprite;
        defaultImage.enabled = true;
        discountText.text = makeTextPretty(discount);

        HasItem = true;
        ItemGuid = itemGuid;
    }

    public void RemoveItem() {
        defaultImage.sprite = emptySprite;
        defaultImage.enabled = true;
        discountText.text = "";

        HasItem = false;
        ItemGuid = Guid.Empty;
    }

    private string makeTextPretty(float discount) {
        discount *= 100;
        discount = (float)Math.Round(discount, 0);

        switch (discount) {
            // when discount < 33
            case float n when (n < 33):
                discountText.color = Color.green;
                return discount.ToString();
            case float n when (n < 67):
                discountText.color = Color.yellow;
                return discount.ToString();
            case float n when (n < 100):
                discountText.color = Color.red;
                return discount.ToString();
            default:
                return discount.ToString();
        }
    }

}
