using UnityEngine;

public class NetworkManager : MonoBehaviour {
    public static bool IsServer { get; private set; }
    [SerializeField] private bool isServer = true;
    [SerializeField] private string serverMenuSceneName;
    [SerializeField] private string clientSceneName;
    [SerializeField] private string levelName;

    private void Start() {
        IsServer = isServer;
        if (isServer) {
            startServer();
        } else {
            startClient();
        }

        Physics.gravity = new Vector3(0, -9.81f * 2, 0);

    }

    private void startServer() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(serverMenuSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    private void startClient() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(clientSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName, UnityEngine.SceneManagement.LoadSceneMode.Additive);

    }
}
