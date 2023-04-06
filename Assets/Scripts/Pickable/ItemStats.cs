using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ItemStats : ScriptableObject {

    public Spawner.Tier Tier; 
    public string ItemName;
    public Sprite ItemSprite;
    public GameObject GameObject;
    public int Weight = 1;

    // Value between 0 and 1 (0 = 0%, 1 = 100%)
    public float discount = 0.1f;

}
