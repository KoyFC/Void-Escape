using UnityEngine;

public class MainMenuManagerScript : MonoBehaviour
{
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
