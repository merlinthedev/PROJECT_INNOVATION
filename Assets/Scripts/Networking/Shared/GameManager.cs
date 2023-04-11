using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }
    [SerializeField] private List<GameState> gameStates = new List<GameState>();
    private GameState currentState;
    private GameState previousState;

    [Serializable]
    private class GameState {
        public string stateName;
        public List<GameObject> stateObjects = new List<GameObject>();
    }

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("There is already an existing GameManager present. Aborting instantiation.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        initialState();
    }

    private void initialState() {
        foreach (var state in gameStates) {
            disableState(state);
        }
        // enable first state in list
        enableState(gameStates[0]);
    }

    private void enableState(GameState gameState) {
        currentState = gameState;
        foreach (GameObject stateObject in gameState.stateObjects) {
            stateObject.SetActive(true);
        }
    }

    private void disableState(GameState gameState) {
        foreach (GameObject stateObject in gameState.stateObjects) {
            stateObject.SetActive(false);
        }
    }

    public void LoadPreviousState() {
        SetState(previousState.stateName);
    }

    public void SetState(string stateName) {
        EventBus<OnStateQuit>.Raise(new OnStateQuit(currentState.stateName));

        foreach (GameObject stateObject in currentState.stateObjects) {
            stateObject.SetActive(false);
        }

        previousState = currentState;

        currentState = gameStates.Find(state => state.stateName == stateName);
        if (currentState == null) {
            Debug.LogError("Could not find state with name: " + stateName);
            return;
        }

        foreach (GameObject stateObject in currentState.stateObjects) {
            stateObject.SetActive(true);
        }

        EventBus<OnStateEnter>.Raise(new OnStateEnter(currentState.stateName));
    }

    public string GetCurrentStateString() {
        return currentState.stateName;
    }

    public void SafeQuitApplication() {
        Application.Quit();
    }

}
