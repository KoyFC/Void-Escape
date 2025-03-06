using UnityEngine;
using System.IO;
using System.Collections.Generic;

// Script that handles the saving and loading of the game data.
public class SaveSystem
{
    #region Variables
    public static SaveData m_SaveData = new();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData playerData;
        public LeaderboardSaveData leaderboardData;
        public SettingsSaveData settingsData;
    }
    #endregion

    #region File Paths
    public static string SaveDataFileName()
    {
        string saveFile = Application.persistentDataPath + "/saveData.kfc";
        return saveFile;
    }

    public static string LeaderboardFileName()
    {
        string leaderboardFile = Application.persistentDataPath + "/leaderboardData.kfc";
        return leaderboardFile;
    }

    public static string SettingsFileName()
    {
        string settingsFile = Application.persistentDataPath + "/playerSettings.kfc";
        return settingsFile;
    }
    #endregion

    #region Save
    public static void Save()
    {
        SavePlayerData();
        SaveLeaderboardData();
        SaveSettingsData();
    }

    private static void SavePlayerData()
    {
        HandleSaveData();
        string saveFile = SaveDataFileName();
        string playerDataJson = JsonUtility.ToJson(m_SaveData.playerData, true);
        File.WriteAllText(saveFile, playerDataJson);
    }

    private static void SaveLeaderboardData()
    {
        string leaderboardFile = LeaderboardFileName();
        string leaderboardDataJson = JsonUtility.ToJson(m_SaveData.leaderboardData, true);
        File.WriteAllText(leaderboardFile, leaderboardDataJson);
    }

    private static void SaveSettingsData()
    {
        string settingsFile = SettingsFileName();
        string settingsDataJson = JsonUtility.ToJson(m_SaveData.settingsData, true);
        File.WriteAllText(settingsFile, settingsDataJson);
    }

    public static void HandleSaveData()
    {
        GameManager.Instance.Save(
            ref m_SaveData.playerData, 
            ref m_SaveData.leaderboardData, 
            ref m_SaveData.settingsData);
    }
    #endregion

    #region Load
    public static void Load()
    {
        LoadSaveData();
        LoadLeaderboardData();
        LoadSettingsData();
    }

    private static void LoadSaveData()
    {
        string saveFile = SaveDataFileName();
        if (!File.Exists(saveFile))
        {
            #if UNITY_EDITOR
                Debug.LogWarning("Save file not found, nothing to load.");
            #endif

            // Create a new empty file and load again
            SavePlayerData();
            LoadSaveData();
        }
        else
        {
            string saveContent = File.ReadAllText(saveFile);
            m_SaveData.playerData = JsonUtility.FromJson<PlayerSaveData>(saveContent);
            HandleLoadSaveData();
        }
    }

    private static void LoadLeaderboardData()
    {
        string leaderboardFile = LeaderboardFileName();
        if (!File.Exists(leaderboardFile))
        {
            #if UNITY_EDITOR
                Debug.LogWarning("Leaderboard file not found, nothing to load.");
            #endif

            // Create a new empty file and load again
            SaveLeaderboardData();
            LoadLeaderboardData();
        }
        else
        {
            string leaderboardContent = File.ReadAllText(leaderboardFile);
            m_SaveData.leaderboardData = JsonUtility.FromJson<LeaderboardSaveData>(leaderboardContent);
            HandleLoadLeaderboardData();
        }
    }

    private static void LoadSettingsData()
    {
        string settingsFile = SettingsFileName();
        if (!File.Exists(settingsFile))
        {
#if UNITY_EDITOR
            Debug.LogWarning("Settings file not found, nothing to load.");
#endif
            // Create a new empty file and load again
            SaveSettingsData();
            Application.targetFrameRate = 60;

#if UNITY_ANDROID
            m_SaveData.settingsData.motionControls = true;
#else
            m_SaveData.settingsData.motionControls = false;
#endif
            LoadSettingsData();
        }
        else
        {
            string settingsContent = File.ReadAllText(settingsFile);
            m_SaveData.settingsData = JsonUtility.FromJson<SettingsSaveData>(settingsContent);
            HandleLoadSettingsData();
        }
    }

    private static void HandleLoadSaveData()
    {
        GameManager.Instance.Load(m_SaveData.playerData);
    }

    private static void HandleLoadLeaderboardData()
    {
        // We need to sort the leaderboard data before loading it
        m_SaveData.leaderboardData.leaderboard.Sort(ComparePlayerScores);

        GameManager.Instance.Load(m_SaveData.leaderboardData);
        // We don't need to update the UI here because the leaderboard is only displayed in the main menu.
    }

    private static void HandleLoadSettingsData()
    {
        // Audio
        GameManager.Instance.Load(m_SaveData.settingsData);
    }

    public static int ComparePlayerScores(GameManager.PlayerScore playerScore1, GameManager.PlayerScore playerScore2)
    {
        return playerScore2.score.CompareTo(playerScore1.score);

        // Returns 1 if playerScore2 is greater than playerScore1
        // Returns 0 if playerScore2 is equal to playerScore1
        // Returns -1 if playerScore2 is less than playerScore1
    }

    public static void DeleteFile(int dataToDelete)
    {
        switch (dataToDelete)
        {
            case 0:
                File.Delete(SaveDataFileName());
                break;
            case 1:
                File.Delete(LeaderboardFileName());
                break;
            case 2:
                File.Delete(SettingsFileName());
                break;
            case 3:
                File.Delete(SaveDataFileName());
                File.Delete(LeaderboardFileName());
                File.Delete(SettingsFileName());
                break;
        }
    }
    #endregion
}

#region Save Data Structs
[System.Serializable]
public struct PlayerSaveData
{
    public string currentName;
    public int credits;
    public List<bool> unlockedShips;
    public List<bool> unlockedColors;
    public SpaceshipAttributes currentSpaceship;
}

[System.Serializable]
public struct LeaderboardSaveData
{
    public List<GameManager.PlayerScore> leaderboard;
}

[System.Serializable]
public struct SettingsSaveData
{
    public int targetFPS;
    public bool motionControls;
    public bool dynamicResolution;
}
#endregion