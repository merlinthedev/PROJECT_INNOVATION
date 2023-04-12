using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour {
    [SerializeField] private Image defaultImage;
    [SerializeField] private TMP_Text discountText;

    public System.Guid Key { get; set; }

    public void SetItem(Item item) {
        defaultImage.sprite = item.ItemStats.ItemSprite;
        defaultImage.enabled = true;
        discountText.text = item.discount.ToString();

        Key = item.key;
    }

    public void RemoveItem() {
        defaultImage.sprite = null;
        defaultImage.enabled = false;
        discountText.text = "";

        Key = System.Guid.Empty;
    }

}
