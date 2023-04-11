using UnityEngine;

public class LoadMainServerScene : MonoBehaviour {
    [SerializeField] private string mainServerSceneName;
    [SerializeField] private string levelName;

    public void LoadMainServerSceneAdditive() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainServerSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName, UnityEngine.SceneManagement.LoadSceneMode.Additive);

        GameManager.Instance.SetState("Game");
    }
}
