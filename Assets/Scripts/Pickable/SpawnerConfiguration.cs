using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnerConfiguration : ScriptableObject {

    public AInteractable[] prefabs;
    public AInteractable GetRandomPrefab() {
        return prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
    }

    public float spawnDelay;
}
