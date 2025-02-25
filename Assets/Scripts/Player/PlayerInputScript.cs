using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputScript : MonoBehaviour
{
    public static PlayerInputScript Instance = null;

    private PlayerInput m_PlayerInput = null;

    [HideInInspector] public Vector2 m_Movement = Vector2.zero;

    void Start()
    {
        m_PlayerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        m_Movement = m_PlayerInput.actions["Move"].ReadValue<Vector2>();
    }
}
