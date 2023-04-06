using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public bool isPressed { get; private set; }

    public void OnPointerDown(PointerEventData _) {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData _) {
        isPressed = false;
    }
}