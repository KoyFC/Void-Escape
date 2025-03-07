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
        transform.position += transform.up * m_Speed * Time.deltaTime;
    }

    private IEnumerator DeactivateProjectile()
    {
        yield return new WaitForSeconds(m_LifeTime);
        ProjectilePoolManager.Instance.ReturnToPool(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool collidedWithObstacle = collision.gameObject.CompareTag("Asteroid") || collision.gameObject.CompareTag("Item");

        if (collidedWithObstacle)
        {
            StopAllCoroutines();
            ProjectilePoolManager.Instance.ReturnToPool(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // We also check for the portal tag. Though even if it collides, we don't destroy the portal.
        // We just prevent the projectile from going through it.
        if (other.CompareTag("Portal"))
        {
            StopAllCoroutines();
            ProjectilePoolManager.Instance.ReturnToPool(gameObject);
        }
    }
}
