using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupUIHandler : MonoBehaviour {
    [SerializeField] private Image powerupSpriteTarget;
    [SerializeField] private InteractableConfiguration interactableConfig;
    [SerializeField] private Sprite powerupPlaceholder;

    private void OnEnable() {
        EventBus<PowerUpUIEvent>.Subscribe(PowerUpEvent);
        powerupSpriteTarget.enabled = true;
        RemovePowerUp();
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
        Debug.Log("removepowerup");
        powerupSpriteTarget.sprite = powerupPlaceholder;
        //powerupSpriteTarget.enabled = false;
    }
}
