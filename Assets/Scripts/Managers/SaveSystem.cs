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

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/saveData.kfc";
        return saveFile;
    }

    #region Save
    public static void Save()
    {
        HandleSaveData();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(m_SaveData, true));
    }

    public static void HandleSaveData()
    {
        GameManager.Instance.Save(ref m_SaveData.playerData, ref m_SaveData.leaderboardData);
    }
    #endregion

    #region Load
    public static void Load()
    {
        string saveFile = SaveFileName();
        if (!File.Exists(saveFile))
        {
            #if UNITY_EDITOR
                Debug.LogWarning("Save file not found, nothing to load.");
            #endif

            return;
        }

        string saveContent = File.ReadAllText(saveFile);

        m_SaveData = JsonUtility.FromJson<SaveData>(saveContent);

        HandleLoadData();
    }

    public static void HandleLoadData()
    {
        GameManager.Instance.Load(m_SaveData.playerData, m_SaveData.leaderboardData);
        // We don't need to update the UI here, because the MainMenuUIManager will do it on Start.
    }
    #endregion
}

[System.Serializable]
public struct PlayerSaveData
{
    public string currentName;
    public int credits;
}

[System.Serializable]
public struct LeaderboardSaveData
{
    public List<GameManager.PlayerScore> leaderboard;
}