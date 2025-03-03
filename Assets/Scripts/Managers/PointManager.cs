using UnityEngine;

public class PointManager : MonoBehaviour
{
    #region Structures
    [System.Serializable]
    public struct Points
    {
        public Transform CenterPoint;

        [Header("Horizontal Points")]
        public Transform LeftPoint;
        public Transform RightPoint;

        [Header("Vertical Points")]
        public Transform TopPoint;
        public Transform BottomPoint;
    }
    #endregion

    #region Variables
    public static PointManager Instance = null;
    public Points m_PlayerPoints;
    public Points m_EnemyPoints;

    private int m_PreviousRandom = 1;
    private int m_RandomRepetitions = 0;
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Transform GetRandomHorizontalPoint()
    {
        int random = GetRandomNumber();

        switch (random)
        {
            case 0:
                return m_EnemyPoints.LeftPoint;
            case 1:
                return m_EnemyPoints.CenterPoint;
            case 2:
                return m_EnemyPoints.RightPoint;
        }

        return null;
    }

    public Transform GetRandomVerticalPoint()
    {
        int random = GetRandomNumber();

        switch (random)
        {
            case 0:
                return m_EnemyPoints.TopPoint;
            case 1:
                return m_EnemyPoints.CenterPoint;
            case 2:
                return m_EnemyPoints.BottomPoint;
        }
        return null;
    }

    // We want to avoid the same number being generated twice in a row
    private int GetRandomNumber()
    {
        int random = Random.Range(0, 3);

        if (random == m_PreviousRandom)
        {
            m_RandomRepetitions++;
        }

        while (random == m_PreviousRandom && m_RandomRepetitions > 1)
        {
            random = Random.Range(0, 3);
            m_RandomRepetitions++;
        }

        m_RandomRepetitions = 0;
        return random;
    }
}
