using UnityEngine;
using UnityEngine.InputSystem;

public class DebugScript : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI m_Text;

    private void Update()
    {
        m_Text.text = "FPS: " + (int)(1.0f / Time.unscaledDeltaTime)
            + "\nAccelerometer: " + Accelerometer.current + " / " + Accelerometer.current.enabled 
            + " / " + PlayerInputScript.Instance.m_Accelerometer
            + "\nGyroscope: " + UnityEngine.InputSystem.Gyroscope.current + " / " + UnityEngine.InputSystem.Gyroscope.current.enabled 
            + " / " + PlayerInputScript.Instance.m_Gyroscope;
    }
}
