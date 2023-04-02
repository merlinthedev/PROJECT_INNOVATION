using Adobe.Substance;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTransform : MonoBehaviour {
    public static readonly Dictionary<Guid, NetworkTransform> Transforms = new Dictionary<Guid, NetworkTransform>();
    public static readonly List<NetworkTransform> UpdatedTransforms = new List<NetworkTransform>();

    public Guid key = Guid.NewGuid();
    bool initialized = false;

    [Tooltip("Whether the transform is being updated from incoming packets")]
    public bool kinematic = false;
    public bool hasPacket => transformPacket != null;
    private TransformPacket transformPacket;
    public TransformPacket GetPacket() {
        var packet = transformPacket;
        transformPacket = null;
        return packet;
    }

    // Start is called before the first frame update
    void Start() {
    }

    public void Initialize() {
        Transforms.Add(key, this);
        initialized = true;

        if (kinematic) {
            var rb = GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = true;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (!initialized)
            Initialize();
        if (!kinematic && transform.hasChanged) {
            if (transformPacket == null) {
                UpdatedTransforms.Add(this);
            }
            transformPacket = new TransformPacket();
            transformPacket.SetTransform(transform);
            transformPacket.guid = key;
        }
    }

    public void UpdateTransform(TransformPacket newTransform) {
        if (kinematic) {
            transform.position = new Vector3(newTransform.transformData[0], newTransform.transformData[1], newTransform.transformData[2]);
            transform.rotation = Quaternion.Euler(newTransform.transformData[3], newTransform.transformData[4], newTransform.transformData[5]);
        }
    }

    private void OnDestroy() {
        Transforms.Remove(key);
    }
}
