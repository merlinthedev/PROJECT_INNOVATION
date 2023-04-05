using Adobe.Substance;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTransform : AGuidSource {
    public static readonly Dictionary<Guid, NetworkTransform> Transforms = new Dictionary<Guid, NetworkTransform>();
    public static readonly HashSet<NetworkTransform> UpdatedTransforms = new HashSet<NetworkTransform>();
    
    bool initialized = false;

    [Tooltip("Whether the transform is being updated from incoming packets")]
    public bool kinematic = false;
    public bool HasChange { get; set; }
    public bool hasPacket => transformPacket != null;
    private TransformPacket transformPacket;
    public TransformPacket GetPacket() {
        return transformPacket;
    }

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    [SerializeField] private SmoothingType smoothingType = SmoothingType.Lerp;
    [SerializeField] private float smoothSpeed = 25f;



    // Start is called before the first frame update
    void Awake() {
        UpdateTransformPacket();
    }

    public void Initialize() {
        UpdateTransformPacket();
        kinematic = !NetworkManager.IsServer;
        initialized = true;
        Transforms.Add(key, this);

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
            UpdateTransformPacket();
            UpdatedTransforms.Add(this);
        }

        if (kinematic) {
            switch (smoothingType) {
                case SmoothingType.Lerp:
                    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
                    break;
                case SmoothingType.SmoothDamp:
                    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref targetPosition, Time.deltaTime * smoothSpeed);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
                    break;
                case SmoothingType.None:
                    transform.position = targetPosition;
                    transform.rotation = targetRotation;
                    break;
            }
        }
    }

    void UpdateTransformPacket() {
        if (transformPacket == null)
            transformPacket = new TransformPacket();
        transformPacket.SetTransform(transform);
        transformPacket.guid = key;
    }
    
    public void UpdateTransform(TransformPacket newTransform) {
        if (kinematic) {
            targetPosition = new Vector3(newTransform.transformData[0], newTransform.transformData[1], newTransform.transformData[2]);
            targetRotation = Quaternion.Euler(newTransform.transformData[3], newTransform.transformData[4], newTransform.transformData[5]);
        }
    }

    private void OnDestroy() {
        Transforms.Remove(key);
    }

    public enum SmoothingType {
        None,
        Lerp,
        SmoothDamp
    }
}
