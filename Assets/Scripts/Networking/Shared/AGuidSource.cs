using System;
using UnityEngine;

public abstract class AGuidSource : MonoBehaviour {
    [SerializeField] protected string keyString = Guid.NewGuid().ToString();
    public Guid key { get => Guid.Parse(keyString); set => keyString = value.ToString(); }
    public Guid Key => key;

    public void NewKey() {
        key = Guid.NewGuid();
    }

    public void SetKey(Guid newKey) {
        key = newKey;
    }

    public void SetKey(string newKey) {
        key = new Guid(newKey);
    }
}
