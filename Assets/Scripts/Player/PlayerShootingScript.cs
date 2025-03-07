using UnityEngine;
using System.Collections;
using CandyCoded.HapticFeedback;

public class PlayerShootingScript : MonoBehaviour
{
    [SerializeField] private float m_DefaultFireRate = 0.5f;
    internal float m_UnmodifiedCurrentFireRate = 0.5f;
    internal float m_CurrentFireRate = 0.5f;
    internal bool m_CanFire = false;
    bool m_MultiShoot = false;

    private float m_MultiShootProjectileRotation = 25f;

    private Transform m_FirePoint;

    private void OnEnable()
    {
        InGameManager.Instance.OnDifficultyChanged += HandleDifficultyChange;
    }

    private void OnDisable()
    {
        InGameManager.Instance.OnDifficultyChanged -= HandleDifficultyChange;
    }

    private void Start()
    {
        m_FirePoint = transform.GetChild(0);
        m_CurrentFireRate = m_DefaultFireRate;
    }

    private void Update()
    {
        m_MultiShoot = PlayerInputScript.Instance.m_Fire2Held;
        bool fire = PlayerInputScript.Instance.m_FireHeld || PlayerInputScript.Instance.m_Fire2Held;

        if (fire && m_CanFire)
        {
            StartCoroutine(FireWeapon());
        }
    }

    private void HandleDifficultyChange(int newDifficulty)
    {
        switch (newDifficulty)
        {
            case 3:
                m_CurrentFireRate = m_DefaultFireRate * 0.75f;
                m_UnmodifiedCurrentFireRate = m_CurrentFireRate;
                break;
            case 5:
                m_CurrentFireRate = m_DefaultFireRate * 0.4f;
                m_UnmodifiedCurrentFireRate = m_CurrentFireRate;
                break;
            default:
                return;
        }

        Debug.Log("New fire rate: " + m_CurrentFireRate);
        //StopAllCoroutines();
    }

    private IEnumerator FireWeapon()
    {
        m_CanFire = false;

        Fire();

        if (m_MultiShoot)
        {
            yield return new WaitForSecondsRealtime(m_CurrentFireRate);
        }

        yield return new WaitForSecondsRealtime(m_CurrentFireRate);

        m_CanFire = true;
    }

    private void Fire()
    {
        GameObject bullet = ProjectilePoolManager.Instance.GetProjectile();
        bullet.transform.position = m_FirePoint.position;
        bullet.transform.rotation = Quaternion.Euler(90, 0, 0);
        HapticFeedback.LightFeedback();

        if (m_MultiShoot)
        {
            float xRotationDiff, zRotation;

            if (InGameManager.Instance.m_IsHorizontal)
            {
                xRotationDiff = 0;
                zRotation = m_MultiShootProjectileRotation;
            }
            else
            {
                xRotationDiff = m_MultiShootProjectileRotation;
                zRotation = 0f;
            }


            GameObject bullet1 = ProjectilePoolManager.Instance.GetProjectile();
            bullet1.transform.position = m_FirePoint.position;
            bullet1.transform.rotation = Quaternion.Euler(90 - xRotationDiff, 0, -zRotation);
            HapticFeedback.LightFeedback();

            GameObject bullet2 = ProjectilePoolManager.Instance.GetProjectile();
            bullet2.transform.position = m_FirePoint.position;
            bullet2.transform.rotation = Quaternion.Euler(90 + xRotationDiff, 0, zRotation);
            HapticFeedback.LightFeedback();
        }
    }
}