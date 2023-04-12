using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupUIHandler : MonoBehaviour {
    [SerializeField] private Image powerupSpriteTarget;
    [SerializeField] private InteractableConfiguration interactableConfig;

    private void OnEnable() {
        EventBus<PowerUpUIEvent>.Subscribe(PowerUpEvent);
    }

    private void OnDisable() {
        EventBus<PowerUpUIEvent>.Unsubscribe(PowerUpEvent);
    }

    void PowerUpEvent(PowerUpUIEvent powerUpUIEvent) {
        Debug.Log($"Item ID: {powerUpUIEvent.PowerUpID}");
        if (powerUpUIEvent.PowerUpID >= 0 && powerUpUIEvent.PowerUpID < interactableConfig.interactables.Length) {
            PowerUp powerUp = interactableConfig.interactables[powerUpUIEvent.PowerUpID].serverPrefab as PowerUp;
            if (powerUp != null) {
                SetPowerUp(powerUp);
            } else {
                RemovePowerUp();
            }
        } else {
            RemovePowerUp();
        }
    }

    public void SetPowerUp(PowerUp powerUp) {
        powerupSpriteTarget.sprite = powerUp.PowerUpSprite;
        powerupSpriteTarget.enabled = true;
    }

    public void RemovePowerUp() {
        powerupSpriteTarget.sprite = null;
        powerupSpriteTarget.enabled = false;
    }
}
