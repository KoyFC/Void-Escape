using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using TMPro;
using System;

public class MainMenuManager : MonoBehaviour
{
    #region Structures
    [System.Serializable]
    private struct ShipTypeButton
    {
        public ShipType shipType;
        public Button button;
        public GameObject iconObject;
        public Image iconImage;
    }

    [System.Serializable]
    private struct ShipColorButton
    {
        public ShipColor shipColor;
        public Button button;
        public GameObject iconObject;
        public Image iconImage;
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

    [Space]
    [SerializeField] private TextMeshProUGUI m_CreditText = null;
    [SerializeField] private TextMeshProUGUI m_LeaderboardText = null;
    [SerializeField] private TMP_InputField m_InputField = null;

    [Space]
    [SerializeField] private TMP_Dropdown m_TargetFramerateDropdown = null;
    [SerializeField] private TMP_Dropdown m_MotionControlsDropdown = null;
    [SerializeField] private TMP_Dropdown m_DynamicResolutionDropdown = null;

    [Space]
    [SerializeField] private ShipTypeButton[] m_ShipTypeSettingsButtons = null;
    [SerializeField] private ShipColorButton[] m_ShipColorSettingsButtons = null;

    [Space]
    [SerializeField] private ShipTypeButton[] m_ShipTypeShopButtons = null;
    [SerializeField] private ShipColorButton[] m_ShipColorShopButtons = null;
    [SerializeField] private Sprite m_CartImage = null, m_SoldImage = null;
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
        m_InputField.text = GameManager.Instance.m_CurrentName;

        InstantiateSpaceship();
        UpdateAllUIElements();
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

        if (m_Spaceship != null)
        {
            m_SpawnRotation = m_Spaceship.transform.rotation;
            Destroy(m_Spaceship);
        }

        Vector3 spawnPosition = m_Spawnpoint.position + SpaceshipManager.Instance.GetSpawnOffset(currentSpaceShip.shipType);
        
        m_Spaceship = SpaceshipManager.Instance.InstantiateCurrentSpaceship(spawnPosition, m_SpawnRotation);
    }
    #endregion

    #region UI Methods
    private void UpdateAllUIElements()
    {
        m_InputField.text = GameManager.Instance.m_CurrentName;

        UpdateTargetFramerateDropdown();
        UpdateMotionControlsDropdown();
        UpdateDynamicResolutionDropdown();

        UpdateShipTypeLocks();
        UpdateShipColorLocks();
        UpdateShipTypeShop();
        UpdateShipColorShop();

    }

    private void UpdateTargetFramerateDropdown()
    {
        switch (Application.targetFrameRate)
        {
            case -1:
                m_TargetFramerateDropdown.value = 0;
                break;
            case 60:
                m_TargetFramerateDropdown.value = 1;
                break;
            case 120:
                m_TargetFramerateDropdown.value = 2;
                break;
            case 144:
                m_TargetFramerateDropdown.value = 3;
                break;
            case 200:
                m_TargetFramerateDropdown.value = 4;
                break;
            default:
                m_TargetFramerateDropdown.value = 1;
                break;
        }
    }

    private void UpdateMotionControlsDropdown()
    {
        m_MotionControlsDropdown.value = GameManager.Instance.m_MotionControls ? 0 : 1;
    }

    private void UpdateDynamicResolutionDropdown()
    {
        m_DynamicResolutionDropdown.value = GameManager.Instance.m_DynamicResolutionActive ? 1 : 0;
    }

    private void UpdateCreditText(int newCredits)
    {
        m_CreditText.text = newCredits.ToString() + (newCredits == 1 ? " credit" : " credits");
    }

    private void UpdateLeaderboard(LeaderboardSaveData data)
    {
        // Sorting using the method that is already in the SaveSystem
        data.leaderboard.Sort(SaveSystem.ComparePlayerScores);

        StringBuilder sb = new();

        for (int i = 0; i < data.leaderboard.Count; i++)
        {
            if (i >= 10) break;

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

    private void UpdateShipTypeLocks()
    {
        for (int i = 0; i < m_ShipTypeSettingsButtons.Length; i++)
        {
            ShipTypeButton button = m_ShipTypeSettingsButtons[i];

            if (i < GameManager.Instance.m_UnlockedShips.Count && GameManager.Instance.m_UnlockedShips[(int)button.shipType])
            { // Can interact with the button
                button.iconImage.enabled = false;
                button.iconImage.raycastTarget = false;
                button.button.interactable = true;

                foreach (Transform child in button.iconObject.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            else
            { // Cannot interact with the button
                button.iconImage.enabled = true;
                button.iconImage.raycastTarget = true;
                button.button.interactable = false;

                foreach (Transform child in button.iconObject.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            m_ShipTypeSettingsButtons[i] = button;
        }
    }

    private void UpdateShipColorLocks()
    {
        for (int i = 0; i < m_ShipColorSettingsButtons.Length; i++)
        {
            ShipColorButton button = m_ShipColorSettingsButtons[i];

            if (i < GameManager.Instance.m_UnlockedColors.Count && GameManager.Instance.m_UnlockedColors[(int)button.shipColor])
            { // Can interact
                button.iconImage.enabled = false;
                button.iconImage.raycastTarget = false;
                button.button.interactable = true;
            }
            else
            { // Cannot interact
                button.iconImage.enabled = true;
                button.iconImage.raycastTarget = true;
                button.button.interactable = false;
            }
            m_ShipColorSettingsButtons[i] = button;
        }
    }

    private void UpdateShipTypeShop()
    {
        for (int i = 0; i < m_ShipTypeShopButtons.Length; i++)
        {
            ShipTypeButton button = m_ShipTypeShopButtons[i];
            if (i < GameManager.Instance.m_UnlockedShips.Count && GameManager.Instance.m_UnlockedShips[(int)button.shipType])
            { // Cannot interact with the button (ship unlocked)
                button.iconImage.sprite = m_SoldImage;
                button.iconImage.raycastTarget = true;
                button.button.interactable = false;
            }
            else
            { // Can interact with the button (ship locked)
                button.iconImage.sprite = m_CartImage;
                button.iconImage.raycastTarget = false;
                button.button.interactable = true;
            }
            m_ShipTypeShopButtons[i] = button;
        }
    }

    private void UpdateShipColorShop()
    {
        for (int i = 0; i < m_ShipColorShopButtons.Length; i++)
        {
            ShipColorButton button = m_ShipColorShopButtons[i];
            if (i < GameManager.Instance.m_UnlockedColors.Count && GameManager.Instance.m_UnlockedColors[(int)button.shipColor])
            { // Cannot interact with the button (color unlocked)
                button.iconImage.sprite = m_SoldImage;
                button.iconImage.raycastTarget = true;
                button.button.interactable = false;
            }
            else
            { // Can interact with the button (color locked)
                button.iconImage.sprite = m_CartImage;
                button.iconImage.raycastTarget = false;
                button.button.interactable = true;
            }
            m_ShipColorShopButtons[i] = button;
        }
    }
    #endregion

    #region Public Methods
    public void SetCurrentName(string name)
    {
        if (name == "")
        {
            m_InputField.text = GameManager.Instance.m_CurrentName;
            return;
        }
        GameManager.Instance.m_CurrentName = name;
    }

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

    public void SetTargetFramerate(int fps)
    {
        switch (fps)
        {
            case 0:
                fps = -1;
                break;
            case 1:
                fps = 60;
                break;
            case 2:
                fps = 120;
                break;
            case 3:
                fps = 144;
                break;
            default:
                fps = 200;
                break;
        }

        Application.targetFrameRate = fps;
    }

    public void SetMotionControls(int motionControls)
    {
        switch (motionControls)
        {
            case 0:
                GameManager.Instance.m_MotionControls = true;
                break;
            case 1:
                GameManager.Instance.m_MotionControls = false;
                break;
        }
    }

    public void SetGraphicsMode(int graphicsMode)
    {
        switch (graphicsMode)
        {
            case 0:
                GameManager.Instance.m_DynamicResolutionActive = false;
                break;
            case 1:
                GameManager.Instance.m_DynamicResolutionActive = true;
                break;
        }
    }

    public void ResetData(int dataToReset)
    {
        GameManager.Instance.ResetData(dataToReset);

        FadeManager.Instance.FadeOutAndLoadScene("MainMenuScene");
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

    #region Unlockables
    public void UnlockData(ItemData data)
    {
        if (CurrencyManager.Instance.Credits < data.m_Price)
        {
            Debug.LogWarning("Not enough credits to unlock this item.");
            return;
        }
        
        switch (data.m_ItemType)
        {
            case ItemData.ItemType.Ship:
                UnlockShip(data);
                break;
            case ItemData.ItemType.Color:
                UnlockColor(data);
                break;
            case ItemData.ItemType.Upgrade:
                UnlockUpgrade(data);
                
                break;
            default:
                Debug.LogWarning("Invalid item type.");
                break;
        }

        CurrencyManager.Instance.RemoveCredits(data.m_Price);
    }

    private void UnlockShip(ItemData data)
    {
        GameManager.Instance.UnlockShipType(data.m_ID);

        UpdateShipTypeLocks();
        UpdateShipTypeShop();
    }

    private void UnlockColor(ItemData data)
    {
        GameManager.Instance.UnlockShipColor(data.m_ID);

        UpdateShipColorLocks();
        UpdateShipColorShop();
    }

    private void UnlockUpgrade(ItemData data)
    {
        throw new NotImplementedException();
    }
    #endregion
}