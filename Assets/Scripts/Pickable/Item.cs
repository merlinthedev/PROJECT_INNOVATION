using System.Numerics;
using UnityEngine;
using System.Collections.Generic;

public class Item : AInteractable {

    public static List<Item> Items = new List<Item>();

    public ItemStats itemStats;
    public int Weight { get { return itemStats.Weight; } }

    private float initialY;

    

    protected override void OnPickUp() {

    }

    private void Start() {
        initialY = transform.position.y;
    }

    private void Update() {
        transform.position = new UnityEngine.Vector3(transform.position.x, initialY + Mathf.Sin(Time.time * 2f) * 0.1f, transform.position.z);
    }
}