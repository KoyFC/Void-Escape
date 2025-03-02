using UnityEngine;
using System.Collections;

public class PlayerShootingScript : MonoBehaviour
{
    [SerializeField] private float m_FireRate = 0.5f;
    internal bool m_CanFire = false;

    private Transform m_FirePoint;

    private void Start()
    {
        m_FirePoint = transform.GetChild(0);
    }

    private void Update()
    {
        if (PlayerInputScript.Instance.m_FireHeld && m_CanFire)
        {
            StartCoroutine(FireWeapon());
        }
    }

    private IEnumerator FireWeapon()
    {
        m_CanFire = false;

        Fire();

        yield return new WaitForSeconds(m_FireRate);

        m_CanFire = true;
    }

    private void Fire()
    {
        GameObject bullet = ProjectilePoolManager.Instance.GetProjectile();

        bullet.transform.position = m_FirePoint.position;
    }
}