using UnityEngine;
using UnityEngine.UI;

public class PlayerMinimapComponent : MonoBehaviour {

    [SerializeField] private RectTransform playerMinimapImageTransform;

    public void UpdatePosition(Vector3 playerPosition) {
        playerMinimapImageTransform.anchoredPosition = new Vector3(playerPosition.x, playerPosition.y - 500, playerPosition.z);
    }
}
