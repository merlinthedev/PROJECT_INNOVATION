using System.Numerics;
using UnityEngine;
using System.Collections.Generic;

public class Item : AInteractable {

    public static List<Item> Items = new List<Item>();

    public Storezone Storezone;

    public ItemStats ItemStats;
    public int Weight { get { return ItemStats.Weight; } }
    
    // Value between 0 and 1 (0 = 0%, 1 = 100%)
    public float discount = 0.1f;

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