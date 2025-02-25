using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance = null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Load from file or player prefs
    }
}
