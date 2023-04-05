using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AGuidSource : MonoBehaviour
{
    private Guid key;
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
