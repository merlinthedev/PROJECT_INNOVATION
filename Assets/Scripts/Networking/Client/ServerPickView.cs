using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;


/**
 * Wraps all elements and functionality required for the LoginView.
 */
public class ServerPickView : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI textConnectResults = null;
    [SerializeField] private GameObject goConnectResultsRow = null;

    [SerializeField] private GameObject serverListRoot;
    [SerializeField] private ServerAddress serverListItemPrefab;

    [SerializeField] private InputField inputField;
    [SerializeField] private Button connectButton;

    public System.Action<string, int> OnServerConnectRequest;

    private string inputAddress;

    private string inputIp;
    private int inputPort;

    private void Start() {
        inputField.onEndEdit.AddListener((value) => {
            inputAddress = value;

            // parse ip:port to separate fields
            string[] split = value.Split(':');
            if (split.Length == 2) {
                inputIp = split[0];
                inputPort = int.Parse(split[1]);
            } else {
                inputIp = value;
                inputPort = 0;
            }
        });

        connectButton.onClick.AddListener(() => {
            try {
                OnServerConnectRequest?.Invoke(inputIp, inputPort);
                GameManager.Instance.SetState("Game");
            } catch (Exception e) {
                Debug.LogError(e);
            }
        });
    }
    public string TextConnectResults {
        set {
            textConnectResults.text = value;
            //enable the row with status info based on whether we actually have status info
            goConnectResultsRow.SetActive(value != null && value.Length > 0);
        }
    }

    public ServerAddress AddServer(string address, int port) {
        ServerAddress serverAddress = Instantiate(serverListItemPrefab, serverListRoot.transform);
        serverAddress.SetServer(address, port);
        serverAddress.OnServerSelected += (a, b) => OnServerConnectRequest?.Invoke(a, b);
        return serverAddress;
    }

    public void ClearServers() {
        Debug.LogError("Clearing servers");
        foreach (Transform child in serverListRoot.transform) {
            Destroy(child.gameObject);
        }
    }
}
