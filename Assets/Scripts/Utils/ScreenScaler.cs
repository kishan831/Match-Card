using UnityEngine;

public class ScreenScaler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; 
    [SerializeField] private float targetWorldWidth = 12f;  
    [SerializeField] private float targetWorldHeight = 6f;    
    [SerializeField] private float padding = 0.5f;



    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        AdjustCamera();
    }

    public void AdjustCamera()
    {
        if (mainCamera == null) return;

        float screenAspect = (float)Screen.width / Screen.height;
        float targetAspect = targetWorldWidth / targetWorldHeight;

        float totalWidth = targetWorldWidth + padding * 2f;
        float totalHeight = targetWorldHeight + padding * 2f;

        if (screenAspect >= targetAspect)
        {
            // Screen is wider — fit by height
            mainCamera.orthographicSize = totalHeight / 2f;
        }
        else
        {
            // Screen is taller — fit by width
            mainCamera.orthographicSize = totalWidth / (2f * screenAspect);
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        AdjustCamera();
    }
}