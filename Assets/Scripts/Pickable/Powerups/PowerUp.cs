using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class PowerUp : AInteractable {
    public Sprite PowerUpSprite;
    
    public void Use(Player player) {
        OnUse(player);
    }
    
    protected abstract void OnUse(Player player);
}
