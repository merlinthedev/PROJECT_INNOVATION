using UnityEngine;

public class NetworkManager : MonoBehaviour {
    public static bool IsServer { get; private set; }
    [SerializeField] private bool isServer = true;
    [SerializeField] private string serverSceneName;
    [SerializeField] private string clientSceneName;

    private void Start() {
        IsServer = isServer;
        if (isServer) {
            startServer();
        } else {
            startClient();
        }
    }

    private void startServer() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(serverSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    private void startClient() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(clientSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
}
