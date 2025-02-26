using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    #region Variables
    public static FadeManager Instance = null;

    [Header("Object References")]
    [SerializeField] private SceneManagerScript m_SceneManager = null;

    [Header("UI Elements")]
    [SerializeField] private GameObject m_BlackPanel = null;
    private Image m_PanelImage;

    [Header("Fading Settings")]
    public float m_PanelFadeTime = 1f;
    public float m_SceneLoadDelay = 0.5f;
    #endregion

    #region Main Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        m_PanelImage = m_BlackPanel.GetComponent<Image>();
    }
    #endregion

    #region Events
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeInPanel());
    }
    #endregion

    #region Public Methods
    public void FadeOutAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }
    #endregion

    #region Coroutines
    private IEnumerator FadeInPanel()
    {
        m_PanelImage.color = new Color(0f, 0f, 0f, 1f);

        float elapsedTime = 0f;
        while (elapsedTime < m_PanelFadeTime)
        {
            elapsedTime += Time.deltaTime;
            m_PanelImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, elapsedTime / m_PanelFadeTime));
            yield return null;
        }
    }

    private IEnumerator FadeOutPanel()
    {
        m_PanelImage.color = new Color(0f, 0f, 0f, 0f);

        float elapsedTime = 0f;
        while (elapsedTime < m_PanelFadeTime)
        {
            elapsedTime += Time.deltaTime;
            m_PanelImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 1f, elapsedTime / m_PanelFadeTime));
            yield return null;
        }
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        StartCoroutine(FadeOutPanel());
        yield return new WaitForSeconds(m_PanelFadeTime + m_SceneLoadDelay);
        Debug.Log("Loading scene: " + sceneName);
        m_SceneManager.LoadScene(sceneName);
    }
    #endregion
}
