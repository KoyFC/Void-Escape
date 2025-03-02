using UnityEngine;
using System.Collections;

public class ObstacleController : MonoBehaviour
{
    [SerializeField] private ObstacleData m_ObstacleData;
    [SerializeField] private Vector3 m_Scale = Vector3.one;

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
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f));

        m_Speed = Random.Range(m_ObstacleData.m_MinSpeed, m_ObstacleData.m_MaxSpeed);
        m_RotationSpeed = Random.Range(m_ObstacleData.m_MinRotationSpeed, m_ObstacleData.m_MaxRotationSpeed);

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
        }
    }
}