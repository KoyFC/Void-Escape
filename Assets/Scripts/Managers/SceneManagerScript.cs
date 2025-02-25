using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        EditorSceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
