using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    #region Structures
    [System.Serializable]
    private struct Points
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
    [SerializeField] private Points m_Points;

    #endregion

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
