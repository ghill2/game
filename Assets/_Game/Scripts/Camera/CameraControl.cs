using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    private Transform character;                  // the player the camera follows/orbits
    private float rotationSpeed = 500.0f;         // how fast the camera orbits with the mouse
    private float distance = 4f;                  // current zoom distance between camera and character
    public float minDistance = 2f;                // closest the camera can zoom in
    public float maxDistance = 10f;               // farthest the camera can zoom out
    public float zoomSpeed = 50f;                 // how fast scroll-wheel zoom adjusts distance

    // Awake runs once; find the player by tag so the camera knows what to follow
    void Awake()
    {
        character = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // LateUpdate runs after all Update calls, so the camera follows the character's new position this frame
    void LateUpdate()
    {
        CamOrbit();                                              // handle mouse-look rotation
        Zoom();                                                  // handle scroll-wheel zoom
        transform.position = character.position - (transform.forward * distance); // position camera 'distance' behind where it looks
        FaceCharacterToCamera();                                 // optionally rotate the character to face camera forward
    }

    // Orbit the camera around the character using mouse drag (left or right button held)
    void CamOrbit()
    {
        if (Mouse.current == null) return;                       // bail out if no mouse is present
        if (Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed) {
            Vector2 delta = Mouse.current.delta.ReadValue();     // mouse movement since last frame
            if (delta.sqrMagnitude < 0.0001f) return;            // ignore negligible movement

            // Scale mouse delta into rotation amounts for this frame
            float verticalInput = delta.y * rotationSpeed * Time.deltaTime;   // pitch (up/down)
            float horizontalInput = delta.x * rotationSpeed * Time.deltaTime; // yaw (left/right)

            transform.Rotate(Vector3.right, -verticalInput, Space.Self);  // invert Y so dragging up looks up; pitch is local
            transform.Rotate(Vector3.up, horizontalInput, Space.World);   // yaw in world space to keep horizon level
        };
    }

    // Adjust the camera distance based on the scroll wheel
    void Zoom()
    {
        if (Mouse.current == null) return;
        float scroll = Mouse.current.scroll.ReadValue().y;       // positive = scroll up, negative = scroll down
        if (Mathf.Abs(scroll) > 0.01f)                           // ignore tiny/noise values
        {
            distance -= scroll * zoomSpeed * Time.deltaTime;     // invert so scroll up zooms in
            distance = Mathf.Clamp(distance, minDistance, maxDistance); // keep distance within allowed range
        }
    }

    // When the right mouse button is held, rotate the character to match the camera's facing
    void FaceCharacterToCamera()
    {
        if (character == null || Mouse.current == null) return;  // safety checks
        if (!Mouse.current.rightButton.isPressed) return;        // only face when right-button dragging

        Vector3 forward = transform.forward;   // camera's current forward direction
        forward.y = 0f;                        // flatten so the character doesn't tilt up/down
        if (forward.sqrMagnitude > 0.001f)     // avoid zero-direction LookRotation (would log an error)
            character.rotation = Quaternion.LookRotation(forward, Vector3.up); // snap character to face camera forward
    }
}
