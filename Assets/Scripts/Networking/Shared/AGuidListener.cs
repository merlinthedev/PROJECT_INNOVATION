using System;
using UnityEngine;

public abstract class AGuidListener : MonoBehaviour {
    public AGuidSource guidSource;
    public Guid key => guidSource.key;
}
