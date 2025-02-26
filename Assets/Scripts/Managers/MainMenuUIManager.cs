using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using TMPro;

public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager Instance = null;

    [Header("UI Elements")]
    [SerializeField] private CanvasGroup m_MainMenuCanvas = null;
    [SerializeField] private TextMeshProUGUI m_CreditText = null;
    [SerializeField] private TextMeshProUGUI m_LeaderboardText = null;

    [Header("Fading")]
    [SerializeField] private float m_UIFadeTime = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        CurrencyManager.Instance.OnCreditsChanged += UpdateCreditText;
        UpdateCreditText(CurrencyManager.Instance.Credits);
    }

    private void OnDisable()
    {
        CurrencyManager.Instance.OnCreditsChanged -= UpdateCreditText;
    }

    private void UpdateCreditText(int newCredits)
    {
        m_CreditText.text = newCredits.ToString() + (newCredits == 1 ? " credit" : " credits");
    }

    public void UpdateLeaderboard(LeaderboardSaveData data)
    {
        StringBuilder sb = new();

        for (int i = 0; i < data.leaderboard.Count; i++)
        {
            sb.Append((i + 1).ToString())
              .Append(". ")
              .Append(data.leaderboard[i].playerName)
              .Append(" - ")
              .Append(data.leaderboard[i].score.ToString())
              .Append(" points\n");
        }

        for (int i = data.leaderboard.Count; i < 10; i++)
        {
            sb.Append((i + 1).ToString())
              .Append(". \n");
        }

        m_LeaderboardText.text = sb.ToString();
    }

    public void SceneTransitionForUI(string sceneName)
    {
        StartCoroutine(SceneTransitionCoroutine(sceneName));
    }

    private IEnumerator SceneTransitionCoroutine(string sceneName)
    {
        // Fade out the UI
        float elapsedTime = 0f;
        while (elapsedTime < m_UIFadeTime)
        {
            elapsedTime += Time.deltaTime;
            m_MainMenuCanvas.alpha = Mathf.Lerp(1f, 0f, elapsedTime / m_UIFadeTime);
            yield return null;
        }
        m_MainMenuCanvas.alpha = 0f;

        // Fade in the black panel and load the new scene
        FadeManager.Instance.FadeOutAndLoadScene(sceneName);
    }
}