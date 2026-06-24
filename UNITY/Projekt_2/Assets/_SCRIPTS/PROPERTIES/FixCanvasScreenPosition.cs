using UnityEngine;

public class FixCanvasScreenPosition : MonoBehaviour
{
    public Vector3 SetCanvasWorldPosition()
    {
        Vector2 viewportPoint = Camera.main.ViewportToScreenPoint(new Vector2(0.25f, 0.5f));

        Vector3 newPoint = Camera.main.ScreenToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, 1f));

        return newPoint;
    }
}
