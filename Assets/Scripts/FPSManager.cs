using UnityEngine;

public class FPSManager : MonoBehaviour
{
    public static FPSManager Instance;

    private int m_FPS;
    [SerializeField] private TMPro.TextMeshProUGUI m_Text;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID
            Application.targetFrameRate = 60;
#endif
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InvokeRepeating("GetFPS", 0.5f, 0.5f);
    }

    private void GetFPS()
    {
        m_FPS = (int)(1.0f / Time.unscaledDeltaTime);
        m_Text.text = "FPS: " + m_FPS;
    }
}
