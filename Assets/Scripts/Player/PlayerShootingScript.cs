using UnityEngine;
using System.Collections;
using CandyCoded.HapticFeedback;

public class PlayerShootingScript : MonoBehaviour
{
    [SerializeField] private float m_DefaultFireRate = 0.5f;
    internal float m_UnmodifiedCurrentFireRate = 0.5f;
    internal float m_CurrentFireRate = 0.5f;
    internal bool m_CanFire = false;

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
        if (PlayerInputScript.Instance.m_FireHeld && m_CanFire)
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

        yield return new WaitForSecondsRealtime(m_CurrentFireRate);

        m_CanFire = true;
    }

    private void Fire()
    {
        GameObject bullet = ProjectilePoolManager.Instance.GetProjectile();

        bullet.transform.position = m_FirePoint.position;
        HapticFeedback.LightFeedback();
    }
}