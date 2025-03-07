using UnityEngine;
using System.Collections;
using System;

public class PlayerItemController : MonoBehaviour
{
    private PlayerController m_PlayerController = null;
    private PlayerShootingScript m_PlayerShooting = null;
    private GameObject m_Shield = null;

    private Coroutine m_NoCooldownCoroutine = null;
    private Coroutine m_ShieldCoroutine = null;
    private Coroutine m_SlowMoCoroutine = null;

    public static event Action<float> OnNoCooldwnTimeChanged;
    public static event Action<float> OnShieldTimeChanged;
    public static event Action<float> OnSlowMoTimeChanged; 
    public static event Action<int> OnScoreMultiplierChanged;

    void Start()
    {
        m_PlayerController = GetComponent<PlayerController>();
        m_PlayerShooting = m_PlayerController.m_PlayerShooting;
    }

    private void OnEnable()
    {
        ObstacleController.OnItemCollected += HandleItemCollected;
    }

    private void OnDisable()
    {
        ObstacleController.OnItemCollected -= HandleItemCollected;
    }

    private void HandleItemCollected(PowerUpType type)
    {
        // In some of them we account for Time.timeScale, but in others we don't. This is intentional.
        switch (type)
        {
            case PowerUpType.NO_FIRE_COOLDOWN:
                if (m_NoCooldownCoroutine != null)
                {
                    StopCoroutine(m_NoCooldownCoroutine);
                }
                m_NoCooldownCoroutine = StartCoroutine(NoFireCooldown(7.5f));
                break;

            case PowerUpType.SHIELD:
                if (m_ShieldCoroutine != null)
                {
                    StopCoroutine(m_ShieldCoroutine);
                }
                m_ShieldCoroutine = StartCoroutine(SpawnShield(10f));
                break;

            case PowerUpType.SLOW_MO:
                if (m_SlowMoCoroutine != null)
                {
                    StopCoroutine(m_SlowMoCoroutine);
                }
                m_SlowMoCoroutine = StartCoroutine(SlowMo(10f));
                break;

            case PowerUpType.SCORE_MULTIPLIER:
                StopCoroutine(nameof(ScoreMultiplier));
                StartCoroutine(ScoreMultiplier(10f));
                break;
        }
    }

    private IEnumerator NoFireCooldown(float duration)
    {
        Debug.Log("No fire cooldown activated");
        m_PlayerShooting.m_CurrentFireRate = 0.005f;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            OnNoCooldwnTimeChanged?.Invoke(duration - elapsedTime);
            yield return null;
        }

        m_PlayerShooting.m_CurrentFireRate = m_PlayerShooting.m_UnmodifiedCurrentFireRate;
    }

    private IEnumerator SpawnShield(float duration)
    {
        Debug.Log("Shield activated");
        if (m_Shield == null)
        {
            m_Shield = Instantiate(InGameManager.Instance.m_ShieldPrefab, transform.position, Quaternion.identity);
            m_Shield.transform.SetParent(transform);
        }
        else if (!m_Shield.activeSelf)
        {
            m_Shield.SetActive(true);
        }

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            OnShieldTimeChanged?.Invoke(duration - elapsedTime);
            yield return null;
        }

        m_Shield.SetActive(false);
    }

    private IEnumerator SlowMo(float duration)
    {
        Time.timeScale = 0.33f;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            OnSlowMoTimeChanged?.Invoke(duration - elapsedTime);
            yield return null;
        }

        Time.timeScale = 1f;
    }

    // Coroutine that doesn't need events since it has no associated UI element
    private IEnumerator ScoreMultiplier(float duration)
    {
        InGameManager.Instance.m_ScoreMultiplier *= 2;
        OnScoreMultiplierChanged?.Invoke(InGameManager.Instance.m_ScoreMultiplier);

        yield return new WaitForSeconds(duration);

        InGameManager.Instance.m_ScoreMultiplier /= 2;
        OnScoreMultiplierChanged?.Invoke(InGameManager.Instance.m_ScoreMultiplier);
    }
}
