using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreDiscountDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public Gradient backgroundGradient;
    public Gradient progressGradient;
    [SerializeField] private Image backgroundMapImage;
    [SerializeField] private Image progressMapImage;

    public void UpdateColor(float discount, float duration) {
        backgroundMapImage.color = backgroundGradient.Evaluate(discount);
        progressMapImage.color = progressGradient.Evaluate(discount);

        LeanTween.value(gameObject, 0, 1, duration).setOnUpdate((float value) => {
            //empty image left to right
            progressMapImage.rectTransform.anchorMin = new Vector2(value, 0);
        });
    }
}
