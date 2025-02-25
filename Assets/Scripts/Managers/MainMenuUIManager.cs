using System.Text;
using UnityEngine;
using TMPro;

public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager Instance = null;

    [SerializeField] private TextMeshProUGUI m_CreditText = null;
    [SerializeField] private TextMeshProUGUI m_LeaderboardText = null;

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
}