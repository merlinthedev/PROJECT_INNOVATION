using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerup : PowerUp {
    [SerializeField] private float boostAmount;
    [SerializeField] private float boostDuration;
    

    protected override void OnPickUp() {
    }

    protected override void OnUse(Player player) {
        var movement = player.Movement;
        movement.Boost(boostAmount, boostDuration);
    }
}
