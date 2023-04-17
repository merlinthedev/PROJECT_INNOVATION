using TMPro;
using UnityEngine;

public class InventoryItemUIHandler : MonoBehaviour {
    [SerializeField] private TMP_Text discountText;

    private void OnEnable() {
        NetworkEventBus.Subscribe<UIDiscountUpdateEvent>(onDiscountUpdate);
    }

    private void OnDisable() {
        NetworkEventBus.Unsubscribe<UIDiscountUpdateEvent>(onDiscountUpdate);
    }

    private void onDiscountUpdate(UIDiscountUpdateEvent x) {
        if (x.source != GameClient.getInstance().GetGuid()) {
            Debug.Log("NOT OUR EVENT DONT GO PEEKING FELLA");
            return;
        }

        Debug.Log(" SETTING DISCOUNT TO NEW DISCOUNT: " + x.newDiscount);
        SetDiscount(x.newDiscount);
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

        Debug.Log("Discount set");
    }
}
