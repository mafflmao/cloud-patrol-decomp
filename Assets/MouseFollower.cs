using UnityEngine;

public class MouseFollowerUI : MonoBehaviour
{
    private RectTransform rectTransform;

    void Start()
    {
        // Get the RectTransform component
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Get the mouse position in screen coordinates
        Vector2 mousePosition = Input.mousePosition;

        // Convert screen point to local point in the RectTransform's parent's coordinate system
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform, 
            mousePosition, 
            Camera.main, 
            out localPoint
        );

        // Update the RectTransform's anchored position
        rectTransform.anchoredPosition = Input.mousePosition;
    }
}
