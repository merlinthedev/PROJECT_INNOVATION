using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectAutoDestroy : MonoBehaviour
{
    // The particle system on this object (if one exists)
    private ParticleSystem system;
 
    void Update () {
        // Try to extract a particle system from the specified root object (First time only)
        if (system == null) {
            system = GetComponent<ParticleSystem> ();
        }
 
        // Test whether the particle system should be destroyed now (Checks every frame)
        if (system != null && !system.IsAlive (true)) {
            Destroy (gameObject);
        }
    }
 
}
