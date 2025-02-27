using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    #region Structures
    [System.Serializable]
    private struct ShipTypeSettingsButton
    {
        public ShipType shipType;
        public Button button;
        public GameObject lockIconObject;
        public Image lockIconImage;
    }

    [System.Serializable]
    private struct ShipColorSettingsButton
    {
        public ShipColor shipColor;
        public Button button;
        public GameObject lockIconObject;
        public Image lockIconImage;
    }
    #endregion

    #region Variables
    public static MainMenuManager Instance = null;

    [Header("Spaceship Model Settings")]
    [SerializeField] private Transform m_Spawnpoint = null;
    [SerializeField] private float m_RotationSpeed = 20f;
    private GameObject m_Spaceship = null;
    private Quaternion m_SpawnRotation = Quaternion.identity;

    [Header("Fading")]
    [SerializeField] private float m_UIFadeTime = 1f;

    [Header("UI Elements")]
    [SerializeField] private CanvasGroup m_MainMenuCanvas = null;
    [SerializeField] private TextMeshProUGUI m_CreditText = null;
    [SerializeField] private TextMeshProUGUI m_LeaderboardText = null;
    [SerializeField] private ShipTypeSettingsButton[] m_ShipTypeButtons = null;
    [SerializeField] private ShipColorSettingsButton[] m_ShipColorButtons = null;
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

        UpdateLocks(); // Move to use events
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

    private void UpdateLocks()
    {
        UpdateShipTypeLocks();
        UpdateShipColorLocks();
    }

    private void UpdateShipTypeLocks()
    {
        for (int i = 0; i < m_ShipTypeButtons.Length; i++)
        {
            ShipTypeSettingsButton button = m_ShipTypeButtons[i];

            if (i < GameManager.Instance.m_UnlockedShips.Count && GameManager.Instance.m_UnlockedShips[(int)button.shipType])
            {
                button.lockIconImage.enabled = false;
                button.lockIconImage.raycastTarget = false;
                button.button.interactable = true;

                foreach (Transform child in button.lockIconObject.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            else
            {
                button.lockIconImage.enabled = true;
                button.lockIconImage.raycastTarget = true;
                button.button.interactable = false;

                foreach (Transform child in button.lockIconObject.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            m_ShipTypeButtons[i] = button;
        }
    }

    private void UpdateShipColorLocks()
    {
        for (int i = 0; i < m_ShipColorButtons.Length; i++)
        {
            ShipColorSettingsButton button = m_ShipColorButtons[i];

            if (i < GameManager.Instance.m_UnlockedColors.Count && GameManager.Instance.m_UnlockedColors[(int)button.shipColor])
            {
                button.lockIconImage.enabled = false;
                button.lockIconImage.raycastTarget = false;
                button.button.interactable = true;

                foreach (Transform child in button.lockIconObject.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            else
            {
                button.lockIconImage.enabled = true;
                button.lockIconImage.raycastTarget = true;
                button.button.interactable = false;

                foreach (Transform child in button.lockIconObject.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            m_ShipColorButtons[i] = button;
        }
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

    public void UnlockShipType(int shipType)
    {
        GameManager.Instance.UnlockShipType(shipType);
    }

    public void UnlockShipColor(int shipColor)
    {
        GameManager.Instance.UnlockShipColor(shipColor);
    }

    public void LockShipType(int shipType)
    {
        GameManager.Instance.LockShipType(shipType);
    }

    public void LockShipColor(int shipColor)
    {
        GameManager.Instance.LockShipColor(shipColor);
    }
    #endregion
}