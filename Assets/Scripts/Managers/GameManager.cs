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

    public string m_CurrentName = "CoolShip";
    private List<PlayerScore> m_Leaderboard = new();
    public List<bool> m_UnlockedShips = new();
    public List<bool> m_UnlockedColors = new();
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

            m_CurrentName = "CoolShip";
            InitializeDefaultShipTypes();
            InitializeDefaultColors();
            InitializeDefaultShip();

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
    public void Save(ref PlayerSaveData playerData, ref LeaderboardSaveData leaderboardData, ref SettingsSaveData settingsData)
    {
        playerData.currentName = m_CurrentName;
        playerData.credits = CurrencyManager.Instance.Credits;
        playerData.unlockedShips = m_UnlockedShips;
        playerData.unlockedColors = m_UnlockedColors;
        playerData.currentSpaceship = m_CurrentSpaceShip;

        leaderboardData.leaderboard = m_Leaderboard;

        settingsData.targetFPS = Application.targetFrameRate;
        // Save resolution
    }

    public void Load(PlayerSaveData playerData)
    {
        m_CurrentName = playerData.currentName;
        CurrencyManager.Instance.SetCredits(playerData.credits);
        m_UnlockedShips = playerData.unlockedShips;
        m_UnlockedColors = playerData.unlockedColors;

        m_CurrentSpaceShip = playerData.currentSpaceship;
    }

    public void Load(LeaderboardSaveData leaderboardData)
    {
        m_Leaderboard = leaderboardData.leaderboard;
    }

    public void Load(SettingsSaveData settingsData)
    {
        Application.targetFrameRate = settingsData.targetFPS;
        // Scale resolution
    }
    #endregion

    #region Helper Methods
    private void InitializeDefaultShip()
    {
        m_CurrentSpaceShip.shipType = ShipType.BIRD;
        m_CurrentSpaceShip.shipColor = ShipColor.NEUTRAL;
    }

    private void InitializeDefaultShipTypes()
    {
        for (int i = 0; i < Enum.GetValues(typeof(ShipType)).Length; i++)
        {
            if (i == 0) // Unlock the first ship by default
            {
                m_UnlockedShips.Add(true);
            }
            else
            {
                m_UnlockedShips.Add(false);
            }
        }
    }

    private void InitializeDefaultColors()
    {
        for (int i = 0; i < Enum.GetValues(typeof(ShipColor)).Length; i++)
        {
            if (i == 0) // Unlock the first color by default
            {
                m_UnlockedColors.Add(true);
            }
            else
            {
                m_UnlockedColors.Add(false);
            }
        }
    }
    #endregion

    #region Public Methods
    public void UnlockShipType(int shipType)
    {
        if (shipType >= 0 && shipType < m_UnlockedShips.Count)
        {
            m_UnlockedShips[shipType] = true;
        }
    }

    public void UnlockShipColor(int shipColor)
    {
        if (shipColor >= 0 && shipColor < m_UnlockedColors.Count)
        {
            m_UnlockedColors[shipColor] = true;
        }
    }

    public void ResetData(int dataToReset)
    {
        SaveSystem.DeleteFile(dataToReset);
        switch (dataToReset)
        {
            case 0:
                ResetPlayerData();
                break;
            case 1:
                ResetLeaderboardData();
                break;
            case 2:
                ResetSettingsData();
                break;
            case 3:
                ResetPlayerData();
                ResetLeaderboardData();
                ResetSettingsData();
                break;
        }
    }
    #endregion

    #region Resetting
    private void ResetPlayerData()
    {
        m_CurrentName = "CoolShip";
        CurrencyManager.Instance.ResetCredits();

        m_UnlockedShips.Clear();
        InitializeDefaultShipTypes();

        m_UnlockedColors.Clear();
        InitializeDefaultColors();

        InitializeDefaultShip();
    }

    private void ResetLeaderboardData()
    {
        m_Leaderboard.Clear();
    }

    private void ResetSettingsData()
    {
        Application.targetFrameRate = 60;
    }
    #endregion
}