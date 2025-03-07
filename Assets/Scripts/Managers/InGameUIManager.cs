using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

public class InGameUIManager : MonoBehaviour
{
    #region Variables
    public static InGameUIManager Instance = null;

    [SerializeField] private GameObject m_BaseCanvas = null;
    [SerializeField] private GameObject m_MotionCanvas = null;
    [SerializeField] private GameObject m_NoMotionCanvas = null;
    [SerializeField] private GameObject m_GameOverScreen = null;

    [Header("Game Stats")]
    [SerializeField] private TextMeshProUGUI m_ScoreText = null;
    [SerializeField] private TextMeshProUGUI m_RoundText = null;

    [Header("Arrows")]
    private Image m_LeftArrow = null;
    private Image m_RightArrow = null;
    [SerializeField] private Image m_MotionLeftArrowImage = null;
    [SerializeField] private Image m_MotionRightArrowImage = null;
    [Space]
    [SerializeField] private Image m_NoMotionLeftArrowImage = null;
    [SerializeField] private Image m_NoMotionRightArrowImage = null;

    [Header("Confidence Slider")]
    public float m_ConfidenceDepletionRate = 5f;
    [SerializeField] private Slider m_ConfidenceSlider = null;

    public static event Action OnConfidenceDepleted;

    [Header("Power-Up Elements")]
    [SerializeField] private Slider m_NoCooldownSlider = null;
    [SerializeField] private Slider m_ShieldSlider = null;
    [SerializeField] private Slider m_SlowMoSlider = null;
    [SerializeField] private TextMeshProUGUI m_ScoreMultiplierText = null;

    [Header("Perspective Change Indicator")]
    [SerializeField] private GameObject m_PerspectiveChangeIndicator = null;
    #endregion

    #region Main Methods
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

#if UNITY_EDITOR
        m_LeftArrow = m_MotionLeftArrowImage;
        m_RightArrow = m_MotionRightArrowImage;
#elif UNITY_ANDROID
        if (GameManager.Instance.m_MotionControls)
        {
            m_LeftArrow = m_MotionLeftArrowImage;
            m_RightArrow = m_MotionRightArrowImage;
        }
        else
        {
            m_LeftArrow = m_NoMotionLeftArrowImage;
            m_RightArrow = m_NoMotionRightArrowImage;
        }
#else 
        m_LeftArrow = m_MotionLeftArrowImage;
        m_RightArrow = m_MotionRightArrowImage;
#endif
    }

    private void OnEnable()
    {
        ObstacleController.OnAsteroidDestroyed += UpdateConfidenceSlider;

        InGameManager.Instance.OnAlmostPerspectiveChanged += FlashPerspectiveChangeIndicator;
        InGameManager.Instance.OnScoreChanged += UpdateScoreText;
        InGameManager.Instance.OnDifficultyChanged += UpdateRoundText;

        PlayerItemController.OnNoCooldwnTimeChanged += UpdateNoCooldownSlider;
        PlayerItemController.OnShieldTimeChanged += UpdateShieldSlider;
        PlayerItemController.OnSlowMoTimeChanged += UpdateSlowMoSlider;
        PlayerItemController.OnScoreMultiplierChanged += UpdateScoreMultiplierText;
    }

    private void OnDisable()
    {
        ObstacleController.OnAsteroidDestroyed -= UpdateConfidenceSlider;

        InGameManager.Instance.OnAlmostPerspectiveChanged -= FlashPerspectiveChangeIndicator;
        InGameManager.Instance.OnScoreChanged -= UpdateScoreText;
        InGameManager.Instance.OnDifficultyChanged -= UpdateRoundText;

        PlayerItemController.OnNoCooldwnTimeChanged -= UpdateNoCooldownSlider;
        PlayerItemController.OnShieldTimeChanged -= UpdateShieldSlider;
        PlayerItemController.OnSlowMoTimeChanged -= UpdateSlowMoSlider;
        PlayerItemController.OnScoreMultiplierChanged -= UpdateScoreMultiplierText;
    }

    void Start()
    {
        float maxConfidence = InGameManager.Instance.m_MaxConfidence;

        m_ConfidenceSlider.maxValue = maxConfidence;
        m_ConfidenceSlider.value = maxConfidence;

        m_NoCooldownSlider.maxValue = 7.5f;
        m_NoCooldownSlider.value = 0f;
        m_NoCooldownSlider.gameObject.SetActive(false);

        m_ShieldSlider.maxValue = 10f;
        m_ShieldSlider.value = 0f;
        m_ShieldSlider.gameObject.SetActive(false);

        m_SlowMoSlider.maxValue = 10f;
        m_SlowMoSlider.value = 0f;
        m_SlowMoSlider.gameObject.SetActive(false);

    }

    private void Update()
    {
        if (m_ConfidenceSlider.gameObject.activeInHierarchy && !InGameManager.Instance.m_ChangingPerspective)
        {
            m_ConfidenceSlider.value -= (m_ConfidenceDepletionRate / 100f) * InGameManager.Instance.m_MaxConfidence * Time.deltaTime;
        }

        if (m_ConfidenceSlider.gameObject.activeInHierarchy && m_ConfidenceSlider.value <= 0)
        {
            StartCoroutine(EnableGameOverScreen());
            OnConfidenceDepleted?.Invoke();
            Time.timeScale = 1f;
        }
    }
    #endregion

    #region UI Updates
    private void FlashPerspectiveChangeIndicator()
    {
        StartCoroutine(FlashIndicator());
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

    private void UpdateNoCooldownSlider(float newValue)
    {
        m_NoCooldownSlider.value = newValue;
        SetSliderActive(m_NoCooldownSlider);
    }

    private void UpdateShieldSlider(float newValue)
    {
        m_ShieldSlider.value = newValue;
        SetSliderActive(m_ShieldSlider);
    }

    private void UpdateSlowMoSlider(float newValue)
    {
        m_SlowMoSlider.value = newValue;
        SetSliderActive(m_SlowMoSlider);
    }

    private void UpdateScoreMultiplierText(int multiplier)
    {
        m_ScoreMultiplierText.text = "x" + multiplier;

        if (multiplier <= 1)
        {
            m_ScoreMultiplierText.gameObject.SetActive(false);
        }
        else
        {
            m_ScoreMultiplierText.gameObject.SetActive(true);
        }
    }

    private void SetSliderActive(Slider slider)
    {
        if (slider.value <= 0)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            slider.gameObject.SetActive(true);
        }
    }
    #endregion

    #region UI Elements
    public void EnableUIElements()
    {
        m_BaseCanvas.SetActive(true);

#if UNITY_EDITOR
        m_MotionCanvas.SetActive(true);
        m_NoMotionCanvas.SetActive(false);
#elif UNITY_ANDROID
        if (GameManager.Instance.m_MotionControls)
        {
            m_MotionCanvas.SetActive(true);
            m_NoMotionCanvas.SetActive(false);
        }
        else
        {
            m_MotionCanvas.SetActive(false);
            m_NoMotionCanvas.SetActive(true);
        }
#else
        m_MotionCanvas.SetActive(true);
        m_NoMotionCanvas.SetActive(false);
#endif
    }

    public void DisableUIElements()
    {
        m_BaseCanvas.SetActive(false);
        m_MotionCanvas.SetActive(false);
        m_NoMotionCanvas.SetActive(false);
    }

    public IEnumerator EnableGameOverScreen()
    {
        DisableUIElements();
        m_BaseCanvas.SetActive(false);

        yield return new WaitForSeconds(2f);
        m_GameOverScreen.SetActive(true);
    }
    #endregion

    #region Perspective Change Indicator
    private IEnumerator FlashIndicator()
    {
        for (int i = 0; i < 4; i++)
        {
            m_PerspectiveChangeIndicator.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            m_PerspectiveChangeIndicator.SetActive(false);
            yield return new WaitForSeconds(0.4f);
        }
    }
    #endregion

    #region Arrow Rotation
    public void RotateArrows(bool returnToOriginal)
    {
        StartCoroutine(RotateSprite(m_LeftArrow, -90f, returnToOriginal));
        StartCoroutine(RotateSprite(m_RightArrow, -90f, returnToOriginal));
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
