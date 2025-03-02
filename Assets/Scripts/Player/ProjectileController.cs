using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float m_Speed = 10f;
    [SerializeField] private float m_LifeTime = 2.5f;

    private void OnEnable()
    {
        StartCoroutine(DeactivateProjectile());
    }

    private void Update()
    {
        transform.position += Vector3.forward * m_Speed * Time.deltaTime;
    }

    private IEnumerator DeactivateProjectile()
    {
        yield return new WaitForSeconds(m_LifeTime);
        ProjectilePoolManager.Instance.ReturnToPool(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            StopAllCoroutines();
            ProjectilePoolManager.Instance.ReturnToPool(gameObject);
        }
    }
}
