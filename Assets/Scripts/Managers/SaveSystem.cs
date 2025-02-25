using UnityEngine;
using System.IO;

// Script that handles the saving and loading of the game data.
public class SaveSystem
{
    private static SaveData m_SaveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData playerData;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/saveData.kfc";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(m_SaveData, true));
    }

    public static void HandleSaveData()
    {
        GameManager.Instance.Save(ref m_SaveData.playerData);
    }

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
        GameManager.Instance.Load(m_SaveData.playerData);
    }
}


