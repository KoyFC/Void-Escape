using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InGameManager : MonoBehaviour
{
    #region Variables
    public static InGameManager Instance = null;

    [SerializeField] private InputActionAsset m_InputActions = null;
    private GameObject m_Player = null;
    private PlayerController m_PlayerController = null;
    private Animator m_CameraStateAnimator = null;

    [Header("Game Stats")]
    [SerializeField] private int m_Score = 0;
    public int m_MaxConfidence = 100;
    public event Action<int> OnScoreChanged;

    [Header("Game Settings")]
    [SerializeField] private float m_CinematicStateDuration = 2.5f;
    [SerializeField] private float m_GameStateDuration = 10f;
    public bool m_ChangingPerspective = false;
    [SerializeField] private float m_ObstacleSpawnRate = 1.5f;

    [HideInInspector] public bool m_IsHorizontal = true;
    public bool m_InvertedControls = false;
    public event Action OnPerspectiveChanged; // Event that allows the player to know when to reset its position

    [Header("Player Settings")]
    public float m_MovementLerpDuration = 0.1f;
    public float m_RotationLerpDuration = 0.1f;
    #endregion

    #region Main Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        m_CameraStateAnimator = GetComponent<Animator>();

        SpawnSpaceship();
        ApplyPlayerComponents();
    }

    // On start, there's a slight delay before the game starts for a cinematic where the player can't move
    private IEnumerator Start()
    {
        InGameUIManager.Instance.DisableUIElements();
        m_PlayerController.m_PlayerMovement.m_IsMoving = true;

        yield return new WaitForSeconds(m_CinematicStateDuration);

        m_CameraStateAnimator.SetTrigger("ContinueCinematic");

        yield return new WaitForSeconds(m_CinematicStateDuration + 1f);

        StartGame();

        yield return new WaitForSeconds(0.9f);

        InGameUIManager.Instance.EnableUIElements();
        m_PlayerController.m_PlayerMovement.m_IsMoving = false;

        StartCoroutine(SpawnObstacles());
        StartCoroutine(AddScoreRepeatedly());
    }

    private void OnEnable()
    {
        ObstacleController.OnAddScore += AddScore;
    }

    private void OnDisable()
    {
        ObstacleController.OnAddScore -= AddScore;
    }
    #endregion

    #region Obstacle Methods
    private IEnumerator SpawnObstacles()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_ObstacleSpawnRate);
            yield return new WaitUntil(() => !m_ChangingPerspective);
            SpawnRandomPortal();
        }
    }

    private void SpawnRandomPortal()
    {
        if (m_ChangingPerspective) return;

        Transform spawnPoint = null;
        if (m_IsHorizontal)
        {
            spawnPoint = PointManager.Instance.GetRandomHorizontalPoint();
        }
        else
        {
            spawnPoint = PointManager.Instance.GetRandomVerticalPoint();
        }

        GameObject portal = ObstaclePoolManager.Instance.GetPortal();
        portal.transform.position = spawnPoint.position;
    }

    private IEnumerator AddScoreRepeatedly()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            yield return new WaitUntil(() => !m_ChangingPerspective);
            AddScore(1);
        }
    }

    private void AddScore(int score)
    {
        m_Score += score;
        OnScoreChanged?.Invoke(m_Score);
    }
    #endregion

    #region Helper Methods
    private void SpawnSpaceship()
    {
        Transform spawnPoint = PointManager.Instance.m_PlayerPoints.CenterPoint;

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
        m_CameraStateAnimator.SetTrigger("StartGame");

        StartCoroutine(ChangePerspective());
        m_PlayerController.m_PlayerShooting.m_CanFire = true;
    }

    public void EndGame()
    {
        StopAllCoroutines();

        // Save score

        // Show end screen
        Time.timeScale = 0f;

        // Add score to leaderboard
    }
    #endregion

    #region Coroutines
    private IEnumerator ChangePerspective()
    {
        bool returnToOriginal = false; // Used to determine if the arrows should rotate back to their original position
        yield return new WaitForSeconds(1);

        while (true)
        {
            yield return new WaitForSeconds(m_GameStateDuration - 1);

            yield return new WaitUntil(() => !m_PlayerController.m_PlayerMovement.m_IsMoving);
            m_PlayerController.m_PlayerShooting.StopAllCoroutines(); // To prevent the player from shooting while changing perspective

            // Change perspective, moving obstacles out of the way and making the player unable to shoot or move
            m_CameraStateAnimator.SetTrigger("ChangePerspective");
            m_IsHorizontal = !m_IsHorizontal;
            m_ChangingPerspective = true;

            m_PlayerController.m_PlayerShooting.m_CanFire = false;
            m_PlayerController.m_PlayerMovement.m_IsMoving = true;

            ObstaclePoolManager.Instance.MoveOutOfTheWay(m_IsHorizontal);

            // Rotating the UI arrows
            InGameUIManager.Instance.RotateArrows(returnToOriginal);

            yield return new WaitForSeconds(0.75f);

            OnPerspectiveChanged?.Invoke();

            yield return new WaitForSeconds(0.25f);

            m_ChangingPerspective = false;
            m_PlayerController.m_PlayerMovement.m_IsMoving = false;
            m_PlayerController.m_PlayerShooting.m_CanFire = true;

            returnToOriginal = !returnToOriginal;

            if (returnToOriginal)
            {
                m_MaxConfidence = (int) (m_MaxConfidence * 1.1f);
                m_ObstacleSpawnRate *= 1.1f;
                Debug.Log("Increased difficulty");
            }
        }
    }
    #endregion
}
