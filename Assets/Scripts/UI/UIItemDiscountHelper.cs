using TMPro;
using UnityEngine;

public class UIItemDiscountHelper : MonoBehaviour {

    [SerializeField] private NetworkTransform networkTransform;
    [SerializeField] private TMP_Text discountText;

    private void OnEnable() {
        NetworkEventBus.Subscribe<ItemDiscountUpdateEvent>(onItemDiscountUpdateEvent);
    }

    private void OnDisable() {
        NetworkEventBus.Unsubscribe<ItemDiscountUpdateEvent>(onItemDiscountUpdateEvent);
    }

    public void SetDiscount(float discount) {
        discount *= 100;
        // round to 0
        discount = Mathf.Round(discount);
        discountText.text = discount.ToString();

        switch (discount) {
            case float n when n < 33f:
                discountText.color = Color.green;
                break;
            case float n when n < 67f:
                discountText.color = Color.yellow;
                break;
            case float n when n < 100f:
                discountText.color = Color.red;
                break;
        }
    }

    private void onItemDiscountUpdateEvent(ItemDiscountUpdateEvent itemDiscountUpdateEvent) {
        if (itemDiscountUpdateEvent.influencedItems.Contains(networkTransform.Key)) {
            return;
        }

        SetDiscount(itemDiscountUpdateEvent.discount);
    }
}
