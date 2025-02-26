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
    public Points m_Points;
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
}
