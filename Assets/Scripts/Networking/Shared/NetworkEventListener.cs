using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// Listens to the NetworkEventBus for specific events, and invokes UnityEvents if these happen
/// </summary>
public class NetworkEventListener : AGuidListener
{
    [HideInInspector] public int networkEventIndex = 0;
    public Type networkEventType { get => NetworkEventBus.EventTypes[networkEventIndex]; }
    public UnityEvent onNetworkEvent;
    [SerializeField] private bool checkGuid = false;

    // Start is called before the first frame update
    void Start()
    {
        if (networkEventIndex >= NetworkEventBus.EventTypes.Length) {
            Debug.LogError("Network Event Index is out of range");
            return;
        }
        NetworkEventBus.SubscribeToType(networkEventType, onEvent);
    }
    
    private void onEvent (NetworkEvent netEvent) {
        if (!checkGuid || guidSource == null || netEvent.source == Key)
            onNetworkEvent.Invoke();
    }
}
