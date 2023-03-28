using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private string sceneName;
    [SerializeField]
    private bool loadAdditive;
    
    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName,
            loadAdditive ? UnityEngine.SceneManagement.LoadSceneMode.Additive : UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}