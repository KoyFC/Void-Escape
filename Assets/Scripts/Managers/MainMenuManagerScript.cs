using UnityEngine;

public class MainMenuManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject m_MainMenuPanel;
    [SerializeField] private GameObject m_ShopPanel;
    [SerializeField] private GameObject m_SettingsPanel;
    [SerializeField] private GameObject m_CreditsPanel;

    public void ChangePanel(int index)
    {
        m_MainMenuPanel.SetActive(false);
        m_ShopPanel.SetActive(false);
        m_SettingsPanel.SetActive(false);
        m_CreditsPanel.SetActive(false);

        switch (index)
        {
            case 0:
                m_MainMenuPanel.SetActive(true);
                break;
            case 1:
                m_ShopPanel.SetActive(true);
                break;
            case 2:
                m_SettingsPanel.SetActive(true);
                break;
            case 3:
                m_CreditsPanel.SetActive(true);
                break;
        }
    }
}
