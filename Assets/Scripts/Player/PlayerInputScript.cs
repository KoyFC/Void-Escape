using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputScript : MonoBehaviour
{
    public static PlayerInputScript Instance = null;

    private PlayerInput m_PlayerInput = null;

    [HideInInspector] public bool m_PreviousPressed = false;
    [HideInInspector] public bool m_NextPressed = false;
    [HideInInspector] public bool m_FireHeld = false;
    [HideInInspector] public bool m_Fire2Held = false;
    [HideInInspector] public Vector3 m_Accelerometer = Vector3.zero;
    [HideInInspector] public Vector3 m_Gyroscope = Vector3.zero;

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

        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
        }

        if (UnityEngine.InputSystem.Gyroscope.current != null)
        {
            InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
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

    void OnDisable()
    {
        m_FireHeld = false;
    }

    private void HandleInput()
    {
        m_PreviousPressed = m_PlayerInput.actions["Previous"].WasPressedThisFrame();

        m_NextPressed = m_PlayerInput.actions["Next"].WasPressedThisFrame();

        if (InGameManager.Instance != null && InGameManager.Instance.m_InvertedControls)
        {
            bool aux = m_PreviousPressed;
            m_PreviousPressed = m_NextPressed;
            m_NextPressed = aux;
        }

        m_Fire2Held = m_PlayerInput.actions["Fire2"].IsPressed();
        m_FireHeld = m_PlayerInput.actions["Fire"].IsPressed() && !m_Fire2Held;

        if (Accelerometer.current != null)
        {
            m_Accelerometer = Accelerometer.current.acceleration.ReadValue();
        }
        if (UnityEngine.InputSystem.Gyroscope.current != null)
        {
            m_Gyroscope = UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue();
        }
    }

    private void HandleEvents()
    {
        if (m_PreviousPressed || m_NextPressed 
            || Mathf.Abs(m_Accelerometer.x) > 0.05f || Mathf.Abs(m_Accelerometer.y) > 0.05f)
        {
            OnMovementPressed?.Invoke();
        }
    }

    public void DisableInput()
    {
        this.enabled = false;
    }
}
