using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneManager", menuName = "Scriptable Objects/SceneManager")]
public class SceneManager : ScriptableObject
{
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            Debug.Log("Quitting the game...");
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }
}
