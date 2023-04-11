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

    public int GetPrefabIndex(AInteractable interactable) {
        for (int i = 0; i < interactables.Length; i++) {
            if (interactables[i].serverPrefab == interactable) {
                return i;
            }
        }

        Debug.LogError("Could not find interactable in configuration", interactable);
        return -1;
    }
}
