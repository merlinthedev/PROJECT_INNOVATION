using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {


    public enum Tier {
        ONE, TWO, THREE
    }

    [SerializeField] private Tier tier;
    [SerializeField] private float[] spawnRates = new float[] { 15, 30, 45 };


    void Start() {
        StartCoroutine(spawnItem());
    }

    // Update is called once per frame
    void Update() {

    }

    private IEnumerator spawnItem() {
        while (true) {

            yield return new WaitForSeconds(spawnRates[(int)tier]);
        }
    }
}
