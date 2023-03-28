using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerAddress : MonoBehaviour {
    [SerializeField] private TMP_Text serverAddressText;
    [SerializeField] private TMP_Text serverPortText;

    public System.Action<string, int> onServerSelected;
    public Button connectButton;

    public string Address { get; private set; }
    public int Port { get; private set; }

    private void Start() {
        if (connectButton != null) connectButton.onClick.AddListener(OnButtonClicked);

    }

    public void SetServer(string address, int port) {
        Address = address;
        Port = port;
        serverAddressText.text = address;
        serverPortText.text = port.ToString();
    }

    private void OnButtonClicked() {
        onServerSelected?.Invoke(Address, Port);
    }
}
