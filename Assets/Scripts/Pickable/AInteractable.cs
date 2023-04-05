using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AInteractable : MonoBehaviour {
    public Spawner spawner { get; set; }
    public void PickUp() {
        OnPickUp();
        spawner?.OnPickup(this);
        spawner = null;
    }
    
    protected abstract void OnPickUp();
    
}
