using Unity.Burst.Intrinsics;
using UnityEngine;

public class BuildConfig : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_ANDROID || UNITY_IOS
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
#endif

        Input.multiTouchEnabled = false;
    }
}