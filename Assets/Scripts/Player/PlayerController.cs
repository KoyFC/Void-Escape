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
        StartCoroutine(PowerUp(type));
    }

    private IEnumerator PowerUp(PowerUpType type)
    {
        float duration = 10f;
        switch (type)
        {
            case PowerUpType.NO_FIRE_COOLDOWN:
                m_PlayerShooting.m_CurrentFireRate = 0.005f;
                break;

            case PowerUpType.SHIELD:
                break;

            case PowerUpType.SLOW_MO:
                Time.timeScale = 0.33f;
                break;

            case PowerUpType.SCORE_MULTIPLIER:
                InGameManager.Instance.m_ScoreMultiplier *= 2; // Can be stacked
                break;
        }

        yield return new WaitForSecondsRealtime(duration);

        switch (type)
        {
            case PowerUpType.NO_FIRE_COOLDOWN:
                m_PlayerShooting.m_CurrentFireRate = m_PlayerShooting.m_UnmodifiedCurrentFireRate;
                break;

            case PowerUpType.SHIELD:
                break;

            case PowerUpType.SLOW_MO:
                Time.timeScale = 1f;
                break;

            case PowerUpType.SCORE_MULTIPLIER:
                InGameManager.Instance.m_ScoreMultiplier /= 2;
                break;
        }
    }
}
