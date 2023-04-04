using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// Listens to the NetworkEventBus for specific events, and invokes UnityEvents if these happen
/// </summary>
public class NetworkEventListener : MonoBehaviour
{
    public Type networkEventType;
    public UnityEvent onNetworkEvent;
    

    // Start is called before the first frame update
    void Start()
    {
        NetworkEventBus.SubscribeToType(networkEventType, onEvent);
    }
    
    private void onEvent (NetworkEvent netEvent) {
        onNetworkEvent.Invoke();
    }
}
