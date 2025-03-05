using UnityEngine;

public class DynamicResolutionTest : MonoBehaviour
{
    [SerializeField] private int m_Width = 1920;
    [SerializeField] private int m_Height = 1080;
    [SerializeField] private bool m_FullScreen = true;

    FrameTiming[] m_FrameTimings = new FrameTiming[3];

    [SerializeField] private float m_MaxResolutionWidthScale = 1.0f;
    [SerializeField] private float m_MaxResolutionHeightScale = 1.0f;
    [SerializeField] private float m_MinResolutionWidthScale = 0.5f;
    [SerializeField] private float m_MinResolutionHeightScale = 0.5f;
    [SerializeField] private float m_ScaleWidthIncrement = 0.1f;
    [SerializeField] private float m_ScaleHeightIncrement = 0.1f;

    float m_WidthScale = 1.0f;
    float m_HeightScale = 1.0f;

    // Variables for dynamic resolution algorithm that persist across frames
    uint m_FrameCount = 0;
    const uint kNumFrameTimings = 2;
    const uint kFrameInterval = 15; // Frame interval for dynamic resolution

    double m_GpuFrameTime;
    double m_CpuFrameTime;

    void Start()
    {
        SetResolution(m_Width, m_Height, m_FullScreen);
    }

    void FixedUpdate()
    {
        if (m_FrameCount % kFrameInterval == 0)
        {
            DetermineResolution();
            m_FrameCount = 0;
        }
        m_FrameCount++;
    }

    // Estimate the next frame time and update the resolution scale if necessary
    private void DetermineResolution()
    {
        FrameTimingManager.CaptureFrameTimings();
        FrameTimingManager.GetLatestTimings(kNumFrameTimings, m_FrameTimings);
        if (m_FrameTimings.Length < kNumFrameTimings)
        {
            return;
        }

        m_GpuFrameTime = (double)m_FrameTimings[0].gpuFrameTime;
        m_CpuFrameTime = (double)m_FrameTimings[0].cpuFrameTime;

        // Adjusting resolution scale based on frame time to achieve target frame rate
        double targetFrameTime = 1000.0 / Application.targetFrameRate; // Frame time in milliseconds
        bool resolutionChanged = false;

        if (m_GpuFrameTime > targetFrameTime || m_CpuFrameTime > targetFrameTime)
        {
            float newHeightScale = Mathf.Max(m_MinResolutionHeightScale, m_HeightScale - m_ScaleHeightIncrement);
            float newWidthScale = Mathf.Max(m_MinResolutionWidthScale, m_WidthScale - m_ScaleWidthIncrement);
            if (newHeightScale != m_HeightScale || newWidthScale != m_WidthScale)
            {
                m_HeightScale = newHeightScale;
                m_WidthScale = newWidthScale;
                resolutionChanged = true;
            }
        }
        else
        {
            float newHeightScale = Mathf.Min(m_MaxResolutionHeightScale, m_HeightScale + m_ScaleHeightIncrement);
            float newWidthScale = Mathf.Min(m_MaxResolutionWidthScale, m_WidthScale + m_ScaleWidthIncrement);
            if (newHeightScale != m_HeightScale || newWidthScale != m_WidthScale)
            {
                m_HeightScale = newHeightScale;
                m_WidthScale = newWidthScale;
                resolutionChanged = true;
            }
        }

        if (resolutionChanged)
        {
            ScalableBufferManager.ResizeBuffers(m_WidthScale, m_HeightScale);
        }
    }

    public void SetResolution(int width, int height, bool fullScreen)
    {
        Screen.SetResolution(width, height, fullScreen);
    }
}
