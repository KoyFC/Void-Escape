using UnityEngine;

public class MainMenuManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject m_MainMenuPanel;
    [SerializeField] private GameObject m_ShopPanel;
    [SerializeField] private GameObject m_SettingsPanel;
    [SerializeField] private GameObject m_CreditsPanel;

    private void Awake()
    {
#if UNITY_ANDROID
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.AutoRotation;
#endif
    }
}
