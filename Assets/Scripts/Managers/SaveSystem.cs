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
    }
    #endregion

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

    #region Save
    public static void Save()
    {
        SavePlayerData();
        SaveLeaderboardData();
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

    public static void HandleSaveData()
    {
        GameManager.Instance.Save(ref m_SaveData.playerData, ref m_SaveData.leaderboardData);
    }

    #endregion

    #region Load
    public static void Load()
    {
        LoadSaveData();
        LoadLeaderboardData();
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

    public static void HandleLoadSaveData()
    {
        GameManager.Instance.Load(m_SaveData.playerData);
    }

    public static void HandleLoadLeaderboardData()
    {
        // We need to sort the leaderboard data before loading it
        m_SaveData.leaderboardData.leaderboard.Sort(ComparePlayerScores);

        GameManager.Instance.Load(m_SaveData.leaderboardData);
        // We don't need to update the UI here because the leaderboard is only displayed in the main menu.
    }

    private static int ComparePlayerScores(GameManager.PlayerScore playerScore1, GameManager.PlayerScore playerScore2)
    {
        return playerScore2.score.CompareTo(playerScore1.score);

        // Returns 1 if playerScore2 is greater than playerScore1
        // Returns 0 if playerScore2 is equal to playerScore1
        // Returns -1 if playerScore2 is less than playerScore1
    }

    #endregion
}

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