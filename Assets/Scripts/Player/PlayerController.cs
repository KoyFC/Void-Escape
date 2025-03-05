using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerInputScript))]
[RequireComponent(typeof(PlayerMovementScript))]
[RequireComponent(typeof(PlayerHealthScript))]
[RequireComponent(typeof(PlayerShootingScript))]
public class PlayerController : MonoBehaviour
{
    internal PlayerMovementScript m_PlayerMovement = null;
    internal PlayerHealthScript m_PlayerHealth = null;
    internal PlayerShootingScript m_PlayerShooting = null;

    private GameObject m_Shield = null;

    private void OnEnable()
    {
        ObstacleController.OnItemCollected += HandleItemCollected;
    }

    private void OnDisable()
    {
        ObstacleController.OnItemCollected -= HandleItemCollected;
    }

    void Awake()
    {
        GetAllComponents();
    }

    private void GetAllComponents()
    {
        m_PlayerMovement = GetComponent<PlayerMovementScript>();
        m_PlayerHealth = GetComponent<PlayerHealthScript>();
        m_PlayerShooting = GetComponent<PlayerShootingScript>();
    }

    private void HandleItemCollected(PowerUpType type)
    {
        // In some of them we account for Time.timeScale, but in others we don't. This is intentional.
        switch (type)
        {
            case PowerUpType.NO_FIRE_COOLDOWN:
                StopCoroutine(nameof(NoFireCooldown));
                StartCoroutine(NoFireCooldown(7.5f));
                break;

            case PowerUpType.SHIELD:
                StopCoroutine(nameof(SpawnShield));
                StartCoroutine(SpawnShield(10f));

                break;

            case PowerUpType.SLOW_MO:
                StopCoroutine(nameof(SlowMo));
                StartCoroutine(SlowMo(10f));
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

        yield return new WaitForSecondsRealtime(duration);

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

        yield return new WaitForSeconds(duration);

        m_Shield.SetActive(false);
    }

    private IEnumerator SlowMo(float duration)
    {
        Time.timeScale = 0.33f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
    }

    private IEnumerator ScoreMultiplier(float duration)
    {
        InGameManager.Instance.m_ScoreMultiplier *= 2;

        yield return new WaitForSeconds(duration);

        InGameManager.Instance.m_ScoreMultiplier /= 2;
    }
}