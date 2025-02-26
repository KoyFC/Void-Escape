using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Script that handles the player data during the execution. Also saves and loads the data automatically.
public class GameManager : MonoBehaviour
{
    #region variables
    public static GameManager Instance = null;

    private string m_CurrentName = string.Empty;
    public List<PlayerScore> m_Leaderboard;

    [System.Serializable]
    public struct PlayerScore
    {
        public string playerName;
        public int score;
    }
    #endregion

    #region Main Methods
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
    #endregion

    #region Save and Load
    public void Save(ref PlayerSaveData playerData, ref LeaderboardSaveData leaderboardData)
    {
        playerData.currentName = m_CurrentName;
        playerData.credits = CurrencyManager.Instance.Credits;

        leaderboardData.leaderboard = m_Leaderboard;
    }

    public void Load(PlayerSaveData playerData, LeaderboardSaveData leaderboardData)
    {
        m_CurrentName = playerData.currentName;
        CurrencyManager.Instance.SetCredits(playerData.credits);

        m_Leaderboard = leaderboardData.leaderboard;
    }
    #endregion
}