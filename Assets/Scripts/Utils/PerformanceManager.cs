using UnityEngine;

public class PerformanceManager : MonoBehaviour
{
    [Header("Target Frame Rate")]
    [SerializeField] private int targetFPS = 60;

    [Header("Quality")]
    [SerializeField] private bool enableVSync = false;

    private void Awake()
    {
        Application.targetFrameRate = targetFPS;
        QualitySettings.vSyncCount = enableVSync ? 1 : 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}