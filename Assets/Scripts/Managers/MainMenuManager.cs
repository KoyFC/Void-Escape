using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    #region Variables
    public static MainMenuManager Instance = null;

    [Header("Spaceship Model Settings")]
    [SerializeField] private Transform m_Spawnpoint = null;
    [SerializeField] private float m_RotationSpeed = 20f;
    private GameObject m_Spaceship = null;
    private Quaternion m_SpawnRotation = Quaternion.identity;

    [Header("UI Elements")]
    [SerializeField] private CanvasGroup m_MainMenuCanvas = null;
    [SerializeField] private TextMeshProUGUI m_CreditText = null;
    [SerializeField] private TextMeshProUGUI m_LeaderboardText = null;

    [Header("Fading")]
    [SerializeField] private float m_UIFadeTime = 1f;
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
            Destroy(gameObject);
        }

        m_SpawnRotation = m_Spawnpoint.rotation;
    }

    private void Start()
    {
        UpdateLeaderboard(SaveSystem.m_SaveData.leaderboardData);

        InstantiateSpaceship();
    }

    private void Update()
    {
        // Rotate spaceship around the global Y axis
        m_Spaceship.transform.Rotate(Vector3.up, m_RotationSpeed * Time.deltaTime);
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
    #endregion

    #region Helper Methods
    private void InstantiateSpaceship()
    {
        SpaceshipAttributes currentSpaceShip = GameManager.Instance.m_CurrentSpaceShip;

        GameObject spaceshipPrefab = SpaceshipManager.Instance.GetSpaceshipPrefab(currentSpaceShip.shipType, currentSpaceShip.shipColor);

        if (m_Spaceship != null)
        {
            m_SpawnRotation = m_Spaceship.transform.rotation;
            Destroy(m_Spaceship);
        }

        m_Spaceship = Instantiate(spaceshipPrefab, m_Spawnpoint.position, m_SpawnRotation);
    }
    #endregion

    #region UI Methods
    private void UpdateCreditText(int newCredits)
    {
        m_CreditText.text = newCredits.ToString() + (newCredits == 1 ? " credit" : " credits");
    }

    private void UpdateLeaderboard(LeaderboardSaveData data)
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
    #endregion

    #region Public Methods
    public void ChangeCurrentSpaceshipType(int newShipType)
    {
        GameManager.Instance.m_CurrentSpaceShip.shipType = (ShipType)newShipType;
        InstantiateSpaceship();
    }

    public void ChangeCurrentSpaceshipColor(int newShipColor)
    {
        GameManager.Instance.m_CurrentSpaceShip.shipColor = (ShipColor)newShipColor;
        InstantiateSpaceship();
    }

    public void SetShipModelActive(bool active)
    {
        m_Spaceship.SetActive(active);
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
    #endregion
}