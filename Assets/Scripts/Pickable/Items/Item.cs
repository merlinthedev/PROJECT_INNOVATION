using System;
using UnityEngine;
using System.Collections.Generic;

public class Item : AInteractable {

    public static Dictionary<Guid, Item> Items = new Dictionary<Guid, Item>();

    public Storezone Storezone;

    public ItemStats ItemStats;
    public int Weight { get { return ItemStats.Weight; } }

    // Value between 0 and 1 (0 = 0%, 1 = 100%)
    public float discount = 0.1f;

    private float initialY;
    public bool PaidFor = false;

    protected override void OnPickUp() {

    }

    private void Start() {
        initialY = transform.position.y;
    }

    private void Update() {
        //transform.position = new UnityEngine.Vector3(transform.position.x, initialY + Mathf.Sin(Time.time * 2f) * 0.1f, transform.position.z);
    }
}