using UnityEngine;

[CreateAssetMenu(fileName = "New Spawner Configuration", menuName = "Spawner Configuration")]
public class SpawnerConfiguration : ScriptableObject {

    public AInteractable[] prefabs;
    public AInteractable GetRandomPrefab() {
        return prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
    }

    public float spawnDelay;
}
