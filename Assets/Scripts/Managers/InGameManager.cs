using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("UI Settings")]
    [SerializeField] private Image m_LeftArrowImage = null;
    [SerializeField] private Image m_RightArrowImage = null;
    [SerializeField] private Sprite[] m_ArrowSprites = null;


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

        StartCoroutine(ChangePerspective());
    }

    public void EndGame()
    {
        StopAllCoroutines();

        // Save score

        // Show end screen

        // Add score to leaderboard
    }

    private IEnumerator ChangePerspective()
    {
        yield return new WaitForSeconds(1);

        while (true)
        {
            yield return new WaitForSeconds(4);

            // Wait until the player stops moving so we can ensure no variables
            // are modified by the movement script during the transition
            yield return new WaitUntil(() => !m_PlayerController.m_PlayerMovement.m_IsMoving);

            m_CameraStateAnimator.SetTrigger("ChangePerspective");
            m_IsHorizontal = !m_IsHorizontal;

            m_PlayerController.m_PlayerMovement.m_IsMoving = true;

            yield return new WaitForSeconds(0.75f);

            OnPerspectiveChanged?.Invoke(); // Notify the player movement script that the perspective has changed

            m_LeftArrowImage.sprite = m_IsHorizontal ? m_ArrowSprites[2] : m_ArrowSprites[0];
            m_RightArrowImage.sprite = m_IsHorizontal ? m_ArrowSprites[3] : m_ArrowSprites[1];

            yield return new WaitForSeconds(0.25f);

            m_PlayerController.m_PlayerMovement.m_IsMoving = false;
        }
    }
}
