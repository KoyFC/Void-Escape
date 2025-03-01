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
        int random = Random.Range(0, 3);

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
        int random = Random.Range(0, 3);
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
}
