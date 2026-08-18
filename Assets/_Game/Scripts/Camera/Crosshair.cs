using UnityEngine;

// draws the crosshair and provides the location it points at
public class Crosshair : MonoBehaviour
{
    private const float AimRange = 100f; // how far the aim projects when the ray hits nothing

    // point under the crosshair (always screen center = camera forward)
    public Vector3 GetAimPoint()
    {
        return GetAimPoint(out _);
    }

    // also return the physics hit so callers can use its surface data
    public Vector3 GetAimPoint(out RaycastHit hit)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        return Physics.Raycast(ray, out hit, AimRange)
            ? hit.point
            : ray.origin + ray.direction * AimRange;
    }

    // draw the crosshair at the center of the screen
    void OnGUI()
    {
        if (Event.current.type != EventType.Repaint) return;
        float cx = Screen.width * 0.5f, cy = Screen.height * 0.5f;
        GUI.DrawTexture(new Rect(cx - 1f, cy - 8f, 2f, 5f), Texture2D.whiteTexture); // top
        GUI.DrawTexture(new Rect(cx - 1f, cy + 3f, 2f, 5f), Texture2D.whiteTexture); // bottom
        GUI.DrawTexture(new Rect(cx - 8f, cy - 1f, 5f, 2f), Texture2D.whiteTexture); // left
        GUI.DrawTexture(new Rect(cx + 3f, cy - 1f, 5f, 2f), Texture2D.whiteTexture); // right
    }
}
