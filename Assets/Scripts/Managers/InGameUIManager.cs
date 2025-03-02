using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance = null;

    [Header("Game Stats")]
    [SerializeField] private TextMeshProUGUI m_ScoreText = null;

    [Header("Arrows")]
    [SerializeField] private Image m_LeftArrowImage = null;
    [SerializeField] private Image m_RightArrowImage = null;

    [Header("Slider")]
    [SerializeField] private float m_ConfidenceDepletionRate = 5f;
    [SerializeField] private Slider m_ConfidenceSlider = null;

    public event Action OnConfidenceDepleted;

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
    }

    private void OnDisable()
    {
        ObstacleController.OnAsteroidDestroyed -= UpdateConfidenceSlider;
        InGameManager.Instance.OnScoreChanged -= UpdateScoreText;
    }

    void Start()
    {
        float maxConfidence = InGameManager.Instance.m_MaxConfidence;

        m_ConfidenceSlider.maxValue = maxConfidence;
        m_ConfidenceSlider.value = maxConfidence;
    }

    private void Update()
    {
        if (m_ConfidenceSlider.gameObject.activeSelf)
        {
            m_ConfidenceSlider.value -= m_ConfidenceDepletionRate * Time.deltaTime;
        }
    }

    private void UpdateScoreText(int newScore)
    {
        m_ScoreText.text = "Score: " + newScore;
    }

    private void UpdateConfidenceSlider(float newValue)
    {
        m_ConfidenceSlider.value += newValue;
    }

    public void EnableUIElements()
    {
        m_ScoreText.enabled = true;
        m_LeftArrowImage.enabled = true;
        m_RightArrowImage.enabled = true;
        m_ConfidenceSlider.gameObject.SetActive(true);
    }

    public void DisableUIElements()
    {
        m_ScoreText.enabled = false;
        m_LeftArrowImage.enabled = false;
        m_RightArrowImage.enabled = false;
        m_ConfidenceSlider.gameObject.SetActive(false);
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
