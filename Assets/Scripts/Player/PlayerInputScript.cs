using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputScript : MonoBehaviour
{
    public static PlayerInputScript Instance = null;

    private PlayerInput m_PlayerInput = null;

    [HideInInspector] public bool m_PreviousPressed = false;
    [HideInInspector] public bool m_NextPressed = false;
    [HideInInspector] public bool m_FireHeld = false;

    public event Action OnMovementPressed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        m_PlayerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        HandleInput();
        HandleEvents();
    }

    private void HandleInput()
    {
        m_PreviousPressed =
            m_PlayerInput.actions["Left"].WasPressedThisFrame() ||
            m_PlayerInput.actions["Up"].WasPressedThisFrame();

        m_NextPressed = 
            m_PlayerInput.actions["Right"].WasPressedThisFrame() ||
            m_PlayerInput.actions["Down"].WasPressedThisFrame();

        if (InGameManager.Instance.m_InvertedControls)
        {
            bool aux = m_PreviousPressed;
            m_PreviousPressed = m_NextPressed;
            m_NextPressed = aux;
        }

        m_FireHeld = m_PlayerInput.actions["Fire"].IsPressed();
    }

    private void HandleEvents()
    {
        if (m_PreviousPressed || m_NextPressed)
        {
            OnMovementPressed?.Invoke();
        }
    }
}
