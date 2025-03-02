using UnityEngine;
using System.Collections;

public class PlayerHealthScript : MonoBehaviour
{
    private PlayerController m_PlayerController = null;
    private PlayerMovementScript m_PlayerMovement = null;

    internal bool m_Hit = false;
    private float m_HitDisplacement = 5.5f;
    private float m_HitDuration = 0.075f;

    private void Start()
    {
        m_PlayerController = GetComponent<PlayerController>();
        m_PlayerMovement = m_PlayerController.m_PlayerMovement;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            StartCoroutine(HitCoroutine());
        }
    }

    private IEnumerator HitCoroutine()
    {
        // Lerp slightly backwards in the Z axis and then return to the original position
        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z - m_HitDisplacement);

        float elapsedTime = 0f;

        m_PlayerMovement.m_IsMoving = true;
        m_Hit = true;

        while (elapsedTime < m_HitDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / m_HitDuration);
            yield return null;
        }

        transform.position = targetPosition;

        elapsedTime = 0f;

        while (elapsedTime < m_HitDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(targetPosition, originalPosition, elapsedTime / m_HitDuration);
            yield return null;
        }

        transform.position = originalPosition;

        m_PlayerMovement.m_IsMoving = false;
        m_Hit = false;
    }
}
