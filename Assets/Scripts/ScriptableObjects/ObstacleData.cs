using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Scriptable Objects/ObstacleData")]
public class ObstacleData : ScriptableObject
{
    #region Structures
    [System.Serializable]
    public enum ObstacleType
    {
        ASTEROID,
        ITEM
    }
    #endregion

    [Header("Obstacle settings")]
    public ObstacleType m_ObstacleType;
    public float m_LifeTime = 5f;

    [Header("Asteroid Only Settings")]
    public float m_GainedConfidenceOnDestroy = 7.5f;
    public int m_ScoreOnDestroy = 10;
    public float m_LostConfidenceOnCollision = 5f;
    public int m_ScoreOnCollision = 25;

    [Header("Item Only Settings")] 
    public PowerUpType m_PowerUpType;

    [Header("Speed")]
    public float m_MinSpeed = 1f;
    public float m_MaxSpeed = 3f;

    [Header("Rotation")]
    public float m_MinRotationSpeed = 1f;
    public float m_MaxRotationSpeed = 3f;
}

[System.Serializable]
public enum PowerUpType
{
    NO_FIRE_COOLDOWN,
    SHIELD,
    SLOW_MO,
    SCORE_MULTIPLIER
}