using UnityEngine;
using System.Collections.Generic;

public class HideOnSpecificStates : MonoBehaviour {

    [SerializeField] private List<string> statesToHideOn = new List<string>();
    [SerializeField] private GameObject goBackButton;

    private void OnEnable() {
        EventBus<OnStateEnter>.Subscribe(onStateEnter);
    }

    private void OnDisable() {
        EventBus<OnStateEnter>.Unsubscribe(onStateEnter);
    }

    private void Start() {
        Debug.Log("HideOnSpecificStates.Start() called, state to check: " + GameManager.Instance.GetCurrentStateString());
        if (statesToHideOn.Contains(GameManager.Instance.GetCurrentStateString())) {
            goBackButton.SetActive(false);
        } else {
            goBackButton.SetActive(true);
        }
    }

    private void onStateEnter(OnStateEnter onStateEnterEvent) {
        Debug.Log("HideOnSpecificStates.onStateEnter() called, state to check: " + onStateEnterEvent.stateToEnter);
        if (statesToHideOn.Contains(onStateEnterEvent.stateToEnter)) {
            Debug.Log("HideOnSpecificStates.onStateEnter() called, state to check: " + onStateEnterEvent.stateToEnter + " is in statesToHideOn list, setting gameobject to inactive");
            goBackButton.SetActive(false);
        } else {
            Debug.Log("HideOnSpecificStates.onStateEnter() called, state to check: " + onStateEnterEvent.stateToEnter + " is NOT in statesToHideOn list, setting gameobject to active");
            goBackButton.SetActive(true);
        }
    }
}
