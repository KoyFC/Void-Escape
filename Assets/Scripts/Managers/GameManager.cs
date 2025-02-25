using UnityEngine;
using UnityEngine.SceneManagement;

// Script that handles the player data during the execution. Also saves and loads the data automatically.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    private string m_CurrentName = string.Empty;
    private int m_Credits = 0;

    private void Awake()
    {
        #if UNITY_ANDROID
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.orientation = ScreenOrientation.AutoRotation;
        #endif

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SaveSystem.Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        SaveSystem.Save();
    }

    #region Save and Load
    public void Save(ref PlayerSaveData data)
    {
        data.currentName = m_CurrentName;
        data.credits = m_Credits;
    }

    public void Load(PlayerSaveData data)
    {
        m_CurrentName = data.currentName;
        m_Credits = data.credits;
    }
    #endregion
}

[System.Serializable]
public struct PlayerSaveData
{
    public string currentName;
    public int credits;
}
