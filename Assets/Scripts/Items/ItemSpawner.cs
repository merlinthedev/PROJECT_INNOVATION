using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {


    public enum Tier {
        ONE, TWO, THREE
    }

    [SerializeField] private Tier tier;
    [SerializeField] private float[] spawnRates = new float[] { 15, 30, 45 };

    private bool hasItem = true;

    [SerializeField] private GameObject itemPrefab;


    void Start() {
        Instantiate(itemPrefab, transform.position, Quaternion.identity);
        StartCoroutine(spawnItem());
    }

    private void Update() {

    }

    private IEnumerator spawnItem() {
        while (true) {
            yield return new WaitForSeconds(spawnRates[(int)tier]);
            if (!hasItem) {
                Instantiate(itemPrefab, transform.position, Quaternion.identity);
                hasItem = true;
            }
        }
    }
}
