using UnityEngine;
using System.Collections.Generic;

public class SpaceshipManager : MonoBehaviour
{
    public static SpaceshipManager Instance { get; private set; }

    [System.Serializable]
    public class SpaceshipPrefabs
    {
        public ShipType type;
        public List<GameObject> colorVariants;
    }

    [SerializeField]
    private List<SpaceshipPrefabs> spaceshipPrefabs;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetSpaceshipPrefab(ShipType type, int colorIndex)
    {
        foreach (var shipType in spaceshipPrefabs)
        {
            if (shipType.type == type)
            {
                if (colorIndex >= 0 && colorIndex < shipType.colorVariants.Count)
                {
                    return shipType.colorVariants[colorIndex];
                }
            }
        }
        return null;
    }
}

[System.Serializable]
public enum ShipType
{
    Fighter,
    Bomber,
    Transport
}

[System.Serializable]
public enum ShipColors
{
    NEUTRAL,
    RED,
    BLUE,
    YELLOW
}