using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreDiscountDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public Gradient storezoneGradient;
    [SerializeField] private Image mapImage;

    public void UpdateColor(float discount) {
        mapImage.color = storezoneGradient.Evaluate(discount);
    }
}
