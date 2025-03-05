using UnityEngine;

public class ShieldController : MonoBehaviour
{
    [System.Serializable]
    public struct ShieldAttributes
    {
        public ShipType type;
        public Vector3 scale;
    }

    [SerializeField] private ShieldAttributes[] m_ShieldConfigurations;

    private Transform m_PlayerTransform;
    private ShipType m_CurrentShipType;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        m_PlayerTransform = player.transform;
        m_CurrentShipType = GameManager.Instance.m_CurrentSpaceShip.shipType;

        AdjustShieldScale();
    }

    void AdjustShieldScale()
    {
        foreach (ShieldAttributes shieldConfig in m_ShieldConfigurations)
        {
            if (shieldConfig.type == m_CurrentShipType)
            {
                transform.localScale = shieldConfig.scale;
                break;
            }
        }
    }
}
