using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSScript : MonoBehaviour {

    private TMP_Text fpsText;

    // Start is called before the first frame update
    void Start() {
        fpsText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update() {
        fpsText.SetText("FPS: " + (1.0f / Time.deltaTime).ToString("0"));
    }
}
