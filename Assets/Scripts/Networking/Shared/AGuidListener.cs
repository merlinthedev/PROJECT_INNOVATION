using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AGuidListener : MonoBehaviour {
    public AGuidSource guidSource;
    public Guid key => guidSource.key;
}
