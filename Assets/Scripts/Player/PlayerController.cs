using UnityEngine;

[RequireComponent(typeof(PlayerInputScript))]
[RequireComponent(typeof(PlayerMovementScript))]
public class PlayerController : MonoBehaviour
{
    public PlayerInputScript m_PlayerInput = null;
    internal PlayerMovementScript m_PlayerMovement = null;

    void Awake()
    {
        GetAllComponents();
    }

    private void GetAllComponents()
    {
        m_PlayerInput = GetComponent<PlayerInputScript>();
        m_PlayerMovement = GetComponent<PlayerMovementScript>();
    }
}
