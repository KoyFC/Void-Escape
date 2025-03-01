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

    [Header("Game Settings")]
    [SerializeField] private float m_CinematicStateDuration = 2.5f;
    [SerializeField] private float m_GameStateDuration = 10f;
    public bool m_ChangingPerspective = false;
    [SerializeField] private float m_ObstacleSpawnRate = 1.5f;

    [HideInInspector] public bool m_IsHorizontal = true;
    public bool m_InvertedControls = false;
    public event Action OnPerspectiveChanged;

    [Header("Player Settings")]
    public float m_MovementLerpDuration = 0.1f;
    public float m_RotationLerpDuration = 0.1f;

    [Header("UI Settings")]
    [SerializeField] private Image m_LeftArrowImage = null;
    [SerializeField] private Image m_RightArrowImage = null;
    #endregion

    #region Main Methods
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

    // On start, there's a slight delay before the game starts for a cinematic where the player can't move
    private IEnumerator Start()
    {
        m_LeftArrowImage.enabled = false;
        m_RightArrowImage.enabled = false;
        m_PlayerController.m_PlayerMovement.m_IsMoving = true;

        yield return new WaitForSeconds(m_CinematicStateDuration);

        m_CameraStateAnimator.SetTrigger("ContinueCinematic");

        yield return new WaitForSeconds(m_CinematicStateDuration + 1f);

        StartGame();

        yield return new WaitForSeconds(0.9f);

        m_PlayerController.m_PlayerMovement.m_IsMoving = false;
        m_LeftArrowImage.enabled = true;
        m_RightArrowImage.enabled = true;

        StartCoroutine(SpawnObstacles());
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

    private void AddScore(int score)
    {
        m_Score += score;
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
    }

    public void EndGame()
    {
        StopAllCoroutines();

        // Save score

        // Show end screen

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

            ObstaclePoolManager.Instance.ReturnAllToPool();

            m_CameraStateAnimator.SetTrigger("ChangePerspective");
            m_IsHorizontal = !m_IsHorizontal;

            m_ChangingPerspective = true;
            m_PlayerController.m_PlayerMovement.m_IsMoving = true;

            StartCoroutine(RotateSprite(m_LeftArrowImage, -90f, returnToOriginal));
            StartCoroutine(RotateSprite(m_RightArrowImage, -90f, returnToOriginal));

            yield return new WaitForSeconds(0.75f);

            OnPerspectiveChanged?.Invoke();

            yield return new WaitForSeconds(0.25f);

            m_ChangingPerspective = false;
            m_PlayerController.m_PlayerMovement.m_IsMoving = false;

            returnToOriginal = !returnToOriginal;
        }
    }

    private IEnumerator RotateSprite(Image image, float newZRotation, bool returnToOriginal)
    {
        Quaternion startRotation = image.transform.rotation;

        Quaternion endRotation = returnToOriginal ? 
            Quaternion.Euler(0, 0, 0) :
            Quaternion.Euler(0, 0, newZRotation);

        float elapsedTime = 0.0f;

        while (elapsedTime < 0.5f)
        {
            image.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / 0.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        image.transform.rotation = endRotation;
    }
    #endregion
}
