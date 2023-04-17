using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CouponPowerup : PowerUp {
    [Range(1f, 2f)]
    [SerializeField] private float discountMultiplier = 1.1f;
    
    protected override void OnPickUp() {
    }

    protected override void OnUse(Player player) {
        player.ApplyCoupon(discountMultiplier);
    }
}
