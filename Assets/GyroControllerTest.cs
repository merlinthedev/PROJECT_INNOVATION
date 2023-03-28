using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GyroControllerTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //on gyro unity event
    public void OnGyro(InputAction.CallbackContext context) {
        Vector3 newGravity = context.ReadValue<Vector3>();
        transform.rotation = Quaternion.FromToRotation(Vector3.up, newGravity);
    }
}
