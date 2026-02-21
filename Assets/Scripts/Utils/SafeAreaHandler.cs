using UnityEngine;

public class SafeAreaHandler : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRect;

    private Rect lastSafeArea = Rect.zero;

    private void Update()
    {
        if (Screen.safeArea != lastSafeArea)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;

        if (canvasRect == null) return;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        canvasRect.anchorMin = anchorMin;
        canvasRect.anchorMax = anchorMax;
    }
}