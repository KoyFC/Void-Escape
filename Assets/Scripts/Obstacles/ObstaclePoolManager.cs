using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstaclePoolManager : MonoBehaviour
{
    #region Variables
    public static ObstaclePoolManager Instance = null;

    [SerializeField] private ObstacleData m_AsteroidData = null;
    [SerializeField] private ObstacleData m_ItemData = null;

    public GameObject m_PortalPrefab = null;
    public List<GameObject> m_PortalPool = new List<GameObject>();

    public List<GameObject> m_AsteroidPrefabs = null;
    [HideInInspector] public List<GameObject> m_AsteroidPool = new List<GameObject>();

    public List<GameObject> m_ItemPrefabs = null;
    [HideInInspector] public List<GameObject> m_ItemPool = new List<GameObject>();
    #endregion

    #region Main Methods
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CreateObstaclePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CreateObstaclePools()
    {
        for (int i = 0; i < m_AsteroidPrefabs.Count; i++)
        {
            GameObject obstacle = Instantiate(m_AsteroidPrefabs[i]);
            obstacle.SetActive(false);
            obstacle.transform.parent = transform;

            m_AsteroidPool.Add(obstacle);
        }

        for (int i = 0; i < m_ItemPrefabs.Count; i++)
        {
            GameObject obstacle = Instantiate(m_ItemPrefabs[i]);
            obstacle.SetActive(false);
            obstacle.transform.parent = transform;

            m_ItemPool.Add(obstacle);
        }
    }
    #endregion

    #region Public Pool Methods
    public GameObject GetPortal()
    {
        for (int i = 0; i < m_PortalPool.Count; i++)
        {
            GameObject portal = m_PortalPool[i];
            if (portal == null)
            {
                m_PortalPool.RemoveAt(i);
                i--;
                continue;
            }
            if (!portal.activeSelf)
            {
                portal.SetActive(true);
                return portal;
            }
        }

        GameObject newPortal = Instantiate(m_PortalPrefab);
        newPortal.transform.parent = transform;
        newPortal.name = m_PortalPrefab.name;

        m_PortalPool.Add(newPortal);
        return newPortal;
    }

    public GameObject GetAsteroid(int index)
    {
        for (int i = 0; i < m_AsteroidPool.Count; i++)
        {
            GameObject obstacle = m_AsteroidPool[i];
            if (obstacle == null)
            {
                m_AsteroidPool.RemoveAt(i);
                i--;
                continue;
            }

            // Try to find an inactive asteroid of the same type
            if (!obstacle.activeSelf && obstacle.name == m_AsteroidPrefabs[index].name)
            {
                obstacle.SetActive(true);
                return obstacle;
            }
        }

        GameObject newObstacle = Instantiate(m_AsteroidPrefabs[index]);
        newObstacle.transform.parent = transform;
        newObstacle.name = m_AsteroidPrefabs[index].name;

        m_AsteroidPool.Add(newObstacle);
        return newObstacle;
    }

    public GameObject GetItem(int index)
    {
        for (int i = 0; i < m_ItemPool.Count; i++)
        {
            GameObject obstacle = m_ItemPool[i];
            if (obstacle == null)
            {
                m_ItemPool.RemoveAt(i);
                i--;
                continue;
            }

            if (!obstacle.activeSelf && obstacle.name == m_ItemPrefabs[index].name)
            {
                obstacle.SetActive(true);
                return obstacle;
            }
        }

        GameObject newObstacle = Instantiate(m_ItemPrefabs[index]);
        newObstacle.transform.parent = transform;
        newObstacle.name = m_ItemPrefabs[index].name;

        m_ItemPool.Add(newObstacle);
        return newObstacle;
    }

    public void ReturnToPool(GameObject obstacle)
    {
        obstacle.SetActive(false);
    }

    public void ReturnAllToPool()
    {
        for (int i = 0; i < m_AsteroidPool.Count; i++)
        {
            m_AsteroidPool[i].SetActive(false);
        }
        for (int i = 0; i < m_ItemPool.Count; i++)
        {
            m_ItemPool[i].SetActive(false);
        }
        for (int i = 0; i < m_PortalPool.Count; i++)
        {
            m_PortalPool[i].SetActive(false);
        }
    }
    #endregion
}
