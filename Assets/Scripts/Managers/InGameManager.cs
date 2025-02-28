using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance = null;

    [SerializeField] private InputActionAsset m_InputActions = null;
    private GameObject m_Player = null;

    [Header("Game Settings")]
    public bool m_ChangePerspectiveNow = false;
    public event Action OnPerspectiveChanged;

    [Header("Player Settings")]
    public float m_MovementLerpDuration = 0.05f;
    public float m_RotationLerpDuration = 0.2f;

    [HideInInspector] public bool m_IsHorizontal = true;
    public bool m_InvertedControls = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        SpawnSpaceship();
        ApplyPlayerComponents();
    }

    private void Update()
    {
        if (m_ChangePerspectiveNow)
        {
            m_ChangePerspectiveNow = false;
            m_IsHorizontal = !m_IsHorizontal;
            OnPerspectiveChanged?.Invoke();
        }
    }

    private void SpawnSpaceship()
    {
        Transform spawnPoint = PointManager.Instance.m_Points.CenterPoint;

        m_Player = SpaceshipManager.Instance.InstantiateCurrentSpaceship(spawnPoint.position, spawnPoint.rotation);
    }

    private void ApplyPlayerComponents()
    {
        m_Player.AddComponent<PlayerController>();

        m_Player.GetComponent<PlayerInput>().actions = m_InputActions;
    }

    private void StartGame()
    {

    }

    private void EndGame()
    {

    }
}
