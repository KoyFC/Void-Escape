using UnityEngine;

[RequireComponent(typeof(PlayerInputScript))]
[RequireComponent(typeof(PlayerMovementScript))]
[RequireComponent(typeof(PlayerHealthScript))]
[RequireComponent(typeof(PlayerShootingScript))]
public class PlayerController : MonoBehaviour
{
    internal PlayerMovementScript m_PlayerMovement = null;
    internal PlayerHealthScript m_PlayerHealth = null;
    internal PlayerShootingScript m_PlayerShooting = null;

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
}
