using UnityEngine;
using UnityEngine.UI;

public class PlayerMinimapComponent : MonoBehaviour {

    [SerializeField] private RectTransform playerMinimapImageTransform;
    [SerializeField] private Image playerMinimapImage;

    public void SetColor(UnityEngine.Color color) {
        playerMinimapImage.color = color;
    }

    public void UpdatePosition(Vector3 playerPosition) {
        playerMinimapImageTransform.anchoredPosition = new Vector3(playerPosition.x, playerPosition.y - 500, playerPosition.z);
    }

    public void PrepareForRemoval() {
        Destroy(gameObject);
    }
}
