using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractableConfiguration", menuName = "InteractableConfiguration", order = 0)]
public class InteractableConfiguration : ScriptableObject
{
    [Serializable]
    public class InteractablePair {
        public string itemName;
        public AInteractable serverPrefab;
        public NetworkTransform clientPrefab;
    }

    public InteractablePair[] interactables;
}
