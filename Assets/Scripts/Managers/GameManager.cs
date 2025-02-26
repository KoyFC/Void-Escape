using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Script that handles the player data during the execution. Also saves and loads the data automatically.
public class GameManager : MonoBehaviour
{
    #region Structures
    [System.Serializable]
    public struct PlayerScore
    {
        public string playerName;
        public int score;
    }
    #endregion

    #region Variables
    public static GameManager Instance = null;

    private string m_CurrentName = string.Empty;
    private List<PlayerScore> m_Leaderboard;
    public Dictionary<ShipType, bool> m_UnlockedShips = new Dictionary<ShipType, bool>();
    public Dictionary<ShipColor, bool> m_UnlockedColors = new Dictionary<ShipColor, bool>();

    public SpaceshipAttributes m_CurrentSpaceShip; // Holds the current spaceship attributes in order to instantiate it in the scene and save them
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

            InitializeTypeDictionary();
            InitializeColorDictionary();

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

    private void InitializeTypeDictionary()
    {
        m_UnlockedShips = new Dictionary<ShipType, bool>();
        foreach (ShipType shipType in Enum.GetValues(typeof(ShipType)))
        {
            m_UnlockedShips[shipType] = false;
        }
    }

    private void InitializeColorDictionary()
    {
        m_UnlockedColors = new Dictionary<ShipColor, bool>();
        foreach (ShipColor shipColor in Enum.GetValues(typeof(ShipColor)))
        {
            m_UnlockedColors[shipColor] = false;
        }
    }
    #endregion

    #region Save and Load
    public void Save(ref PlayerSaveData playerData, ref LeaderboardSaveData leaderboardData)
    {
        playerData.currentName = m_CurrentName;
        playerData.credits = CurrencyManager.Instance.Credits;
        playerData.unlockedShips = new List<KeyValuePair<ShipType, bool>>(m_UnlockedShips);
        playerData.unlockedColors = new List<KeyValuePair<ShipColor, bool>>(m_UnlockedColors);
        playerData.currentSpaceship = m_CurrentSpaceShip;

        leaderboardData.leaderboard = m_Leaderboard;
    }

    public void Load(PlayerSaveData playerData, LeaderboardSaveData leaderboardData)
    {
        m_CurrentName = playerData.currentName;
        CurrencyManager.Instance.SetCredits(playerData.credits);

        if (playerData.unlockedShips != null)
        {
            foreach (var kvp in playerData.unlockedShips)
            {
                m_UnlockedShips[kvp.Key] = kvp.Value;
            }
        }
        else
        {
            InitializeTypeDictionary();
        }

        if (playerData.unlockedColors != null)
        {
            foreach (var kvp in playerData.unlockedColors)
            {
                m_UnlockedColors[kvp.Key] = kvp.Value;
            }
        }
        else
        {
            InitializeColorDictionary();
        }

        m_CurrentSpaceShip = playerData.currentSpaceship;
        m_Leaderboard = leaderboardData.leaderboard;
    }
    #endregion

    #region Public Methods
    public void SetCurrentName(string name)
    {
        m_CurrentName = name;
    }

    public void UnlockShipType(ShipType shipType)
    {
        m_UnlockedShips[shipType] = true;
    }

    public void UnlockShipColor(ShipColor shipColor)
    {
        m_UnlockedColors[shipColor] = true;
    }

    public void LockShipType(ShipType shipType)
    {
        m_UnlockedShips[shipType] = false;
    }

    public void LockShipColor(ShipColor shipColor)
    {
        m_UnlockedColors[shipColor] = false;
    }
    #endregion
}