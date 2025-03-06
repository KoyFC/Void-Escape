using UnityEngine;
using System;
using System.Collections;
using CandyCoded.HapticFeedback;

public class ObstacleController : MonoBehaviour
{
    [SerializeField] private ObstacleData m_ObstacleData;
    [SerializeField] private Vector3 m_Scale = Vector3.one;
    private float m_LocalDifficulty = 1;

    public static event Action<float> OnAsteroidDestroyed; // Event that gives or takes confidence
    public static event Action<PowerUpType> OnItemCollected; // Event that gives the player the ability to use an item
    public static event Action<int> OnAddScore;

    private float m_Speed;

    private float m_RotationSpeed;
    private Vector3 m_RotationAxis;

    private void Start()
    {
        transform.localScale = m_Scale;
    }

    private void OnEnable()
    {
        InGameManager.Instance.OnDifficultyChanged += HandleDifficultyChange;

        m_RotationAxis = new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f));

        m_Speed = m_LocalDifficulty * UnityEngine.Random.Range(m_ObstacleData.m_MinSpeed, m_ObstacleData.m_MaxSpeed);
        m_RotationSpeed = UnityEngine.Random.Range(m_ObstacleData.m_MinRotationSpeed, m_ObstacleData.m_MaxRotationSpeed);

        StartCoroutine(DeactivateObstacle());
    }

    private void OnDisable()
    {
        InGameManager.Instance.OnDifficultyChanged -= HandleDifficultyChange;
    }

    void Update()
    {
        Quaternion rotation = Quaternion.AngleAxis(m_RotationSpeed * Time.deltaTime, m_RotationAxis);
        transform.rotation *= rotation;

        transform.position += -Vector3.forward * m_Speed * Time.deltaTime;
    }

    private void HandleDifficultyChange(int newDifficulty)
    {
        if (newDifficulty > 1) return;
        m_LocalDifficulty = newDifficulty / 1.5f;
    }

    private IEnumerator DeactivateObstacle()
    {
        yield return new WaitForSeconds(m_ObstacleData.m_LifeTime);
        ObstaclePoolManager.Instance.ReturnToPool(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StopAllCoroutines();
            ObstaclePoolManager.Instance.ReturnToPool(gameObject);

            if (m_ObstacleData.m_ObstacleType == ObstacleData.ObstacleType.ASTEROID)
            {
                OnAsteroidDestroyed?.Invoke(-m_ObstacleData.m_LostConfidenceOnCollision);
                OnAddScore?.Invoke(-m_ObstacleData.m_ScoreOnCollision);
                Handheld.Vibrate();
            }
            else
            {
                OnItemCollected?.Invoke(m_ObstacleData.m_PowerUpType);
                HapticFeedback.MediumFeedback();
            }
        }
        else if (collision.gameObject.CompareTag("Projectile"))
        {
            StopAllCoroutines();
            ObstaclePoolManager.Instance.ReturnToPool(gameObject);

            if (m_ObstacleData.m_ObstacleType == ObstacleData.ObstacleType.ASTEROID)
            {
                OnAsteroidDestroyed?.Invoke(m_ObstacleData.m_GainedConfidenceOnDestroy);
                OnAddScore?.Invoke(m_ObstacleData.m_ScoreOnDestroy);
            }
        }
        else if (collision.gameObject.CompareTag("Shield"))
        {
            if (m_ObstacleData.m_ObstacleType == ObstacleData.ObstacleType.ASTEROID)
            {
                StopAllCoroutines();
                ObstaclePoolManager.Instance.ReturnToPool(gameObject);

                OnAsteroidDestroyed?.Invoke(m_ObstacleData.m_GainedConfidenceOnDestroy);
                OnAddScore?.Invoke(m_ObstacleData.m_ScoreOnDestroy);
            }
        }
    }
}