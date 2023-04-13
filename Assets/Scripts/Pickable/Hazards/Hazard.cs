using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hazard : AInteractable {

    public void Activate(Player player) {
        OnActivate(player);
    }

    protected abstract void OnActivate(Player player);
}
