using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperyHazard : Hazard
{
    [SerializeField] private float slipperyTime = 5f;

    protected override void OnActivate(Player player) {
        player.Movement.Slip(slipperyTime);
    }

    protected override void OnPickUp() {
    }
}
