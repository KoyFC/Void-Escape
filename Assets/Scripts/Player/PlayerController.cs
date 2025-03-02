using UnityEngine;

[RequireComponent(typeof(PlayerInputScript))]
[RequireComponent(typeof(PlayerMovementScript))]
[RequireComponent(typeof(PlayerHealthScript))]
public class PlayerController : MonoBehaviour
{
    public PlayerInputScript m_PlayerInput = null;
    internal PlayerMovementScript m_PlayerMovement = null;
    internal PlayerHealthScript m_PlayerHealth = null;

    void Awake()
    {
        GetAllComponents();
    }

    private void GetAllComponents()
    {
        m_PlayerInput = GetComponent<PlayerInputScript>();
        m_PlayerMovement = GetComponent<PlayerMovementScript>();
        m_PlayerHealth = GetComponent<PlayerHealthScript>();
    }
}
