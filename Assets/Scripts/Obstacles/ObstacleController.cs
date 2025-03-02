using UnityEngine;
using System;
using System.Collections;

public class ObstacleController : MonoBehaviour
{
    [SerializeField] private ObstacleData m_ObstacleData;
    [SerializeField] private Vector3 m_Scale = Vector3.one;

    public static event Action<float> OnAsteroidDestroyed;
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
        m_RotationAxis = new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f));

        m_Speed = UnityEngine.Random.Range(m_ObstacleData.m_MinSpeed, m_ObstacleData.m_MaxSpeed);
        m_RotationSpeed = UnityEngine.Random.Range(m_ObstacleData.m_MinRotationSpeed, m_ObstacleData.m_MaxRotationSpeed);

        StartCoroutine(DeactivateObstacle());
    }

    void Update()
    {
        Quaternion rotation = Quaternion.AngleAxis(m_RotationSpeed * Time.deltaTime, m_RotationAxis);
        transform.rotation *= rotation;

        transform.position += -Vector3.forward * m_Speed * Time.deltaTime;
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
            OnAsteroidDestroyed?.Invoke(-m_ObstacleData.m_LostConfidenceOnCollision);
            OnAddScore?.Invoke(-m_ObstacleData.m_ScoreOnCollision);
        }
        else if (collision.gameObject.CompareTag("Projectile"))
        {
            StopAllCoroutines();
            ObstaclePoolManager.Instance.ReturnToPool(gameObject);
            OnAsteroidDestroyed?.Invoke(m_ObstacleData.m_GainedConfidenceOnDestroy);
            OnAddScore?.Invoke(m_ObstacleData.m_ScoreOnDestroy);
        }
    }
}