using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstaclePoolManager : MonoBehaviour
{
    #region Variables
    public static ObstaclePoolManager Instance = null;

    [SerializeField] private float m_MoveOutOfTheWayDuration = 1f;

    [Header("Portals")]
    [SerializeField] private Transform m_PortalParent = null;
    public GameObject m_PortalPrefab = null;
    private List<GameObject> m_PortalPool = new List<GameObject>();

    [Header("Asteroids")]
    [SerializeField] private Transform m_AsteroidParent = null;
    public List<GameObject> m_AsteroidPrefabs = null;
    private List<GameObject> m_AsteroidPool = new List<GameObject>();

    [Header("Items")]
    [SerializeField] private Transform m_ItemParent = null;
    public List<GameObject> m_ItemPrefabs = null;
    private List<GameObject> m_ItemPool = new List<GameObject>();
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
            Destroy(this);
        }
    }

    private void CreateObstaclePools()
    {
        for (int i = 0; i < m_AsteroidPrefabs.Count; i++)
        {
            GameObject obstacle = Instantiate(m_AsteroidPrefabs[i]);
            obstacle.SetActive(false);
            obstacle.transform.parent = m_AsteroidParent;
            obstacle.name = m_AsteroidPrefabs[i].name;

            m_AsteroidPool.Add(obstacle);
        }

        for (int i = 0; i < m_ItemPrefabs.Count; i++)
        {
            GameObject obstacle = Instantiate(m_ItemPrefabs[i]);
            obstacle.SetActive(false);
            obstacle.transform.parent = m_ItemParent;
            obstacle.name = m_ItemPrefabs[i].name;

            m_ItemPool.Add(obstacle);
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject portal = Instantiate(m_PortalPrefab);
            portal.SetActive(false);
            portal.transform.parent = m_PortalParent;
            portal.name = m_PortalPrefab.name;

            m_PortalPool.Add(portal);
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
        newPortal.transform.parent = m_PortalParent;
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
        newObstacle.transform.parent = m_AsteroidParent;
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
        newObstacle.transform.parent = m_ItemParent;
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

    public void MoveOutOfTheWay(bool isHorizontal)
    {
        foreach (GameObject asteroid in m_AsteroidPool)
        {
            if (asteroid.activeSelf)
            {
                StartCoroutine(MoveAsteroidOutOfTheWay(asteroid, isHorizontal));
            }
        }
    }

    private IEnumerator MoveAsteroidOutOfTheWay(GameObject asteroid, bool isHorizontal)
    {
        // Determining the target value for the asteroid based on the current state (horizontal or vertical)
        float targetValue = isHorizontal ? 12.5f : 12f;
        float direction = isHorizontal ? asteroid.transform.position.x : asteroid.transform.position.y;
        targetValue *= direction > 0 ? 1 : -1;

        Vector3 initialPosition = asteroid.transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < m_MoveOutOfTheWayDuration)
        {
            elapsedTime += Time.deltaTime;

            float newValue = 0f;

            if (isHorizontal)
            {
                newValue = Mathf.Lerp(initialPosition.x, targetValue, elapsedTime / m_MoveOutOfTheWayDuration);
                asteroid.transform.position = new Vector3(newValue, initialPosition.y, initialPosition.z);
            }
            else
            {
                newValue = Mathf.Lerp(initialPosition.y, targetValue, elapsedTime / m_MoveOutOfTheWayDuration);
                asteroid.transform.position = new Vector3(initialPosition.x, newValue, initialPosition.z);
            }

            yield return null;
        }
    }
    #endregion
}