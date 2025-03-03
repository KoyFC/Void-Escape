using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance = null;

    [SerializeField] private GameObject m_NormalCanvas = null;
    [SerializeField] private GameObject m_GameOverScreen = null;

    [SerializeField] private GameObject m_MoveButtons = null;
    [SerializeField] private GameObject m_FireButton = null;

    [Header("Game Stats")]
    [SerializeField] private TextMeshProUGUI m_ScoreText = null;
    [SerializeField] private TextMeshProUGUI m_RoundText = null;

    [Header("Arrows")]
    [SerializeField] private Image m_LeftArrowImage = null;
    [SerializeField] private Image m_RightArrowImage = null;

    [Header("Slider")]
    public float m_ConfidenceDepletionRate = 5f;
    [SerializeField] private Slider m_ConfidenceSlider = null;

    public static event Action OnConfidenceDepleted;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        ObstacleController.OnAsteroidDestroyed += UpdateConfidenceSlider;
        InGameManager.Instance.OnScoreChanged += UpdateScoreText;
        InGameManager.Instance.OnDifficultyChanged += UpdateRoundText;
    }

    private void OnDisable()
    {
        ObstacleController.OnAsteroidDestroyed -= UpdateConfidenceSlider;
        InGameManager.Instance.OnScoreChanged -= UpdateScoreText;
        InGameManager.Instance.OnDifficultyChanged -= UpdateRoundText;
    }

    void Start()
    {
        float maxConfidence = InGameManager.Instance.m_MaxConfidence;

        m_ConfidenceSlider.maxValue = maxConfidence;
        m_ConfidenceSlider.value = maxConfidence;
    }

    private void Update()
    {
        if (m_ConfidenceSlider.gameObject.activeSelf && !InGameManager.Instance.m_ChangingPerspective)
        {
            m_ConfidenceSlider.value -= (m_ConfidenceDepletionRate / 100f) * InGameManager.Instance.m_MaxConfidence * Time.deltaTime;
        }

        if (m_ConfidenceSlider.gameObject.activeSelf && m_ConfidenceSlider.value <= 0)
        {
            StartCoroutine(EnableGameOverScreen());
            OnConfidenceDepleted?.Invoke();
        }
    }

    private void UpdateScoreText(int newScore)
    {
        m_ScoreText.text = "Score: " + newScore;
    }

    private void UpdateRoundText(int round)
    {
        m_RoundText.text = "Round " + round;
    }

    private void UpdateConfidenceSlider(float newValue)
    {
        m_ConfidenceSlider.value += newValue;
    }

    public void EnableUIElements()
    {
        m_ScoreText.gameObject.SetActive(true);
        m_RoundText.gameObject.SetActive(true);
        m_ConfidenceSlider.gameObject.SetActive(true);
        m_MoveButtons.SetActive(true);
        m_FireButton.SetActive(true);
    }

    public void DisableUIElements()
    {
        m_ScoreText.gameObject.SetActive(false);
        m_RoundText.gameObject.SetActive(false);
        m_ConfidenceSlider.gameObject.SetActive(false);
        m_MoveButtons.SetActive(false);
        m_FireButton.SetActive(false);
    }

    public IEnumerator EnableGameOverScreen()
    {
        DisableUIElements();
        m_NormalCanvas.SetActive(false);

        yield return new WaitForSeconds(2f);
        m_GameOverScreen.SetActive(true);
    }

    public void RotateArrows(bool returnToOriginal)
    {
        StartCoroutine(RotateSprite(m_LeftArrowImage, -90f, returnToOriginal));
        StartCoroutine(RotateSprite(m_RightArrowImage, -90f, returnToOriginal));
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
}
