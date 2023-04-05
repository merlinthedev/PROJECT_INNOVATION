using System.Numerics;
using UnityEditor;
using UnityEngine;

public class Item : AInteractable {
    [SerializeField] private ItemStats itemStats;
    public int Weight { get { return itemStats.Weight; } }

    protected override void OnPickUp() {
        
    }

    private void Start() {
    }

    private void Update() {
        
    }
}