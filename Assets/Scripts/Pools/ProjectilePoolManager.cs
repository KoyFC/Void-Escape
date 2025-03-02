using UnityEngine;
using System.Collections.Generic;

public class ProjectilePoolManager : MonoBehaviour
{
    #region Variables
    public static ProjectilePoolManager Instance;

    [SerializeReference] private Transform m_PoolParent = null;
    [SerializeField] private GameObject m_ProjectilePrefab = null;
    private List<GameObject> m_ProjectilePool = new();
    #endregion

    #region Main Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CreatePool();
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    private void CreatePool()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject projectile = Instantiate(m_ProjectilePrefab);
            projectile.SetActive(false);
            projectile.transform.parent = m_PoolParent;
            projectile.name = m_ProjectilePrefab.name;

            m_ProjectilePool.Add(projectile);
        }
    }

    public GameObject GetProjectile()
    {
        for (int i = 0; i < m_ProjectilePool.Count; i++)
        {
            GameObject projectile = m_ProjectilePool[i];

            if (projectile == null)
            {
                m_ProjectilePool.RemoveAt(i);
                i--;
                continue;
            }
            if (!projectile.activeSelf)
            {
                projectile.SetActive(true);
                return projectile;
            }
        }

        GameObject newProjectile = Instantiate(m_ProjectilePrefab);
        newProjectile.SetActive(false);
        newProjectile.transform.parent = m_PoolParent;
        newProjectile.name = m_ProjectilePrefab.name;
        m_ProjectilePool.Add(newProjectile);

        return newProjectile;
    }

    public void ReturnToPool(GameObject projectile)
    {
        projectile.SetActive(false);
    }
}
