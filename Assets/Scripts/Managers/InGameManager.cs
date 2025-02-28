using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance = null;

    [SerializeField] private InputActionAsset m_InputActions = null;
    private GameObject m_Player = null;
    private PlayerController m_PlayerController = null;
    private Animator m_CameraStateAnimator = null;

    [Header("Game Stats")]
    [SerializeField] private int m_Score = 0;


    [Header("Game Settings")]
    public bool m_ChangePerspectiveNow = false;
    [HideInInspector] public bool m_IsHorizontal = true;
    public bool m_InvertedControls = false;
    public event Action OnPerspectiveChanged;

    [Header("Player Settings")]
    public float m_MovementLerpDuration = 0.1f;
    public float m_RotationLerpDuration = 0.1f;


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

        m_CameraStateAnimator = GetComponent<Animator>();

        SpawnSpaceship();
        ApplyPlayerComponents();
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (m_ChangePerspectiveNow)
        {
            m_ChangePerspectiveNow = false;
            ChangePerspective();
        }
    }

    private void SpawnSpaceship()
    {
        Transform spawnPoint = PointManager.Instance.m_Points.CenterPoint;

        m_Player = SpaceshipManager.Instance.InstantiateCurrentSpaceship(spawnPoint.position, spawnPoint.rotation);
    }

    private void ApplyPlayerComponents()
    {
        m_PlayerController = m_Player.AddComponent<PlayerController>();

        m_Player.GetComponent<PlayerInput>().actions = m_InputActions;
    }

    private void StartGame()
    {
        m_Score = 0;

        InvokeRepeating("ChangePerspective", 5, 5);
    }

    private void EndGame()
    {

    }

    private void ChangePerspective()
    {
        m_CameraStateAnimator.SetTrigger("ChangePerspective");

        m_IsHorizontal = !m_IsHorizontal;

        OnPerspectiveChanged?.Invoke();
    }
}
