using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance = null;

    [Header("Game Stats")]
    [SerializeField] private TextMeshProUGUI m_ScoreText = null;

    [Header("Arrows")]
    [SerializeField] private Image m_LeftArrowImage = null;
    [SerializeField] private Image m_RightArrowImage = null;

    [Header("Slider")]
    [SerializeField] private Slider m_HealthSlider = null;

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

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void EnableUIElements()
    {
        //m_ScoreText.enabled = true;
        m_LeftArrowImage.enabled = true;
        m_RightArrowImage.enabled = true;
        m_HealthSlider.enabled = true;
    }

    public void DisableUIElements()
    {
        //m_ScoreText.enabled = false;
        m_LeftArrowImage.enabled = false;
        m_RightArrowImage.enabled = false;
        m_HealthSlider.enabled = false;
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
