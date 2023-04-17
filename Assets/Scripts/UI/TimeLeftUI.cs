using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLeftUI : MonoBehaviour {

    [SerializeField] private TMPro.TMP_Text timeLeftText;

    private void OnEnable() {
        EventBus<TimeUpdateEvent>.Subscribe(onTimeUpdate);
    }

    private void OnDisable() {
        EventBus<TimeUpdateEvent>.Unsubscribe(onTimeUpdate);
    }

    private void onTimeUpdate(TimeUpdateEvent e) {
        timeLeftText.text = "Time left: " + e.time.ToString("0") + " S";
    }


}
