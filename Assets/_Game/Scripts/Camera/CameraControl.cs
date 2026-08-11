using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    private Transform character;
    public float rotationSpeed = 500.0f;           // how fast the camera orbits with the right mouse
    [SerializeField] private float distance = 4f;
    [SerializeField] private float horizontalOffset = 1.5f;
    [SerializeField] private float verticalOffset = 1f;
    [SerializeField] private float pitchAngle = 15f;
    [SerializeField] private float minPitch = -40f;    // furthest the camera can look up (degrees)
    [SerializeField] private float maxPitch = 60f;     // furthest the camera can look down (degrees)
    private float yaw;                                 // horizontal orbit angle
    private float pitch;                               // vertical aim angle (degrees, positive is down)
    private Crosshair crosshair;

    // find the player by tag so the camera knows what to follow
    void Awake()
    {
        character = GameObject.FindGameObjectWithTag("Player").transform;
        crosshair = GetComponent<Crosshair>(); // the crosshair lives on the same camera
        pitch = pitchAngle;
        Vector3 flatForward = transform.forward;
        flatForward.y = 0f;
        if (flatForward.sqrMagnitude > 0.001f) {
            yaw = Quaternion.LookRotation(flatForward, Vector3.up).eulerAngles.y;
        }
    }

    // runs after all update calls, so the camera follows the character's new position this frame
    void LateUpdate()
    {
        CamOrbit(); // handle mouse look rotation
        // position behind + horizontal shift + fixed height offset
        transform.position = 
            character.position
            - (transform.forward * distance)
            + (transform.right * horizontalOffset)
            + (Vector3.up * verticalOffset);
        FaceCharacterToCamera(); // rotate the character to face the crosshair target
    }

    // orbit the camera around the character using mouse drag (right button held)
    void CamOrbit()
    {
        if (Mouse.current != null && Mouse.current.rightButton.isPressed) {
            Vector2 delta = Mouse.current.delta.ReadValue(); // mouse movement since last frame
            if (delta.sqrMagnitude >= 0.0001f)
            {
                yaw += delta.x * rotationSpeed * Time.deltaTime;
                pitch -= delta.y * rotationSpeed * Time.deltaTime;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }
        }
        transform.rotation = Quaternion.AngleAxis(yaw, Vector3.up) * Quaternion.AngleAxis(pitch, Vector3.right);
    }

    // make the character face the crosshair
    void FaceCharacterToCamera()
    {
        if (character == null || crosshair == null) return;
        Vector3 dir = crosshair.GetAimPoint() - character.position; // direction from character to crosshair target
        dir.y = 0f;                                        // flatten so the character stays upright
        if (dir.sqrMagnitude <= 0.001f) return;            // avoid zero direction
        character.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }
}
