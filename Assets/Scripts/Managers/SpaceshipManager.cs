using UnityEngine;
using System.Collections.Generic;

public class SpaceshipManager : MonoBehaviour
{
    #region Structures
    [System.Serializable]
    public struct SpaceshipVariant
    {
        public ShipType type;
        public Vector3 spawnOffset;
        public List<GameObject> colorVariants;
    }
    #endregion

    #region Variables
    public static SpaceshipManager Instance = null;

    [SerializeField] private List<SpaceshipVariant> spaceshipPrefabs;
    #endregion

    #region Main Methods
    void Awake()
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

    private void Start()
    {
        //SpaceshipAttributes currentSpaceShip = GameManager.Instance.m_CurrentSpaceShip;

        //GameObject spaceshipPrefab = GetSpaceshipPrefab(currentSpaceShip.shipType, currentSpaceShip.shipColor);

        //Instantiate(spaceshipPrefab, PointManager.Instance.m_Points.CenterPoint.position, Quaternion.identity);
        // MOVE TO IN-GAME MANAGER
    }
    #endregion

    public GameObject GetSpaceshipPrefab(ShipType type, ShipColor color)
    {
        foreach (SpaceshipVariant ship in spaceshipPrefabs)
        {
            if (ship.type == type)
            {
                return ship.colorVariants[(int)color];
            }
        }
        return null;
    }

    public Vector3 GetSpawnOffset(ShipType type)
    {
        foreach (SpaceshipVariant ship in spaceshipPrefabs)
        {
            if (ship.type == type)
            {
                return ship.spawnOffset;
            }
        }
        return Vector3.zero;
    }
}

#region Public Structures
[System.Serializable]
public struct SpaceshipAttributes
{
    public ShipType shipType;
    public ShipColor shipColor;
}

[System.Serializable]
public enum ShipType
{
    BIRD,
    FLAT,
    STINGER,
    DOUBLE_CANNON
}

[System.Serializable]
public enum ShipColor
{
    NEUTRAL,
    RED,
    BLUE,
    YELLOW
}
#endregion